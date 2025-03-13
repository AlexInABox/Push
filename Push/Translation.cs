namespace Push
{
    using Exiled.API.Interfaces;

#pragma warning disable SA1600 // Elements should be documented
    public class Translation : ITranslation
    {
        // General Text
        public string KeybindSettingLabel { get; set; } = "Push someone in front of you!";

        public string KeybindSettingHintDescription { get; set; } =
            "Pressing this will push the player in front of you! Don't be mean :3";

        public string PlayerPushCooldownHint { get; set; } =
            "You cannot push yet! <color=yellow>Cooldown is active.</color> $remainingCooldown$ seconds remaining...";

        public string PlayerPushSuccessfulHint { get; set; } =
            "You <color=green>pushed</color> $player$!";

        public string PlayerGotPushedHint { get; set; } =
            "You have been <color=yellow>pushed</color> by $player$!!";
    }
#pragma warning restore SA1600 // Elements should be documented
}