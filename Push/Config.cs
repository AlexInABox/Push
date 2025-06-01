using System.ComponentModel;

namespace Push;

public class Config
{
    public bool Enabled { get; set; } = true;
    public bool Debug { get; set; }

    [Description("Hint displayed when the player was successfully pushed.")]
    public int PlayerPushSuccessfulHintDuration { get; set; } = 1;

    [Description("Cooldown between pushes.")]
    public int PlayerPushHintDuration { get; set; } = 3;

    [Description("Hint displayed to the pushed player.")]
    public int PlayerGotPushedHintDuration { get; set; } = 2;

    [Description("Hint displayed to the pushed player.")]
    public float PushForce { get; set; } = 8.0f;


    [Description("The unique id of the setting.")]
    public int KeybindId { get; set; } = 202;
}