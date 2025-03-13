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

        public int PlayerPushSuccessfulHintDuration { get; set; } = 1;

        [Description("Cooldown between pushes.")]
        public int PlayerPushHintDuration { get; set; } = 3;
        
        [Description("Hint displayed to the pushed player.")]
        public int PlayerGotPushedHintDuration { get; set; } = 2;


        [Description("The unique id of the setting.")]
        public int KeybindId { get; set; } = 202;
    }
}