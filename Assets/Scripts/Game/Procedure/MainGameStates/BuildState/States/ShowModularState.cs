using System.ComponentModel;
using Game.Input;
using ilsFramework.Core;
using UnityEngine.InputSystem;

namespace Game
{
    public partial class  GameBuilderState : SubProcedureController
    {
        public class ShowModularState : ProcedureNode
        {
            private ModelReference<GameBuildStateModel> _modelReference;
            public override void OnGizmos()
            {
                base.OnGizmos();
            }

            public override void OnInit()
            {
                _modelReference = new ModelReference<GameBuildStateModel>();
                base.OnInit();
            }

            public override void OnEnter()
            {
                _modelReference.Value.Stage = Stage.ShowModularState;
                CursorUtility.FreeCursor();
                CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CanTranslation = false;
                UIManager.Instance.GetUIPanelAsync<UIModularListView>((view) =>
                {
                    view.Open();
                });
                UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
                {
                    if (view.IsOpen)
                        view.Close();
                });
                _modelReference.Value.PropertyChanged += ValueOnPropertyChanged;
                InputManager.Instance.GetCurrentInputAction().BuildMode.OpenModularList.performed += OpenModularListOnperformed;
                base.OnEnter();
            }

            private void OpenModularListOnperformed(InputAction.CallbackContext obj)
            {
                ChangeState<PlaceState>();
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
                UIManager.Instance.GetUIPanelAsync<UIModularListView>((view) =>
                {
                    view.Close();
                });
                _modelReference.Value.PropertyChanged -= ValueOnPropertyChanged;
                InputManager.Instance.GetCurrentInputAction().BuildMode.OpenModularList.performed -= OpenModularListOnperformed;
                base.OnExit();
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
            }
            private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(GameBuildStateModel.UseModelID) && _modelReference.Value.UseModelID >0)
                {
                    ChangeState<PlaceState>();
                }
            }
        }
    }
}