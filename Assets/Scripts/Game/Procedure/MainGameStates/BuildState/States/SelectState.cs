using System.ComponentModel;
using ilsFramework.Core;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Game
{
    public partial class  GameBuilderState : State
    {
        public class SelectState : ProcedureNode
        {
            RaycastHit[] _hit;
        
            int _hitCount;
            
            ModelReference<GameBuildStateModel> _modelReference;

            private BaseModularNode oldNode;
            public override void OnInit()
            {
                _hit = new RaycastHit[1];
                _modelReference = new ModelReference<GameBuildStateModel>();
                base.OnInit();
            }

            public override void OnEnter()
            {
                _modelReference.Value.Stage = Stage.SelectState;
                UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
                {
                    if (!view.IsOpen)
                        view.Open();
                });
                CursorUtility.LockCursor();
                CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CanTranslation = true;
                _modelReference.Value.PropertyChanged += ValueOnPropertyChanged;
                base.OnEnter();
            }



            public override void OnUpdate()
            {
                UpdateEyeRaycast();
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
                if (oldNode && (oldNode.TryGetComponent<RenderMaterialCollection>(out var old_rmc)))
                {
                    old_rmc.RemoveMaterial("Modular_Selected");
                }
                _modelReference.Value.PropertyChanged -= ValueOnPropertyChanged;
                base.OnExit();
            }
            private void UpdateEyeRaycast()
            {
                var cTransform = CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CameraObject.transform;
                _hitCount = Physics.RaycastNonAlloc(new Ray(cTransform.position,
                    cTransform.TransformDirection(Vector3.forward * 100)), _hit,100,ColliderLayer.BoundMask);
                _modelReference.Value.SelectedNode = _hitCount > 0 ? _hit[0].transform?.parent?.GetComponent<BaseModularNode>() : null;
            }
            
            public override void OnGizmos()
            {
                Gizmos.color = Color.green;
                var transform = CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CameraObject.transform;
                Gizmos.DrawRay(transform.position,transform.TransformDirection(Vector3.forward * 100));
            }
            
            private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(GameBuildStateModel.SelectedNode))
                {
                    if (oldNode && (oldNode.TryGetComponent<RenderMaterialCollection>(out var old_rmc)))
                    {
                        old_rmc.RemoveMaterial("Modular_Selected");
                    }
                    if (_modelReference.Value.SelectedNode && (_modelReference.Value.SelectedNode.TryGetComponent<RenderMaterialCollection>(out var rmc)))
                    {
                        rmc.AddMaterial("Modular_Selected",_modelReference.Value.SelectedMaterial);
                        oldNode = _modelReference.Value.SelectedNode;
                    }
                }
            }
        }
    }
}