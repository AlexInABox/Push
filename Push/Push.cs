namespace Push
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Events;
    using UserSettings.ServerSpecific;
    using Exiled.API.Features.Core.UserSettings;


    public class Push : Plugin<Config>
    {
        public override string Prefix => "Push";
        public override string Name => "Push";
        public override string Author => "AlexInABox";
        public override Version Version => new Version(1, 0, 0);

        private static Push Singleton;
        public static Push Instance => Singleton;
        private SettingValueReceived settingValueReceived;
        
        public override PluginPriority Priority { get; } = PluginPriority.Last;
        public override void OnEnabled()
        {
            Singleton = this;
            Log.Info("Push has been enabled!");

            settingValueReceived = new SettingValueReceived();

            HeaderSetting header = new HeaderSetting("Push");
            IEnumerable<SettingBase> settingBases = new SettingBase[]
            {
                header,
                new KeybindSetting(Config.KeybindId, "Push someone in front of you!", default, hintDescription: "Pressing this will push the player in front of you! Don't be mean :3"),
            };

            SettingBase.Register(settingBases);
            SettingBase.SendToAll();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += settingValueReceived.OnSettingValueReceived;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Log.Info("Push has been disabled!");

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= settingValueReceived.OnSettingValueReceived;

            base.OnDisabled();
        }
    }
}