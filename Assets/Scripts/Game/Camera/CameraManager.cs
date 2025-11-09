using System;
using System.Collections;
using System.Collections.Generic;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class CameraManager : ManagerSingleton<CameraManager>
    {
        public MainBaseCameraController MainBaseCameraController { get; private set; }
        
        [ShowInInspector]
        public BaseCameraController CurrentCameraController { get; private set; }
        
        public const string CAMERA_PREFAB_PATH = "Assets/Assets/Base/Camera/{0}.prefab";

        public int CameraIndexBuffer =0;
        [ShowInInspector]
        public Dictionary<int, BaseCameraController> Cameras = new Dictionary<int, BaseCameraController>();
        [ShowInInspector]
        public Dictionary<Type,BaseCameraController> InstanceCameras = new Dictionary<Type, BaseCameraController>();
        
        public Dictionary<ICinemachineCamera, BaseCameraController> CinemachinesMap = new Dictionary<ICinemachineCamera, BaseCameraController>();
        
        public override IEnumerator OnInit()
        {
            MainBaseCameraController = new MainBaseCameraController();
            var op = Asset.LoadResourceAsync<GameObject>("Base", string.Format(CAMERA_PREFAB_PATH, MainBaseCameraController.CameraPrefabPath),
                LoadResourcePriority.Camera);
            yield return op;
            var handle = op.GetResult();
            var instance = GameObject.Instantiate(handle.GetAssetObject<GameObject>(),Vector3.zero,Quaternion.identity);
            GameObject.DontDestroyOnLoad(instance);
            MainBaseCameraController.Initialize(null,instance,CameraIndexBuffer);
            MainBaseCameraController.SetMainCamera(instance.GetComponent<Camera>(),instance.GetComponent<CinemachineBrain>());
            CameraIndexBuffer++;
            InstanceCameras[typeof(MainBaseCameraController)] = MainBaseCameraController;
            handle.Release();
            yield return null;
        }

        public override void OnUpdate()
        {
            if (MainBaseCameraController?.Brain?.ActiveVirtualCamera != null && CinemachinesMap.TryGetValue(MainBaseCameraController.Brain.ActiveVirtualCamera,out var camera))
            {
                CurrentCameraController = camera;
            }

            if (MainBaseCameraController is not null &&MainBaseCameraController.Enabled) MainBaseCameraController.OnUpdate(Time.deltaTime);
            Cameras.Values.ForEach((cc) => {if(cc.Enabled) cc.OnUpdate(Time.deltaTime);});
        }

        public override void OnLateUpdate()
        {
            if (MainBaseCameraController is not null &&MainBaseCameraController.Enabled) MainBaseCameraController.OnLateUpdate();
            Cameras.Values.ForEach((cc) => {if(cc.Enabled) cc.OnLateUpdate();});
        }

        public override void OnLogicUpdate()
        {
            if (MainBaseCameraController is not null &&MainBaseCameraController.Enabled) MainBaseCameraController.OnLogicUpdate();
            Cameras.Values.ForEach((cc) => {if(cc.Enabled) cc.OnLogicUpdate();});
        }

        public override void OnFixedUpdate()
        {
            if (MainBaseCameraController is not null &&MainBaseCameraController.Enabled) MainBaseCameraController.OnFixedUpdate();
            Cameras.Values.ForEach((cc) => {if(cc.Enabled) cc.OnFixedUpdate();});
        }

        public override void OnDestroy()
        {
            MainBaseCameraController.OnCameraDestroy();
            foreach (var cc in Cameras.Values)
                cc.OnCameraDestroy();
        }

        public override void OnDrawGizmos()
        {
            MainBaseCameraController.OnGizmos();
            foreach (var cc in Cameras.Values)
                cc.OnGizmos();
        }

        public override void OnDrawGizmosSelected()
        {
           
        }

        public void RegisterCamera<T>() where T : BaseCameraController
        {
            var instance = Activator.CreateInstance<T>();
            Asset.LoadResourceAsync<GameObject>("Base", string.Format(CAMERA_PREFAB_PATH, instance.CameraPrefabPath),
                LoadResourcePriority.Camera,(assetHandle =>
                {
                    var go = GameObject.Instantiate(assetHandle.GetAssetObject<GameObject>(),Vector3.zero,Quaternion.identity);
                    GameObject.DontDestroyOnLoad(go);
                    instance.Initialize(go.GetComponent<CinemachineVirtualCameraBase>(),go,CameraIndexBuffer);
                    Cameras[CameraIndexBuffer] = instance;
                    CameraIndexBuffer++;
                    assetHandle.Release();
                    if (instance.IsInstance)
                    {
                        InstanceCameras[typeof(T)] = instance;
                    }
                    CinemachinesMap[instance.VirtualCamera] = instance;
                }));
        }

        public void UnRegisterCamera(int ID)
        {
            if (Cameras.TryGetValue(ID, out var camera))
            {
                camera.OnCameraDestroy();
                GameObject.Destroy(camera.CameraObject);
                Cameras.Remove(ID);
            }
        }

        public T GetCameraInstance<T>() where T : BaseCameraController
        {
            return InstanceCameras[typeof(T)] as T;
        }

        public static T GetCameraInstanceStatic<T>() where T : BaseCameraController
        {
            return Instance.GetCameraInstance<T>();
        }

        public void SetAsMainCamera(BaseCameraController baseCamera)
        {
            CurrentCameraController = baseCamera;
            foreach (var camerasValue in Cameras.Values)
            {
                if (baseCamera == camerasValue)
                {
                    camerasValue.VirtualCamera.Priority = 1;
                }
                else
                {
                    camerasValue.VirtualCamera.Priority = 0;
                }
            }
        }
    }
}