using System.Collections.Generic;
using Game.Ship;
using ilsFramework.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using Material = Unity.Physics.Material;

namespace Game
{
    [UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))] // 确保在ECB系统之前运行
    [UpdateBefore(typeof(SimulationSystemGroup))]
    public partial struct ShipColliderPreBuildSystem : ISystem
    {
        private EntityQuery _rebuildQuery;
        private BeginSimulationEntityCommandBufferSystem.Singleton _ecbSystemSingleton; 
        public void OnCreate(ref SystemState state)
        {
            _rebuildQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<ShipColliderBuildTag, TransformSync>()
                .Build(ref state);
        
            state.RequireForUpdate(_rebuildQuery);
            
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            // 通过该系统创建 ECB（延后命令）
            _ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = _ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged); 

            var entityManager = state.EntityManager;

            foreach (var (buildTag, transformSync, entity) in SystemAPI.Query<RefRW<ShipColliderBuildTag>, TransformSync>().WithEntityAccess())
            {
                if (!transformSync.ManagedTransform)
                    continue;

                if (!transformSync.ManagedTransform) 
                    continue;
                if (buildTag.ValueRO.IsRebuild)
                {
                    
                    PreCleanupOldPhysics(entityManager, entity, buildTag.ValueRO.ShipID,ecb);
                }
                var shipCore = transformSync.ManagedTransform.GetComponent<ShipCore>();
                buildTag.ValueRW.ColliderCounts = (uint)shipCore.ModularNodes.Count;
                ecb.RemoveComponent<ShipColliderBuildTag>(entity);
                BuildPhysicsCollider(ref state,shipCore, entity, ecb);
            }
        }
        private void PreCleanupOldPhysics(Unity.Entities.EntityManager entityManager, Entity entity,uint shipCoreID, EntityCommandBuffer ecb)
        {
            // 清理子碰撞体实体
            var childQuery = entityManager.CreateEntityQuery(typeof(ModularECSLinkerComponent));
            using (var childEntities = childQuery.ToEntityArray(Allocator.Temp))
            {
                foreach (var childEntity in childEntities)
                {
                    var link = entityManager.GetComponentData<ModularECSLinkerComponent>(childEntity);
                    // 根据你的条件判断，这里简化处理
                    if (shipCoreID == link.ShipID)
                    {
                        ecb.DestroyEntity(childEntity);
                    }
                }
            }
        }
         private void BuildPhysicsCollider(ref SystemState state,ShipCore shipCore, Entity entity, EntityCommandBuffer ecb)
         {
             ecb.AddBuffer<PendingColliderData>(entity);
             foreach (var moduleNode in shipCore.Graph.nodes)
             {
                 if (moduleNode.BaseNode)
                 {
                     foreach (var boxColliderShape in moduleNode.BaseNode.CollisonBounds)
                     {
                         CreateColliderBlobInstance(shipCore.ID,
                             boxColliderShape,
                             moduleNode.BaseNode,
                             ref state,
                             entity,
                             ecb);
                     }

                 }
             }
         }
    
    private void CreateColliderBlobInstance(uint shipID,BoxColliderShape boxColliderShape, BaseModularNode node, ref SystemState state, Entity parentEntity, EntityCommandBuffer ecb)
    {
        var modularNodeEntity = ecb.CreateEntity();
        // 添加必要的组件
        ecb.AddComponent(modularNodeEntity, new Parent { Value = parentEntity });
        ecb.AddComponent(modularNodeEntity, new ModularECSLinkerComponent() 
        { 
            ShipID = shipID,
            ModuleID = node.InstanceID
        });
        // 创建碰撞体几何
        var boxGeometry = new BoxGeometry
        {
            Center = boxColliderShape.center,
            Size = boxColliderShape.size,
            Orientation = boxColliderShape.rotation,
            BevelRadius = 0.01f
        };
        ecb.AppendToBuffer(parentEntity,new PendingColliderData()
        {
            TargetEntity = modularNodeEntity,
            Geometry = boxGeometry,
            Transform = new RigidTransform
            {
                pos = node.transform.localPosition,
                rot = node.transform.localRotation
            },
        });
    }
    }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))] // 移入SimulationSystemGroup
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]  // 确保在ECB播放之后运行
    [UpdateBefore(typeof(PrePhysicsCommandBufferSystem))] // 根据您的依赖关系调整，确保在物理系统之前
    public partial struct ShipColliderPostBuildSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var world = state.World;
            var prePhysicsEcbSystem = world.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            //if (prePhysicsEcbSystem == null)
             //   return; // 安全检查
            //var ecb = new EntityCommandBuffer(Allocator.Temp);
            // 通过该系统创建 ECB（延后命令）
            var ecb = prePhysicsEcbSystem.CreateCommandBuffer(); // NOTE: 不传 WorldUnmanaged；使用 Managed API
            
            foreach (var (buildTag, entity) in SystemAPI.Query<DynamicBuffer<PendingColliderData>>()
                         .WithEntityAccess())
            {
                var colliderInstances = new NativeList<CompoundCollider.ColliderBlobInstance>(
                    buildTag.Length, Allocator.Temp);

                foreach (var child in buildTag)
                {
                    if (state.EntityManager.Exists(child.TargetEntity))
                    {
                        var collider = BoxCollider.Create(child.Geometry, CollisionFilter.Default,
                            new Material()
                            {
                                CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents
                            });
                        
                        collider.Value.SetCollisionFilter(new CollisionFilter()
                        {
                            BelongsTo = ColliderLayer.ShipModularSelfLayer,
                            CollidesWith =ColliderLayer.ShipModularCollisionLayer
                        });
                        colliderInstances.Add(new CompoundCollider.ColliderBlobInstance
                        {
                            Collider = collider,
                            CompoundFromChild = child.Transform,
                            Entity = child.TargetEntity // 现在这是真实的实体引用
                        });
                    }
                }
                
                // 创建复合碰撞体
                if (colliderInstances.Length > 0)
                {
                    var compoundCollider = CompoundCollider.Create(colliderInstances.AsArray());

                    ecb.SetComponent(entity, new PhysicsCollider { Value = compoundCollider });
                }

                // 清空缓冲区
                ecb.RemoveComponent<PendingColliderData>(entity);
    
                colliderInstances.Dispose();
            }
        }
    }
}