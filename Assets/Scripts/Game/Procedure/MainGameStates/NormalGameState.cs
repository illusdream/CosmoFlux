using Game.Input;
using ilsFramework.Core;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game
{
    public class NormalGameState  : ProcedureNode
    {
        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnEnter()
        {
            CameraManager.Instance.SetAsMainCamera(CameraManager.Instance.GetCameraInstance<ShipControlBaseCamera>());
            CursorUtility.LockCursor();
            "进入正常游戏模式".LogSelf();
            InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchBuildMode.performed += SwitchBuildModeOnperformed;
            base.OnEnter();
        }




        public override void OnUpdate()
        {

            base.OnUpdate();
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnLogicUpdate()
        {
            base.OnLogicUpdate();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchBuildMode.performed -= SwitchBuildModeOnperformed;
            base.OnExit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        
        private void SwitchBuildModeOnperformed(InputAction.CallbackContext obj)
        {
            ChangeState<GameBuilderState>();
        }
    }
}