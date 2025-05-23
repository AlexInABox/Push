namespace Push
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Events;
    using UserSettings.ServerSpecific;
    using Exiled.API.Features.Core.UserSettings;


    public class Push : Plugin<Config, Translation>
    {
        public override string Prefix => "Push";
        public override string Name => "Push";
        public override string Author => "AlexInABox";
        public override Version Version => new Version(1, 3, 2);

        private static Push Singleton;
        public static Translation Translations => Singleton.Translation;
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
                new KeybindSetting(Config.KeybindId, Translation.KeybindSettingLabel, default,
                    hintDescription: Translation.KeybindSettingHintDescription),
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