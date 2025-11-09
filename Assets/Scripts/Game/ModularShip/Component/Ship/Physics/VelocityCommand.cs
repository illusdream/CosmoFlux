using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    public struct AddLinearVelocityCommand : IComponentData
    {
        public float3 Value;
    }

    public struct AddAngleVelocityCommand : IComponentData
    {
        public float3 Value;
    }
    
    public struct SetVelocityCommand : IComponentData
    {
        public float3 Linear; // 线性速度命令
        public float3 Angular; // 角速度命令
    }
}