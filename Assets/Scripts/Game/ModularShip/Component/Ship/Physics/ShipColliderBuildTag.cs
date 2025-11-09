using Unity.Entities;

namespace Game
{
    public struct ShipColliderBuildTag : IComponentData
    {
        public bool IsRebuild;
        public uint ShipID;
        public uint ColliderCounts;
    }

    public struct ShipModularEntityDestoryTag : IComponentData
    {
        
    }
}