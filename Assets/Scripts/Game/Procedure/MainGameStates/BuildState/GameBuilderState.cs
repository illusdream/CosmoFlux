using System;
using System.ComponentModel;
using Game.Input;
using ilsFramework.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public partial class GameBuilderState : State
    {
        public const string StateKey = "GameBuilderState";
        BuilderBaseCamera _baseCamera;

        public enum Stage
        {
            ModifyState,
            PlaceState,
            SelectState,
            ShowModularState,
        }
        
        Collider _collider;

        RaycastHit[] _hit;
        
        int _hitCount;
        
        GameObject _prefab;

        GameObject ShowModel;
        
        public ModelReference<GameBuildStateModel> Model = new ModelReference<GameBuildStateModel>();

        public override void OnInit()
        {
            //this.AddState<SelectState>();
            //this.AddState<ModifyState>();
            //this.AddState<ShowModularState>();
            //this.AddState<PlaceState>();
            _hit = new RaycastHit[1];
            base.OnInit();
        }

        public override void OnEnter()
        {
            ShipManager.BuildTargetShip = ShipManager.PlayerControlShip;
            _baseCamera ??= CameraManager.Instance.GetCameraInstance<BuilderBaseCamera>();
            "进入建造游戏模式".LogSelf();
            // 
            // Model.Value.PropertyChanged += ValueOnPropertyChanged;
            var buildCamera = CameraManager.Instance.GetCameraInstance<BuilderBaseCamera>();
            buildCamera.Reset();
            CameraManager.Instance.SetAsMainCamera(buildCamera);
            InputManager.Instance.GetCurrentInputAction().BuildMode.OpenModularList.performed += OpenModularListOnperformed;
            InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchBuildMode.performed += SwitchBuildModeOnperformed;

            InputManager.Instance.SetPlayActionEnabledInput(false);

            Model.Value.EditingShipCore = ShipManager.PlayerControlShip;
            //StartState<SelectState>();

            base.OnEnter();
        }

        public override State GetTransition()
        {
            if(InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchBuildMode.WasPressedThisDynamicUpdate())
                return GetStateInSameLayer(NormalGameState.StateKey);
            return base.GetTransition();
        }

        private void SwitchBuildModeOnperformed(InputAction.CallbackContext obj)
        {
            //Owner.ChangeState<NormalGameState>();
        }

        private void OpenModularListOnperformed(InputAction.CallbackContext obj)
        {
           // if (_currentState is not ShowModularState)
           // {
               // ChangeState<ShowModularState>();
           // }

        }


        public override void OnLogicUpdate()
        {
            if (!ShipManager.BuildTargetShip)
            {
               // Owner.ChangeState<NormalGameState>();
            }
            base.OnLogicUpdate();

        }
        
        
        public override void OnExit()
        {

            
            if (ShipManager.PlayerControlShip)
            {
                ShipManager.PlayerControlShip.Save();
            }
            UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
            {
                if (view.IsOpen)
                    view.Close();
            });
            InputManager.Instance.GetCurrentInputAction().BuildMode.OpenModularList.performed -= OpenModularListOnperformed;
            InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchBuildMode.performed -= SwitchBuildModeOnperformed;
            
            InputManager.Instance.SetPlayActionEnabledInput(true);
            base.OnExit();
        }

        public  void OnGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_baseCamera.CameraObject.transform.position, _baseCamera.CameraObject.transform.TransformDirection(Vector3.forward * 100));
        }
    }
}