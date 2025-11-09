using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = UnityEngine.RaycastHit;

namespace Game
{
    public static class ECSUtils
    {
        public static Unity.Entities.EntityManager? GetEntityManager()
        {
            World defaultWorld = World.DefaultGameObjectInjectionWorld;
            if (defaultWorld == null || !defaultWorld.IsCreated)
            {
                Debug.LogError("Dots未正常加载");
                return null;
            }
            return defaultWorld.EntityManager;
        }
        
        public static (Entity, Unity.Physics.RaycastHit)? Raycast(float3 startPos, float3 endPos,CollisionFilter? filter = null) {
            
            var _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var _physicsWorldSingletonQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<PhysicsWorldSingleton>());
            // 1. 获取物理世界
            PhysicsWorld physicsWorld = _physicsWorldSingletonQuery.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;

            
            // 2. 设置射线参数
            RaycastInput input =new RaycastInput {
                Start = startPos,
                End = endPos,
                Filter =filter ?? new CollisionFilter { // 碰撞过滤器：设置可与哪些层碰撞
                    BelongsTo = ~0u, // 属于所有层
                    CollidesWith = ~0u, // 与所有层碰撞
                    GroupIndex = 0
                }
            };

            // 3. 发射射线
            if (collisionWorld.CastRay(input,out var hit)) {
                // 4. 通过命中的刚体索引获取对应的Entity
                if (_entityManager.HasComponent<PhysicsCollider>(hit.Entity))
                {
                    var collider = _entityManager.GetComponentData<PhysicsCollider>(hit.Entity);
                    if (collider.Value is { IsCreated: true, Value: { Type: ColliderType.Compound } } && collider.Value.Value.GetLeaf(hit.ColliderKey,out var leaf))
                    {
                       return (leaf.Entity,hit);
                    }
                    else
                    {
                        return (hit.Entity,hit);
                    }
                }
                return  (hit.Entity,hit);
            }
            return  null;
        }
    }
}