using ReduxLib.Configuration;
using ReduxLib.Input;
using UnityEngine;

namespace WasdForVab
{
    public class WasdConfig
    {
        public ConfigValue<float> CameraSensitivity;
        public ConfigValue<float> BaseSpeed;
        public ConfigValue<float> FastSpeedMultiplier;
        public ConfigValue<float> SlowSpeedMultiplier;
        public ConfigValue<bool> RequireRightClickForControl;
        public ConfigValue<KeyboardShortcut> KeyToggleEnabled;
        public ConfigValue<KeyBind> KeyForward;
        public ConfigValue<KeyBind> KeyLeft;
        public ConfigValue<KeyBind> KeyBack;
        public ConfigValue<KeyBind> KeyRight;
        public ConfigValue<KeyBind> KeyUp;
        public ConfigValue<KeyBind> KeyDown;
        public ConfigValue<KeyBind> KeyFast;
        public ConfigValue<KeyBind> KeySlow;
        public ConfigValue<KeyBind> KeySlowToggle;

        public void Initialize(IConfigFile config)
        {
            CameraSensitivity = new(config.Bind("Settings", "Camera Sensitivity", 1.0f));
            BaseSpeed = new(config.Bind("Settings", "Base Speed", 20.0f));
            FastSpeedMultiplier = new(config.Bind("Settings", "Fast Speed Multiplier", 3.0f));
            SlowSpeedMultiplier = new(config.Bind("Settings", "Slow Speed Multiplier", 0.4f));
            RequireRightClickForControl = new(config.Bind("Settings", "Require Right Click For Control", false));
            KeyToggleEnabled = new(config.Bind("Settings", "Key Toggle Enabled", new KeyboardShortcut(KeyCode.W, KeyCode.LeftAlt)));
            KeyForward = new(config.Bind("Settings", "Key Forward", new KeyBind(KeyCode.W)));
            KeyLeft = new(config.Bind("Settings", "Key Left", new KeyBind(KeyCode.A)));
            KeyBack = new(config.Bind("Settings", "Key Back", new KeyBind(KeyCode.S)));
            KeyRight = new(config.Bind("Settings", "Key Right", new KeyBind(KeyCode.D)));
            KeyUp = new(config.Bind("Settings", "Key Up", new KeyBind(KeyCode.E)));
            KeyDown = new(config.Bind("Settings", "Key Down", new KeyBind(KeyCode.Q)));
            KeyFast = new(config.Bind("Settings", "Key Fast", new KeyBind(KeyCode.LeftShift)));
            KeySlow = new(config.Bind("Settings", "Key Slow", new KeyBind(KeyCode.LeftControl)));
            KeySlowToggle = new(config.Bind("Settings", "Key Slow Toggle", new KeyBind(KeyCode.None)));
        }
    }
}