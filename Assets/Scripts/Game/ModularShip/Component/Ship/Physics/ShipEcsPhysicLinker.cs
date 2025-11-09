using System.Collections.Generic;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Material = Unity.Physics.Material;

namespace Game.Ship
{
    public class ShipEcsPhysicLinker : ECSLinkerComponet
    {
        public Unity.Entities.EntityManager EntityManager { get; set; }
        public Entity ECSLinkEntity { get; set; }

        public ShipCore ShipCore;

        public override void OnCreate()
        {
            
        }

        public override void OnSetArchetype(List<ComponentType> types)
        {
            types.Add(typeof(PhysicsCollider));
            types.Add(typeof(PhysicsVelocity));
            types.Add(typeof(PhysicsMass));
            types.Add(typeof(PhysicsWorldIndex));
            types.Add(typeof(PhysicsGravityFactor));
        }

        public override void OnSetComponentData(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            entityManager.SetComponentData(entity, new PhysicsGravityFactor() { Value = 0.0f });

            BuildPhysicsCollider(entityManager, entity);
        }
        

        public override void OnMonoDestroy(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            // 确保在销毁时清理 Entity 和关联资源
            if (EntityManager.World.IsCreated && EntityManager.Exists(ECSLinkEntity))
            {
                // 释放碰撞体 BlobAsset 资源
                if (EntityManager.HasComponent<PhysicsCollider>(ECSLinkEntity))
                {
                    DestroyColliderBindingEntity(entityManager,entity);
                    var collider = EntityManager.GetComponentData<PhysicsCollider>(ECSLinkEntity);
                    if(collider.Value.IsCreated)
                    {
                        collider.Value.Dispose();
                    }
                }
                EntityManager.DestroyEntity(ECSLinkEntity);
            }
        }
        
        private CompoundCollider.ColliderBlobInstance CreateColliderBlobInstance(BoxColliderShape boxColliderShape,BaseModularNode node,Unity.Entities.EntityManager manager,Entity parentEntity)
        {
            var ModularNodeEntity = manager.CreateEntity();

            manager.AddComponentData(ModularNodeEntity, new Parent() { Value = parentEntity });
            manager.AddComponentData(ModularNodeEntity,new ModularECSLinkerComponent() { ShipID = ShipCore.ID,ModuleID = node.InstanceID});
            // 创建子碰撞体（这里以BoxCollider为例）
            var boxGeometry = new BoxGeometry
            {
                Center = boxColliderShape.center,
                Size = boxColliderShape.size,
                Orientation = boxColliderShape.rotation,
                BevelRadius = 0.01f
            };
        
            var collider = Unity.Physics.BoxCollider.Create(boxGeometry,CollisionFilter.Default,
                new Material() { CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents,
            });
            collider.Value.SetCollisionFilter(new CollisionFilter()
            {
                BelongsTo = ColliderLayer.ShipModularSelfLayer,
                CollidesWith =ColliderLayer.ShipModularCollisionLayer
            });
            // 创建碰撞体实例
            return new CompoundCollider.ColliderBlobInstance
            {
                Collider = collider,
                CompoundFromChild = new RigidTransform
                {
                    pos = node.transform.localPosition,
                    rot = node.transform.localRotation
                },
                Entity = ModularNodeEntity
            };
        }

        private void BuildPhysicsCollider(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            List<CompoundCollider.ColliderBlobInstance> colliders = new List<CompoundCollider.ColliderBlobInstance>();
            foreach (var graphNode in ShipCore.Graph.nodes)
            {
                if (graphNode.BaseNode)
                {
                    var collider = CreateColliderBlobInstance(graphNode.BaseNode.BuildHelperBound, graphNode.BaseNode, entityManager, entity);

                    colliders.Add( collider);
                }
            }

            using (var children = new NativeArray<CompoundCollider.ColliderBlobInstance>(colliders.ToArray(), Allocator.Temp))
            {
                
                var compoundCollider = CompoundCollider.Create(children);
                entityManager.SetComponentData(entity, new PhysicsCollider
                {
                    Value = compoundCollider
                });
                entityManager.SetComponentData(entity, PhysicsMass.CreateDynamic(
                        mass: 10f
                        , massProperties:compoundCollider.Value.MassProperties) // 给定一个质量 kg
                );
            }
        }

        private void DestroyColliderBindingEntity(Unity.Entities.EntityManager entityManager, Entity entity)
        {
            if (entityManager.HasComponent<PhysicsCollider>(entity))
            {
                EntityQuery childDeleteQuery = entityManager.CreateEntityQuery(typeof(ModularECSLinkerComponent));

                using (var childDeleteEntities = childDeleteQuery.ToEntityArray(Allocator.TempJob))
                {
                    foreach (var childDelete in childDeleteEntities)
                    {
                        if (entityManager.HasComponent<ModularECSLinkerComponent>(childDelete))
                        {
                            if (entityManager.GetComponentData<ModularECSLinkerComponent>(childDelete).ShipID == ShipCore.ID) // 您的判断条件
                            {
                                entityManager.DestroyEntity(childDelete);
                            }
                        }
                    }
                }
            }
        }

        [ShowInInspector]
        public void RebuildPhysicsComponents()
        {
            var _manager = ECSUtils.GetEntityManager();
            if (!_manager.HasValue)
                return;
            var manager = _manager.Value;
            
            EntityQueryDesc queryDesc = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TransformSync) }
            };

            // 创建 EntityQuery
            using (var query = manager.CreateEntityQuery(queryDesc))
            {
                using (var entities = query.ToEntityArray(Allocator.Temp))
                {
                    foreach (var entity in entities)
                    {
                        TransformSync transformSync = manager.GetComponentData<TransformSync>(entity);
                        if (transformSync.ManagedTransform == this.transform)
                        {
                            manager.AddComponentData(entity, new ShipColliderBuildTag()
                            {
                                ShipID = ShipCore.ID,
                                IsRebuild = true
                            });
                        }
                    }
                }
            }
        }
    }
}