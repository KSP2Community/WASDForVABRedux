using HarmonyLib;
using KSP.Messages;
using KSP.OAB;
using Redux.ExtraModTypes;
using UnityEngine;

namespace WasdForVab
{
    /* Extend KerbalMod instead if you need the MonoBehaviour update loop/references to game stuff like SW 1.x mods */
    public class WasdForVabPlugin : KerbalMod
    {
        private SubscriptionHandle _loadSubscription;
        private readonly WasdConfig _config = new();
        private bool _slowToggled = false;
        private Harmony _harmony;

        public override void OnPreInitialized()
        {
            _config.Initialize(SWConfiguration);
        }

        public override void OnInitialized()
        {
            _loadSubscription = Game.Messages.PersistentSubscribe<OABLoadedMessage>(OnOABLoadFinalized);
            _harmony = CreateHarmonyAndPatchAll();
        }

        public override void OnPostInitialized()
        {
        }

        private void OnOABLoadFinalized(MessageCenterMessage msg)
        {
            _slowToggled = false;
            WasdPatches.patchState.IsEnabled = true;
            if (Game != null && Game.OAB != null && Game.OAB.Current != null)
            {
                ObjectAssemblyBuilder current = Game.OAB.Current;
                WasdPatches.patchState.OnLoad(current.CameraManager);
            }
        }

        public void Update()
        {
            WasdPatches.patchState.IsCtrlPressed = false;
            if (Game == null) return;
            if (Game.OAB is not { IsLoaded: true }) return;
            // Toggle Wasd cam on ALT+w
            if (_config.KeyToggleEnabled.Value.Down)
            {
                WasdPatches.patchState.IsEnabled = !WasdPatches.patchState.IsEnabled;
            }

            if (_config.RequireRightClickForControl.Value && !Input.GetMouseButton(1))
                return;

            if (_config.KeySlowToggle.Value.Down)
            {
                _slowToggled = !_slowToggled;
            }

            var inputVector = new Vector3d();

            if (_config.KeyForward.Value.Held)
            {
                inputVector.z = 1;
            }

            if (_config.KeyBack.Value.Held)
            {
                inputVector.z = -1;
            }

            if (_config.KeyRight.Value.Held)
            {
                inputVector.x = 1;
            }

            if (_config.KeyLeft.Value.Held)
            {
                inputVector.x = -1;
            }

            if (_config.KeyUp.Value.Held)
            {
                inputVector.y = 1;
            }

            if (_config.KeyDown.Value.Held)
            {
                inputVector.y = -1;
            }

            if (_config.KeyFast.Value.Held)
            {
                inputVector *= _config.FastSpeedMultiplier.Value;
            }

            if (_slowToggled || _config.KeySlow.Value.Held)
            {
                inputVector *= _config.SlowSpeedMultiplier.Value;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                WasdPatches.patchState.IsCtrlPressed = true;
            }

            if (!inputVector.IsZero())
            {
                WasdPatches.patchState.OnMove(inputVector, Time.deltaTime);
            }
        }

        public void OnDestroy()
        {
            if (Game != null)
            {
                Game.Messages.Unsubscribe(ref _loadSubscription);
            }

            _harmony.UnpatchAll(_harmony.Id);
        }
    }
}