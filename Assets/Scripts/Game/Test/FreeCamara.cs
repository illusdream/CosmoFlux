using ilsFramework.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
    public class FreeFlyCamera : MonoBehaviour
    {
        [Header("移动设置")] public float moveSpeed = 5.0f;
        public float sprintMultiplier = 2.0f;

        [Header("视角设置")] public float mouseSensitivity = 2.0f;
        public float verticalAngleLimit = 85.0f;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        void Start()
        {
            // 锁定鼠标到屏幕中心并隐藏
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 初始化旋转角度
            Vector3 rot = transform.localRotation.eulerAngles;
            rotationX = rot.x;
            rotationY = rot.y;
        }

        void Update()
        {
            // 处理视角旋转
            HandleRotation();

            // 处理移动
            HandleMovement();

            // 按ESC键解锁鼠标
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // 按鼠标左键重新锁定鼠标
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void HandleRotation()
        {
            // 获取鼠标输入
            rotationY += UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationX -= UnityEngine.Input.GetAxis("Mouse Y") * mouseSensitivity;

            // 限制垂直角度
            rotationX = Mathf.Clamp(rotationX, -verticalAngleLimit, verticalAngleLimit);

            // 应用旋转
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }

        void HandleMovement()
        {
            // 获取键盘输入
            float horizontal = UnityEngine.Input.GetAxis("Horizontal");
            float vertical = UnityEngine.Input.GetAxis("Vertical");
            float upDown = 0.0f;

            // 处理上升/下降
            if (UnityEngine.Input.GetKey(KeyCode.Space))
                upDown = 1.0f;
            else if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
                upDown = -1.0f;

            // 计算移动方向
            Vector3 moveDirection = new Vector3(horizontal, upDown, vertical);

            // 是否加速
            float currentSpeed = moveSpeed;
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                currentSpeed *= sprintMultiplier;

            // 根据相机朝向转换方向
            moveDirection = transform.TransformDirection(moveDirection);

            // 应用移动
            transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
    }
}