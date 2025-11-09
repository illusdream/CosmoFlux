using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public abstract class BaseCameraController
    {
        public abstract string CameraPrefabPath { get; }
        
        public abstract bool IsInstance { get; }
        
        public CinemachineVirtualCameraBase VirtualCamera {get; private set;}
        public GameObject CameraObject {get; private set;}
        
        public int ID {get; private set;}

        public bool IsMainCamera => VirtualCamera.Priority == 1;

        public bool Enabled = true;

        public void Initialize(CinemachineVirtualCameraBase camera,GameObject gameObject,int id)
        {
            VirtualCamera = camera;
            CameraObject = gameObject;
            ID = id;
            OnCameraInitialized();
        }

        public virtual void OnCameraInitialized()
        {
            
        }

        public virtual void OnUpdate(float deltaTime)
        {
            
        }

        public virtual void OnLogicUpdate()
        {
            
        }

        public virtual void OnLateUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnCameraDestroy()
        {
            
        }

        public virtual void OnGizmos()
        {
        }
    }

    public abstract class BaseCameraController<T> : BaseCameraController where T : CinemachineVirtualCameraBase
    {
        public T Camera => VirtualCamera as T;
    }
}