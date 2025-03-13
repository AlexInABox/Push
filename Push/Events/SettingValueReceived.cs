namespace Push.Events
{
    using UserSettings.ServerSpecific;
    using Exiled.API.Features;
    using System.Collections.Generic;
    using UnityEngine;
    using MEC;

    internal sealed class SettingValueReceived
    {
        public void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player))
                return;

            if (settingBase is SSKeybindSetting keybindSetting &&
                keybindSetting.SettingId == Push.Instance.Config.KeybindId &&
                keybindSetting.SyncIsPressed)
            {
                TryToPush(player);
            }
        }


        private readonly Dictionary<int, float> _pushCooldowns = new Dictionary<int, float>();

        private void TryToPush(Player player)
        {
            // Check if player is a human
            if (!player.IsHuman)
                return;

            // Get the current epoch time in seconds
            float currentTime = Time.time;

            // Check if the player is on cooldown
            if (_pushCooldowns.TryGetValue(player.Id, out float lastPushTime) && currentTime - lastPushTime < 10f)
            {
                float remainingCooldown = 10 - (currentTime - lastPushTime);
                remainingCooldown = Mathf.Round(remainingCooldown * 10f) / 10f; // Round to one decimal place
                Log.Debug("Player is on cooldown for pushing.");
                player.ShowHint(
                    Push.Instance.Config.PlayerPushCooldown.Content + " " + remainingCooldown + " seconds remaining...",
                    Push.Instance.Config.PlayerPushCooldown.Duration);
                return;
            }

            // Get the player's position and the direction they are facing
            Vector3 playerPosition = player.Position;
            Vector3
                forwardDirection =
                    player.Rotation * Vector3.forward; // Using player's rotation to determine forward direction

            // Check what the player is shooting at with a raycast
            if (!Physics.Raycast(playerPosition, player.Rotation * Vector3.forward, out RaycastHit raycastHit, 1f,
                    ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            // Log the object hit by the raycast
            Log.Debug($"Hit object: {raycastHit.transform.gameObject.name}");

            // Check if the hit object is a player
            if (Player.TryGet(raycastHit.transform.gameObject, out Player targetedPlayer))
            {
                float angle = Vector3.Angle(player.Rotation * Vector3.forward,
                    targetedPlayer.Rotation * Vector3.forward);
                const float MAX_ALLOWED_ANGLE = 360f; // Maximum angle (in degrees) to allow the push

                if (angle <= MAX_ALLOWED_ANGLE)
                {
                    Timing.RunCoroutine(ApplyPushForce(targetedPlayer, forwardDirection.normalized));

                    string hintContent = Push.Instance.Config.PlayerPushSuccessful.Content;
                    string customizedHint = hintContent.Replace("$player", targetedPlayer.Nickname);
                    player.ShowHint(customizedHint, Push.Instance.Config.PlayerPushSuccessful.Duration);

                    hintContent = Push.Instance.Config.PlayerGotPushed.Content;
                    customizedHint = hintContent.Replace("$player", player.Nickname);
                    targetedPlayer.ShowHint(customizedHint, Push.Instance.Config.PlayerGotPushed.Duration);

                    // Update the player's cooldown time
                    _pushCooldowns[player.Id] = currentTime;
                }
                else
                {
                    Log.Debug("Player is not within the allowed angle for pushing.");
                    Log.Debug(angle.ToString());
                    player.ShowHint(Push.Instance.Config.PlayerPushWrongAngle.Content,
                        Push.Instance.Config.PlayerPushWrongAngle.Duration);
                }
            }
        }


        private IEnumerator<float> ApplyPushForce(Player player, Vector3 direction)
        {
            const float PUSH_DISTANCE = 3f; // total push distance
            const float PUSH_DURATION = 0.000000000001f; //sweet spot!

            for (int i = 0; i < 20; i++) //repeat the loop infinitely
            {
                player.Position += direction * (PUSH_DISTANCE / 50);
                yield return Timing.WaitForSeconds((PUSH_DURATION / 50));
            }

            Log.Debug(PUSH_DURATION / 50);
            Log.Debug("Push force applied");
        }
    }
}