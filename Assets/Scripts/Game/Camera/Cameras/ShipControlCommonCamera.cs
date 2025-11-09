using System;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class ShipControlCommonCamera : CinemachineExtension
    {
        [Tooltip("摄像机与目标的距离")]
        public float distance = 5.0f;
        
        [FoldoutGroup("横向范围")]
        public float minHorizontalAngle;
        [FoldoutGroup("横向范围")]
        public float maxHorizontalAngle;
        
        [FoldoutGroup("横向范围")]
        public float minVerticalAngle;
        [FoldoutGroup("横向范围")]
        public float maxVerticalAngle;

        public float currentHorizontalAngle { get; private set; }
        public float currentVerticalAngle  { get; private set; }
        
        [Tooltip("旋转平滑时间")]
        public float dampingTime = 0.2f;
        [ShowInInspector]
        private Vector3 m_DampingVelocity;
        private Quaternion m_LastTargetRotation;
        
        private Vector3 currentPosition;

        private Transform followCache;

        public void LateUpdate()
        {
        }
        public void Update()
        {
        }
        public override void PrePipelineMutateCameraStateCallback(CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime)
        {
            // 获取目标物体
            if (!vcam.Follow)
            {
                followCache = null;
                return;
            }
            var target = vcam.Follow;
            followCache = target.transform;
            // 计算摄像机在目标局部坐标系中的位置
            Vector3 localOffset = CalculateLocalOffset(target);
            // 将局部偏移转换为世界空间
            Vector3 worldOffset = target.rotation * localOffset;
           
            // 应用平滑阻尼
            if (Application.isPlaying && dampingTime > 0)
            {
                currentPosition = Vector3.SmoothDamp(currentPosition, worldOffset, ref m_DampingVelocity, dampingTime);
                curState.RawPosition = target.position +currentPosition;
            }
            else
            {
                curState.RawPosition = target.position + worldOffset;
            }
            // 计算目标旋转变化
            Quaternion targetRotationDelta = target.rotation * Quaternion.Inverse(m_LastTargetRotation);
            m_LastTargetRotation = target.rotation;
            // 设置摄像机朝向目标
            curState.RawOrientation = Quaternion.LookRotation(target.position - curState.RawPosition, target.up);
            base.PrePipelineMutateCameraStateCallback(vcam, ref curState, deltaTime);
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state,
            float deltaTime)
        {
        }

        // 计算摄像机在目标局部坐标系中的偏移
        private Vector3 CalculateLocalOffset(Transform targetTransform)
        {
            // 将角度转换为弧度
            float horizontalRad = currentHorizontalAngle * Mathf.Deg2Rad;
            float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

            // 计算局部坐标
            var result = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0) * (Vector3.back * distance);

            return result;
        }

        // 设置旋转角度（外部调用）
        public void SetRotationAngles(float horizontal, float vertical)
        {
            currentHorizontalAngle = Mathf.Clamp(horizontal, minHorizontalAngle, maxHorizontalAngle);
            currentVerticalAngle = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);
        }

        // 增加旋转角度（外部调用）
        public void AddRotationAngles(float horizontalDelta, float verticalDelta)
        {
            SetRotationAngles(
                currentHorizontalAngle + horizontalDelta,
                currentVerticalAngle + verticalDelta
            );
        }

        // 重置为默认角度
        public void ResetAngles()
        {
            currentHorizontalAngle = 0;
            currentVerticalAngle = 0;
        }
        [ShowInInspector]
        public void LookAt(Transform target)
        {
            LookAt(target.position);
        }

        public void LookAt(Vector3 targetPosition)
        {
            if (!followCache)
            {
                return;
            }
            var toward =targetPosition - followCache.position;
            var relativeToward = followCache.InverseTransformDirection(toward);
            var quaternion = Quaternion.LookRotation(relativeToward);
            SetRotationAngles(quaternion.eulerAngles.y.NormalizeAngle(),quaternion.eulerAngles.x.NormalizeAngle());
        }
    }
}