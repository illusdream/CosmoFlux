using Unity.Entities;

namespace Game
{
    public struct ModularECSLinkerComponent : IComponentData
    {
        public uint ShipID;
        public uint ModuleID;
    }
}