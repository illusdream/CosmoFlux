using System.Collections.Generic;
using Unity.Entities;

namespace Game
{
    public struct ShipCoreECSComponent : IComponentData
    {
        public uint ShipID;
    }
    
    public class ShipCoreECSLinkerComponent : ECSLinkerComponet
    {
        public ShipCore Ship;
        public override void OnCreate()
        {
            
        }

        public override void OnSetArchetype(List<ComponentType> types)
        {
            types.Add(typeof(ShipCoreECSComponent));
        }

        public override void OnSetComponentData(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            entityManager.SetComponentData(entity,new ShipCoreECSComponent() {ShipID = Ship.ID});
        }

        public override void OnMonoDestroy(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            
        }
    }
}