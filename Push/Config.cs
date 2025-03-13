namespace Push
{
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        public Config()
        {
        }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool Debug { get; set; }

        [Description("Hint displayed when the player was successfully pushed.")]
        public Hint PlayerPushSuccessful { get; private set; } = new("You <color=green>pushed</color> $player!", 1);

        [Description("Hint displayed when the push is still on cooldown.")]
        public Hint PlayerPushCooldown { get; private set; } =
            new("You cannot push yet! <color=yellow>Cooldown is active.</color>", 3);

        [Description("Hint displayed when the push is from the wrong angle.")]
        public Hint PlayerPushWrongAngle { get; private set; } =
            new("Push failed! <color=red>You need to align better.</color>", 3);

        [Description("Hint displayed to the pushed player.")]
        public Hint PlayerGotPushed { get; private set; } =
            new("You have been <color=yellow>pushed</color> by $player!!", 1);

        [Description("The unique id of the setting.")]
        public int KeybindId { get; set; } = 202;
    }
}