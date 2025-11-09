using ilsFramework.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class ShipControlBaseCamera : BaseCameraController<CinemachineVirtualCameraBase>
    {
        public override string CameraPrefabPath => "ShipControlCamera";
        public override bool IsInstance => true;

        private ShipCore targetShip;
        
        
        public float minHorizontalAngle = -40;
        public float maxHorizontalAngle = 40;
        
        public float minVerticalAngle = -40;
        public float maxVerticalAngle = 40;

        private ShipControlCommonCamera CameraController;
        
        public Vector3 CameraLookDirection => Camera.transform.forward;
        
        public Vector2 CameraLocalRotation => new Vector2(CameraController.currentHorizontalAngle, CameraController.currentVerticalAngle);
        
        public override void OnCameraInitialized()
        {
            CameraController = Camera.GetComponent<ShipControlCommonCamera>();
            base.OnCameraInitialized();
        }

        public override void OnUpdate(float deltaTime)
        {
            CameraController.minHorizontalAngle = minHorizontalAngle;
            CameraController.maxHorizontalAngle = maxHorizontalAngle;
            CameraController.minVerticalAngle = minVerticalAngle;
            CameraController.maxVerticalAngle = maxVerticalAngle;
            base.OnUpdate(deltaTime);
        }

        public override void OnLateUpdate()
        {
            UpdateCamera();
            base.OnLateUpdate();
        }
        
        void UpdateCamera()
        {
            if (!Enabled)
            {
                return;
            }
            targetShip = ShipManager.PlayerControlShip;
            if (targetShip)
            {
                Camera.Follow = targetShip.transform;
            }

            var look = InputManager.Instance.GetCurrentInputAction().Play.Look.ReadValue<Vector2>();
            CameraController.AddRotationAngles(look.x, look.y);
        }
    }
}