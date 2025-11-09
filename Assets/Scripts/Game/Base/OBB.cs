using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public struct OBB
    {
        public Vector3 center;
        public Vector3 size;
        public Vector3[] axes;

        /// <summary>
        /// 从BoxCollider上获取OBB
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static OBB CalculateOBBFromBoxCollider(BoxCollider collider)
        {
            OBB obb = new OBB();

            // 获取BoxCollider的中心和大小（本地坐标）
            Vector3 center = collider.center;
            Vector3 size = collider.size;

            // 考虑物体的缩放
            Vector3 lossyScale = collider.transform.lossyScale;
            size = Vector3.Scale(size, lossyScale);

            // 获取物体的旋转
            Quaternion rotation = collider.transform.rotation;

            // 计算OBB的中心（世界坐标）
            obb.center = collider.transform.TransformPoint(center);

            // 计算OBB的三个轴方向（世界坐标）
            obb.axes = new Vector3[3];
            obb.axes[0] = rotation * Vector3.right; // X轴
            obb.axes[1] = rotation * Vector3.up; // Y轴
            obb.axes[2] = rotation * Vector3.forward; // Z轴

            // OBB的大小就是BoxCollider的大小（考虑缩放）
            obb.size = size;

            return obb;
        }

        public static OBB CalculateOBBFromBoxColliderShape(BoxColliderShape box,Transform transform)
        {
            OBB obb = new OBB();

            // 获取BoxCollider的中心和大小（本地坐标）
            Vector3 center = box.center;
            Vector3 size = box.size;

            // 考虑物体的缩放
            Vector3 lossyScale = transform.lossyScale;
            size = Vector3.Scale(size, lossyScale);

            // 获取物体的旋转
            Quaternion rotation = transform.rotation;

            // 计算OBB的中心（世界坐标）
            obb.center = transform.TransformPoint(center);

            // 计算OBB的三个轴方向（世界坐标）
            obb.axes = new Vector3[3];
            obb.axes[0] = rotation * Vector3.right; // X轴
            obb.axes[1] = rotation * Vector3.up; // Y轴
            obb.axes[2] = rotation * Vector3.forward; // Z轴

            // OBB的大小就是BoxCollider的大小（考虑缩放）
            obb.size = size;

            return obb;
        }

        public static float GetDistanceToEdgeInDirection(OBB obb, Vector3 direction)
        {
            // 确保方向是单位向量
            direction.Normalize();

            // 计算方向向量在OBB三个轴上的投影长度
            float projX = Mathf.Abs(Vector3.Dot(direction, obb.axes[0]));
            float projY = Mathf.Abs(Vector3.Dot(direction, obb.axes[1]));
            float projZ = Mathf.Abs(Vector3.Dot(direction, obb.axes[2]));

            // 计算在指定方向上OBB的半长
            float halfLength =
                (projX * obb.size.x * 0.5f) +
                (projY * obb.size.y * 0.5f) +
                (projZ * obb.size.z * 0.5f);

            return halfLength;
        }

        public Vector3 GetNearestFaceCenter(Vector3 point)
        {
            // 计算 OBB 的半尺寸
            Vector3 halfSize = size * 0.5f;

            // 将点转换到 OBB 局部坐标系
            Vector3 localPoint = point - center;
            float px = Vector3.Dot(localPoint, axes[0]);
            float py = Vector3.Dot(localPoint, axes[1]);
            float pz = Vector3.Dot(localPoint, axes[2]);

            // 计算点到各个面的距离（带符号）
            float[] distances = new float[6];

            // X 轴两个面
            distances[0] = px - halfSize.x; // +X 面（距离为负表示在 OBB 内部）
            distances[1] = -px - halfSize.x; // -X 面

            // Y 轴两个面
            distances[2] = py - halfSize.y; // +Y 面
            distances[3] = -py - halfSize.y; // -Y 面

            // Z 轴两个面
            distances[4] = pz - halfSize.z; // +Z 面
            distances[5] = -pz - halfSize.z; // -Z 面

            // 找到距离最近的面（绝对值最小的距离）
            int minIndex = 0;
            float minDistance = Mathf.Abs(distances[0]);

            for (int i = 1; i < 6; i++)
            {
                float absDist = Mathf.Abs(distances[i]);
                if (absDist < minDistance)
                {
                    minDistance = absDist;
                    minIndex = i;
                }
            }

            // 根据找到的面索引返回对应的中心点
            switch (minIndex)
            {
                case 0: // +X 面
                    return center + axes[0] * halfSize.x;
                case 1: // -X 面
                    return center - axes[0] * halfSize.x;
                case 2: // +Y 面
                    return center + axes[1] * halfSize.y;
                case 3: // -Y 面
                    return center - axes[1] * halfSize.y;
                case 4: // +Z 面
                    return center + axes[2] * halfSize.z;
                case 5: // -Z 面
                    return center - axes[2] * halfSize.z;
                default:
                    return center; // 理论上不会执行
            }
        }

        public override string ToString()
        {
            return $"center: {center}, size: {size}, axes: {string.Join(", ", axes)}";
        }
    }
}