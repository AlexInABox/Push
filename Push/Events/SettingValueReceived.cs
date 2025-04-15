using UnityEngine.Experimental.GlobalIllumination;

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
                    Push.Translations.PlayerPushCooldownHint.Replace("$remainingCooldown$",
                        remainingCooldown.ToString()),
                    Push.Instance.Config.PlayerPushHintDuration);
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

            // Log the object name and its layer
            GameObject hitObject = raycastHit.transform.gameObject;
            string layerName = LayerMask.LayerToName(hitObject.layer);
            Log.Debug($"Hit object: {hitObject.name}, Layer: {layerName} ({hitObject.layer})");

            // Check if the hit object is a player
            if (Player.TryGet(raycastHit.transform.gameObject, out Player targetedPlayer))
            {
                Timing.RunCoroutine(ApplyPushForce(targetedPlayer, forwardDirection.normalized));

                player.ShowHint(
                    Push.Translations.PlayerPushSuccessfulHint.Replace("$player$", targetedPlayer.Nickname),
                    Push.Instance.Config.PlayerPushSuccessfulHintDuration);

                targetedPlayer.ShowHint(
                    Push.Translations.PlayerGotPushedHint.Replace("$player$", player.Nickname),
                    Push.Instance.Config.PlayerGotPushedHintDuration);

                // Update the player's cooldown time
                _pushCooldowns[player.Id] = currentTime;
            }
        }


        private IEnumerator<float> ApplyPushForce(Player player, Vector3 direction)
        {

            float PUSH_DISTANCE = Push.Instance.Config.PushForce; // total push distance
            const float PUSH_DURATION = 0.000000000001f; //sweet spot!
            
            const int mask = (1 << 0)  // Default
                       | (1 << 25) // OnlyWorldCollision
                       | (1 << 27) // Door
                       | (1 << 29); // Fence

            for (int i = 0; i < 20; i++)
            {
                if (Physics.Raycast(player.Position, direction, out RaycastHit raycastHit, 1f, mask))
                {
                    Log.Debug("Cant push since wall.");
                    break;
                }

                player.Position += direction * (PUSH_DISTANCE / 50);
                yield return Timing.WaitForSeconds((PUSH_DURATION / 50));
            }

            Log.Debug(PUSH_DURATION / 50);
            Log.Debug("Push force applied");
        }
    }
}