using System;
using System.Collections.Generic;
using System.ComponentModel;
using Game.Input;
using Game.VisualEffect;
using ilsFramework.Core;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Asset = ilsFramework.Core.Asset;
using Collider = UnityEngine.Collider;
using Material = UnityEngine.Material;
using RaycastHit = UnityEngine.RaycastHit;

namespace Game
{
    public partial class  GameBuilderState : State
    {
        public class PlaceState : ProcedureNode
        {
            ModelReference<GameBuildStateModel> _modelReference;

            private GameObject showModel;
            private GameObject prefab;
            
            RaycastHit[] _hit;
            int _hitCount;

            private Vector3 _dir;
            
            private Vector3 panelCenter;
            
            private Vector3 panelNormal;
            
            private GridPanelController _gridPanelController;

            public bool hasGridPanelController;
            
            private bool isLoadGridPanelController;
            
            private AimCenterController _selectModularFaceCenter;

            private AimCenterController _placePositionController;

            public const float FindModularMaxDistance = 12f;
            
            private float placeDistance;
            
            public const float PlaceDistanceChangeDelta = 0.1f;

            private bool rebuild;
            
            
            public override void OnInit()
            {

                _modelReference = new ModelReference<GameBuildStateModel>();
                _hit = new RaycastHit[10];
                base.OnInit();
            }

            public override void OnEnter()
            {
                "Enter 放置模式".LogSelf();
                _modelReference.Value.Stage = Stage.PlaceState;
                UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
                {
                    if (!view.IsOpen)
                        view.Open();
                });
                CursorUtility.LockCursor();
                CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CanTranslation = true;
                _modelReference.Value.PropertyChanged += ValueOnPropertyChanged;
                RegisterAllRotationHandleInSnap();
                InputManager.Instance.GetCurrentInputAction().BuildMode.LeftClick.performed += HandleLeftClick;
                InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchSnapMode.performed += HandleSwitchSnapMode;
                GameObject.Destroy(showModel);
                //加载模型
                LoadPlaceModular();
                base.OnEnter();
            }

            private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case nameof(GameBuildStateModel.SelectedNode):
                    {
                        placeDistance = 1;
                        if (_modelReference.Value.SelectedNode)
                        {
                            CloseBuildGridPanel();
                            ShowBuildGridPanel();
                            CloseSelectedNodeFaceCenterAimCenter();
                            ShowSelectedNodeFaceCenterAimCenter();
                            ClosePlacePositionAimCenter();
                            ShowPlacePositionAimCenter();
                        }
                        else
                        {
                            CloseBuildGridPanel();
                            CloseSelectedNodeFaceCenterAimCenter();
                            ClosePlacePositionAimCenter();
                        }
                        break;
                    }
                    case nameof(GameBuildStateModel.IsInSnapMode) : 
                    {
                        if (_modelReference.Value.IsInSnapMode)
                        {
                            if (!_gridPanelController)
                            {
                                ShowBuildGridPanel();
                                ShowSelectedNodeFaceCenterAimCenter();
                            }
                            Vector3 rot = _modelReference.Value.EularAngleOfEditorGO;
                            rot.x = Mathf.Round(rot.x / 90) * 90f;
                            rot.y = Mathf.Round(rot.y / 90) * 90f;
                            rot.z = Mathf.Round(rot.z / 90) * 90f;
                            _modelReference.Value.EularAngleOfEditorGO  = rot;
                            RegisterAllRotationHandleInSnap();
                        }
                        else
                        {
                            CloseBuildGridPanel();
                            CloseSelectedNodeFaceCenterAimCenter();
                            UnRegisterAllRotationHandleInSnap();
                        }
                        
                        break;
                    }
                    case nameof(GameBuildStateModel.HitNormal):
                    {
                        if (!_modelReference.Value.SelectedNode)
                        {
                            return;
                        }
                        if (_modelReference.Value.HitNormal == Vector3.zero)
                        {
                            CloseBuildGridPanel();
                            CloseSelectedNodeFaceCenterAimCenter();
                            ClosePlacePositionAimCenter();
                        }
                        else
                        {
                            CloseBuildGridPanel();
                            ShowBuildGridPanel();
                            CloseSelectedNodeFaceCenterAimCenter();
                            ShowSelectedNodeFaceCenterAimCenter();
                            ClosePlacePositionAimCenter();
                            ShowPlacePositionAimCenter();
                        }
                        break;
                    }
                }
            }

            public override void OnUpdate()
            {
                if (!showModel || !prefab)
                {
                    return;
                }
                try
                {
                    
                }
                finally{}
                UpdateEyeRaycast();
                UpdateRotation();
                UpdatePlaceDistance();
                if (_modelReference.Value.SelectedNode)
                {
                    var obb = OBB.CalculateOBBFromBoxColliderShape(showModel.GetComponent<BaseModularNode>().BuildHelperBound,showModel.transform);
                    var off = OBB.GetDistanceToEdgeInDirection(obb,_modelReference.Value.HitNormal) *placeDistance;
                    // 获取BoxCollider的Transform和其本地轴
                    Transform boxTransform = _modelReference.Value.SelectedNode.transform;
                    Vector3 boxRight = boxTransform.right;
                    Vector3 boxUp = boxTransform.up;
                    Vector3 boxForward = boxTransform.forward;
            
                    //使用FromToRotation保持up对齐法线，forward尽量保持与boxCollider的forward平行
                    Vector3 targetForward = Vector3.ProjectOnPlane(boxForward, _hit[0].normal).normalized;
            
                    // 如果投影后长度为0，说明法线与forward平行，使用right轴
                    if (targetForward.sqrMagnitude < 0.001f)
                    {
                        targetForward = Vector3.ProjectOnPlane(boxRight,_hit[0].normal).normalized;
                    }
                    // 计算最终的旋转
                    Quaternion targetRotation = Quaternion.LookRotation(targetForward, _hit[0].normal);
                    if (_modelReference.Value.IsInSnapMode)
                    {
                        var obb2 = OBB.CalculateOBBFromBoxColliderShape(_modelReference.Value.SelectedNode.BuildHelperBound,_modelReference.Value.SelectedNode.transform);
                        var center = obb2.GetNearestFaceCenter(_modelReference.Value.HitPoint);
                
                        // 应用旋转（保持其他方向不变）
                        if (_gridPanelController)
                        {
                            _gridPanelController.transform.rotation = targetRotation;
                            // 计算从当前up向量到目标法向量的旋转
                            _gridPanelController.transform.position =center;
                        }

                        if (_selectModularFaceCenter)
                        {
                            _selectModularFaceCenter.transform.rotation = targetRotation;
                            _selectModularFaceCenter.transform.position = center;
                        }
                        Vector3 projectedWorldVec = Vector3.ProjectOnPlane(_modelReference.Value.HitPoint - _modelReference.Value.SelectedNode.transform.position,_modelReference.Value.HitNormal);
                        Vector3 localPos = _modelReference.Value.SelectedNode.transform.InverseTransformVector(projectedWorldVec);
                        Vector3 snappedPos = new Vector3(Mathf.Round(localPos.x), Mathf.Round(localPos.y), Mathf.Round(localPos.z));
                        Vector3 offest = _modelReference.Value.SelectedNode.transform.TransformVector(snappedPos);
                        if (_placePositionController)
                        {
                            _placePositionController.transform.rotation = targetRotation;
                            _placePositionController.transform.position = center + offest;
                        }
                        showModel.transform.position = center+ _modelReference.Value.HitNormal * off+offest; 
                    }
                    else
                    {
                        if (_placePositionController)
                        {
                            _placePositionController.transform.rotation = targetRotation;
                            _placePositionController.transform.position = _hit[0].point;
                        }
                        showModel.transform.position = _modelReference.Value.HitPoint + _modelReference.Value.HitNormal * off; 
                       
                    }
                    showModel.transform.rotation = Quaternion.Euler(_modelReference.Value.EularAngleOfEditorGO);
                }
                else
                {
                    var cTransform = CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CameraObject.transform;
                    showModel.transform.position =cTransform.transform.position + cTransform.TransformDirection(Vector3.forward * FindModularMaxDistance); 
                }
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
                _modelReference.Value.UseModelID = -1;
                _modelReference.Value.SelectedNode = null;
                _modelReference.Value.PropertyChanged -= ValueOnPropertyChanged;
                InputManager.Instance.GetCurrentInputAction().BuildMode.LeftClick.performed -= HandleLeftClick;
                InputManager.Instance.GetCurrentInputAction().BuildMode.SwitchSnapMode.performed -= HandleSwitchSnapMode;
                GameObject.Destroy(showModel);
                showModel = null;
                prefab = null;
                CloseBuildGridPanel();
                CloseSelectedNodeFaceCenterAimCenter();
                ClosePlacePositionAimCenter();
                UnRegisterAllRotationHandleInSnap();
                base.OnExit();
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
            }

            private void LoadPlaceModular()
            {
                if (_modelReference.Value.UseModelID > 0)
                {
                    var modularInfo = Table<tShipModular>.Get((uint)_modelReference.Value.UseModelID);
                    if (modularInfo != null)
                    {
                        Asset.LoadResourceAsync<GameObject>("ShipModular", modularInfo.Path, LoadResourcePriority.Default, model =>
                        {
                            "PlaceState 资源加载完成".LogSelf();
                            var operation=model.InstantiateAsync();
                            operation.Completed += (_) =>
                            {
                                $"PlaceState 模型加载完毕".LogSelf();
                                if (!IsExecuting)
                                {
                                    return;
                                }
                                prefab = model.GetAssetObject<GameObject>();
                                showModel = operation.Result;
                                foreach (var componentInChild in showModel.GetComponentsInChildren<Collider>())
                                {
                                    componentInChild.enabled = false;
                                }

                                foreach (var filter in showModel.GetComponentsInChildren<MeshFilter>())
                                {
                                    if (filter.TryGetComponent<MeshRenderer>(out var renderer))
                                    {
                                        var materials = new Material[filter.sharedMesh.subMeshCount];
                                        for (int i = 0; i < filter.sharedMesh.subMeshCount; i++)
                                        {
                                            materials[i] = _modelReference.Value.ShowModelMaterial;
                                        }
                                        renderer.materials = materials;
                                    }
                                }

                                {
                                    if (showModel.TryGetComponent<MeshRenderer>(out var renderer) && showModel.TryGetComponent<MeshFilter>(out var filter))
                                    {
                                        var materials = new Material[filter.sharedMesh.subMeshCount];
                                        for (int i = 0; i < filter.sharedMesh.subMeshCount; i++)
                                        {
                                            materials[i] = _modelReference.Value.ShowModelMaterial;
                                        }
                                        renderer.materials = materials;
                                    }
                                }
                                
                            };
                        });
                    }
                }
            }
            
            private void UpdateEyeRaycast()
            {
               
                var cTransform = CameraManager.GetCameraInstanceStatic<BuilderBaseCamera>().CameraObject.transform;
                /*
               _hitCount = Physics.RaycastNonAlloc(new Ray(cTransform.position,
                   cTransform.TransformDirection(Vector3.forward)), _hit,FindModularMaxDistance,ColliderLayer.BoundMask);

               float distance =   FindModularMaxDistance+1f;
               if (_hitCount > 0)
               {
                   // 由于RaycastNonAlloc返回的碰撞点不一定按距离排序，我们可以手动排序
                   System.Array.Sort(_hit, 0, _hitCount,new RaycastHitDistanceComparer());

                   RaycastHit result = _hit[0];
                   //_modelReference.Value.HitNormal = result.normal;
                   //_modelReference.Value.SelectedNode = result.collider.transform.parent.GetComponent<BaseModularNode>();
               }
               else
               {
                  // _modelReference.Value.HitNormal = Vector3.zero;
                  // _modelReference.Value.SelectedNode = null;
               }
               */
                
                var tuple = ECSUtils.Raycast(cTransform.position,cTransform.position +  cTransform.TransformDirection(Vector3.forward)* FindModularMaxDistance,new CollisionFilter()
                {
                    BelongsTo =  ColliderLayer.ShipModularSelfLayer,
                    CollidesWith = ColliderLayer.ShipModularSelfLayer
                });
                if (!tuple.HasValue)
                {
                    _modelReference.Value.SelectedNode = null;
                    _modelReference.Value.HitNormal = Vector3.zero;
                    _modelReference.Value.HitPoint = Vector3.zero;
                    return;
                }
                var _manager = ECSUtils.GetEntityManager();
                if (!_manager.HasValue)
                {
                    _modelReference.Value.SelectedNode = null;
                    _modelReference.Value.HitNormal = Vector3.zero;
                    _modelReference.Value.HitPoint = Vector3.zero;
                    return;
                }
                var manager = _manager.Value;
                if (tuple.Value.Item1 != Entity.Null)
                {
                    if (manager.HasComponent<ModularECSLinkerComponent>(tuple.Value.Item1) )
                    {
                        var linker = manager.GetComponentData<ModularECSLinkerComponent>(tuple.Value.Item1);
                        if (ShipManager.Instance.QueryModular(linker.ShipID,linker.ModuleID,out var modularNode))
                        {
                            _modelReference.Value.SelectedNode = modularNode;
                            _modelReference.Value.HitNormal = tuple.Value.Item2.SurfaceNormal;
                            _modelReference.Value.HitPoint = tuple.Value.Item2.Position;
                            return;
                        }
                    }
                }
                _modelReference.Value.SelectedNode = null;
                _modelReference.Value.HitNormal = Vector3.zero;
                _modelReference.Value.HitPoint = Vector3.zero;
            }
            
            private void UpdateRotation()
            {

                if (_modelReference.Value.IsInSnapMode)
                {
                    return;
                }
                float deltaAngle = 120 * Time.deltaTime;
                Vector3 rot = _modelReference.Value.EularAngleOfEditorGO;
                rot.x += InputManager.Instance.GetCurrentInputAction().BuildMode.RotateX.ReadValue<float>() * deltaAngle;
                rot.y += InputManager.Instance.GetCurrentInputAction().BuildMode.RotateY.ReadValue<float>() * deltaAngle;
                rot.z += InputManager.Instance.GetCurrentInputAction().BuildMode.RotateZ.ReadValue<float>() * deltaAngle;
                _modelReference.Value.EularAngleOfEditorGO = rot;
                
            }

            private void UpdatePlaceDistance()
            {
                placeDistance = Mathf.Clamp(placeDistance+InputManager.Instance.GetCurrentInputAction().BuildMode.ChangeModularPlaceDistance.ReadValue<float>() * PlaceDistanceChangeDelta, -1, 1);
            }

            private void RegisterAllRotationHandleInSnap()
            {
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateXTapNegative.performed += HandleRotationInSnapModeXNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateXTapPositive.performed += HandleRotationInSnapModeXPositive;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateYTapNegative.performed += HandleRotationInSnapModeYNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateYTapPositive.performed += HandleRotationInSnapModeYPositive;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateZTapNegative.performed += HandleRotationInSnapModeZNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateZTapPositive.performed += HandleRotationInSnapModeZPositive;
            }

            private void UnRegisterAllRotationHandleInSnap()
            {
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateXTapNegative.performed -= HandleRotationInSnapModeXNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateXTapPositive.performed -= HandleRotationInSnapModeXPositive;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateYTapNegative.performed -= HandleRotationInSnapModeYNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateYTapPositive.performed -= HandleRotationInSnapModeYPositive;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateZTapNegative.performed -= HandleRotationInSnapModeZNegative;
                InputManager.Instance.GetCurrentInputAction().BuildMode.RotateZTapPositive.performed -= HandleRotationInSnapModeZPositive;
            }

            private Vector3 ReSnapBuildModularRotation()
            {
                Vector3 rot = _modelReference.Value.EularAngleOfEditorGO;
                rot.x = Mathf.Round(rot.x / 90) * 90f;
                rot.y = Mathf.Round(rot.y / 90) * 90f;
                rot.z = Mathf.Round(rot.z / 90) * 90f;
                return rot;
            }
            
            private void HandleRotationInSnapModeXNegative(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.x -= 90;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }
            private void HandleRotationInSnapModeXPositive(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.x += 90;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }

            private void HandleRotationInSnapModeYNegative(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.y -= 90 ;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }
            private void HandleRotationInSnapModeYPositive(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.y += 90 ;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }
            private void HandleRotationInSnapModeZNegative(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.z -= 90;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }
            private void HandleRotationInSnapModeZPositive(InputAction.CallbackContext obj)
            {
                Vector3 rot = ReSnapBuildModularRotation();
                rot.z += 90;
                _modelReference.Value.EularAngleOfEditorGO  = rot;
            }

            private void HandleLeftClick(InputAction.CallbackContext obj)
            {
                if (!showModel || !prefab || !_modelReference.Value.SelectedNode)
                {
                    return;
                }
                var instance = GameObject.Instantiate<GameObject>(prefab,_modelReference.Value.SelectedNode.core.transform, true);
                if (!instance)
                    return;
                instance.transform.position = showModel.transform.position;
                instance.transform.rotation = Quaternion.Euler(_modelReference.Value.EularAngleOfEditorGO);
      
                _modelReference.Value.EditingShipCore.AddModularToShip(instance.GetComponent<BaseModularNode>(),(uint)_modelReference.Value.UseModelID);
                
                _modelReference.Value.EditingShipCore.ShipEcsPhysicLinker.RebuildPhysicsComponents();
                InputManager.BuilderActionCollector.Clear(InputBuildAction.LeftClick);
            }

            private void HandleSwitchSnapMode(InputAction.CallbackContext obj)
            {
                _modelReference.Value.IsInSnapMode = !_modelReference.Value.IsInSnapMode;
            }
            public override void OnGizmos()
            {
                if (_hitCount>0)
                {
                    Gizmos.DrawCube(_hit[0].point,Vector3.one*0.1f);
                    Gizmos.DrawRay(_hit[0].point,_dir);
                }
                base.OnGizmos();
            }

            private void ShowBuildGridPanel()
            {
                if (!IsExecuting)
                {
                    return;
                }
                if (!_modelReference.Value.IsInSnapMode)
                {
                    return;
                }
                if (hasGridPanelController || isLoadGridPanelController)
                {
                    return;
                }
                Asset.LoadResourceAsync<GameObject>("Assets/Assets/Base/Prefab/GridPlane.prefab", LoadResourcePriority.Default, model =>
                {
                    hasGridPanelController = true;
                    isLoadGridPanelController = true;
                    var op = model.InstantiateAsync();
                    op.Completed += (_) =>
                    {
                        hasGridPanelController = true;
                        _gridPanelController = op.Result.GetComponent<GridPanelController>();
                        _gridPanelController.Open(0.4f);
                        isLoadGridPanelController = false;
                    };
                });
            }

            private void CloseBuildGridPanel()
            {
                if (isLoadGridPanelController)
                {
                    return;
                }
                _gridPanelController?.Close(0.4f);
                _gridPanelController = null;
                hasGridPanelController = false;
            }

            private void ShowSelectedNodeFaceCenterAimCenter()
            {                                
                if (!IsExecuting || !_modelReference.Value.IsInSnapMode)
                {
                    return;
                }
                if (_selectModularFaceCenter)
                {
                    _selectModularFaceCenter.gameObject.SetActive(true);
                    return;
                }
                Asset.LoadResourceAsync<GameObject>("Assets/Assets/Base/Prefab/AimCenter.prefab", LoadResourcePriority.Default, model =>
                {
                    var op = model.InstantiateAsync();
                    op.Completed += (_) =>
                    {
                        _selectModularFaceCenter = op.Result.GetComponent<AimCenterController>();
                        _selectModularFaceCenter.Color = Color.yellow;
                    };
                });
            }

            private void CloseSelectedNodeFaceCenterAimCenter()
            {
                _selectModularFaceCenter?.gameObject?.SetActive(false);
            }
            private void ShowPlacePositionAimCenter()
            {
                if (!IsExecuting)
                {
                    return;
                }
                if (_placePositionController)
                {
                    _placePositionController.gameObject.SetActive(true);
                    return;
                }
                Asset.LoadResourceAsync<GameObject>("Assets/Assets/Base/Prefab/AimCenter.prefab", LoadResourcePriority.Default, model =>
                {
                    var op = model.InstantiateAsync();
                    op.Completed += (_) =>
                    {
                        _placePositionController = op.Result.GetComponent<AimCenterController>();
                        _placePositionController.Color = Color.green;
                    };
                });
            }

            private void ClosePlacePositionAimCenter()
            {
                _placePositionController?.gameObject.SetActive(false);
            }
        }
    }
}