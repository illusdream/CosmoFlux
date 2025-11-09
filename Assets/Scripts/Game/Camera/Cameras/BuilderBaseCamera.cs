using UnityEngine;

namespace Game
{
    public class BuilderBaseCamera : BaseCameraController
    {
        public float moveSpeed = 5.0f;
        public float sprintMultiplier = 2.0f;
        
        public float verticalAngleLimit = 85.0f;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        public override string CameraPrefabPath => "BuilderCamera";
        public override bool IsInstance => true;

        public bool CanTranslation = true;
        
        public Vector2 灵敏度 = new Vector2(0.3f, 0.3f);

        public override void OnCameraInitialized()
        {
            // 锁定鼠标到屏幕中心并隐藏
           // Cursor.lockState = CursorLockMode.Locked;
          //  Cursor.visible = false;

            // 初始化旋转角度
            Vector3 rot = CameraObject.transform.localRotation.eulerAngles;
            rotationX = rot.x;
            rotationY = rot.y;
            base.OnCameraInitialized();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (CameraManager.Instance.MainBaseCameraController.Brain.IsBlending)
            {
                return;
            }
            if (IsMainCamera)
            {
                if (CanTranslation)
                {
                    // 处理视角旋转
                    HandleRotation();

                    // 处理移动
                    HandleMovement();
                }
            }
            else
            {
                CameraObject.transform.position = CameraManager.Instance.CurrentCameraController.VirtualCamera.transform.position;
                CameraObject.transform.LookAt(CameraManager.Instance.CurrentCameraController.VirtualCamera.LookAt);
                rotationX = CameraObject.transform.rotation.eulerAngles.x;
                rotationY = CameraObject.transform.rotation.eulerAngles.y;
            }


           
            base.OnUpdate(deltaTime);
        }
        
        void HandleRotation()
        {
            var look = InputManager.Instance.GetCurrentInputAction().BuildMode.Look.ReadValue<Vector2>();
            // 获取鼠标输入
            rotationY +=look.x * 灵敏度.x;
            rotationX -= look.y * 灵敏度.y;

            // 限制垂直角度
            rotationX = Mathf.Clamp(rotationX, -verticalAngleLimit, verticalAngleLimit);

            // 应用旋转
            CameraObject.transform.rotation = ShipManager.BuildTargetShip.transform.rotation *  Quaternion.Euler(rotationX, rotationY, 0) ;
        }

        void HandleMovement()
        {
            // 获取键盘输入
            var move = InputManager.Instance.GetCurrentInputAction().BuildMode.Move.ReadValue<Vector3>();
            // 是否加速
            float currentSpeed = moveSpeed;
            if (InputManager.Instance.GetCurrentInputAction().BuildMode.QuickChange.IsPressed())
                currentSpeed *= sprintMultiplier;

            // 根据相机朝向转换方向
            Vector3 moveDirection = CameraObject.transform.TransformDirection(move);

            // 应用移动
            CameraObject.transform.position += moveDirection * (currentSpeed * Time.deltaTime);
        }

        public void Reset()
        {
            rotationX = 0.0f;
            rotationY = 0.0f;
        }
    }
}