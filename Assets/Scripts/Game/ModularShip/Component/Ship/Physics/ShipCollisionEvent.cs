using System.Numerics;
using Unity.Mathematics;

namespace Game
{
    public struct ShipCollisionEvent
    {
        public uint ShipID;
        public uint ModularID;
        
        public float3 CollisionPoint;
    }
}