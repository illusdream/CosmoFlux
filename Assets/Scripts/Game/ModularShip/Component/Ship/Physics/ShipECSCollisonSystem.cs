using ilsFramework.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Systems;
using UnityEngine;

namespace Game
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateAfter(typeof(PhysicsDebugDisplayGroup))]
    public partial struct ShipECSCollisonSystem : ISystem
    {
        private NativeQueue<ShipCollisionEvent> _collisionQueue;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<SimulationSingleton>();
            _collisionQueue = new NativeQueue<ShipCollisionEvent>(Allocator.Persistent);
        }

        
        public void OnUpdate(ref SystemState state) {
            state.Dependency = new HealthJob
            {
                ColliderLookup = SystemAPI.GetComponentLookup<PhysicsCollider>(),
                LinkerLookup = SystemAPI.GetComponentLookup<ModularECSLinkerComponent>(),
                CollisionQueue = _collisionQueue.AsParallelWriter(),
                PhysicsWorld =SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld
                // 传递所需数据
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
            
            state.Dependency.Complete();

            while (_collisionQueue.TryDequeue(out var collisionEvent))
            {
                if (ShipManager.Instance.QueryModular(collisionEvent.ShipID,collisionEvent.ModularID,out var modularNode))
                {
                    modularNode.InvokeCollision(collisionEvent);
                }
            }
            _collisionQueue.Clear();
        }
    }
    //[BurstCompile]
    public partial struct HealthJob  : ICollisionEventsJob
    {
        public ComponentLookup<PhysicsCollider> ColliderLookup;
        public ComponentLookup<ModularECSLinkerComponent> LinkerLookup;
        public NativeQueue<ShipCollisionEvent>.ParallelWriter CollisionQueue;
        [ReadOnly]public PhysicsWorld PhysicsWorld;
        public void Execute(CollisionEvent collisionEvent)
        {
            
            _Debug(collisionEvent.EntityA,collisionEvent.ColliderKeyA,collisionEvent);
            _Debug(collisionEvent.EntityB,collisionEvent.ColliderKeyB,collisionEvent);
            
            // 检查它是否是复合碰撞体，并尝试通过 key 获取子碰撞器信息
          
            //.LogSelf();
        }

        private void _Debug(Entity entity,ColliderKey keyA,CollisionEvent collisionEvent)
        {
            if (ColliderLookup.HasComponent(entity))
            {
                var collider = ColliderLookup[entity];
                if (collider.Value.Value.Type == ColliderType.Compound)
                {
                    if (collider.Value.Value.GetLeaf(keyA,out var leaf) &&LinkerLookup.HasComponent(leaf.Entity))
                    {
                       var modular = LinkerLookup[leaf.Entity];
                       CollisionQueue.Enqueue(new ShipCollisionEvent()
                       {
                           ShipID = modular.ShipID,
                           ModularID = modular.ModuleID,
                           CollisionPoint = collisionEvent.CalculateDetails(ref PhysicsWorld).AverageContactPointPosition,
                       });
                    }
                }
            }
        }
    }
}