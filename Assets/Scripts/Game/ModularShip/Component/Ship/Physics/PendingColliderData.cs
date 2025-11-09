using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Game
{
    public struct PendingColliderData : IBufferElementData
    {
        public Entity TargetEntity;
        public BoxGeometry Geometry;
        public RigidTransform Transform;
    }
}