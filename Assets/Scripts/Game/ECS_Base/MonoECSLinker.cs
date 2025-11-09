using System;
using System.Collections.Generic;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using Collider = UnityEngine.Collider;

namespace Game
{
    /// <summary>
    /// 用于将ECS与Mono组件联合在一起，以及提供
    /// </summary>
    public class MonoECSLinker : MonoBehaviour
    {
        public Entity physicsEntity { get; private set; }
        private Unity.Entities.EntityManager entityManager;
        [SerializeReference]
        public List<ECSLinkerComponet> IECSLinkers = new List<ECSLinkerComponet>();
        
        public void CreateLinkedEntity()
        {
            World defaultWorld = World.DefaultGameObjectInjectionWorld;
            if (defaultWorld == null || !defaultWorld.IsCreated)
            {
                Debug.LogError("Dots未正常加载");
                this.enabled = false;
                return;
            }
            entityManager = defaultWorld.EntityManager;
             
            CreatePhysicsEntity();
        }

        private void CreatePhysicsEntity()
        {
            var list = new List<ComponentType>()
            {
                typeof(LocalToWorld),
                typeof(LocalTransform),
                typeof(TransformSync), // 自定义同步组件
            };

            foreach (var linker in IECSLinkers)
            {
                linker.EntityManager = entityManager;
                linker.OnCreate();
                linker.OnSetArchetype(list);
            }
            NativeArray<ComponentType> array = new NativeArray<ComponentType>(list.ToArray(),Allocator.Temp);
            EntityArchetype archetype = entityManager.CreateArchetype(array);
            // 使用 EntityArchetype 创建实体


            physicsEntity = entityManager.CreateEntity(archetype);

#if UNITY_EDITOR
            // 在编辑器中为实体命名，方便调试
            entityManager.SetName(physicsEntity, $"MonoGameObject:{gameObject.name}");
#endif
            LocalTransform newTransform = new LocalTransform
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = 1.0f
            };

            entityManager.SetComponentData<LocalTransform>(physicsEntity, newTransform);
          
            // 关键：将 Transform 实例关联到实体上
            entityManager.AddComponentData(physicsEntity, new TransformSync { ManagedTransform = this.transform });

            foreach (var linker in IECSLinkers)
            {
                linker.ECSLinkEntity = physicsEntity;
                linker.OnSetComponentData(entityManager, physicsEntity);
            }
        }

        public void DestroyLinkedEntity()
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
                            foreach (var linker in IECSLinkers)
                            {
                                linker.OnMonoDestroy(manager, entity);
                            }
                        }
                    }
                }
            }

        }

        public void Reset()
        {
           ResearchAllLinkedComponents();
        }
        [ShowInInspector]
        public void ResearchAllLinkedComponents()
        {
            IECSLinkers ??= new List<ECSLinkerComponet>();
            IECSLinkers.Clear();
            //IECSLinkers.AddRange( transform.GetComponentsInChildren<IECSLinker>());;
            IECSLinkers.AddRange( transform.GetComponents<ECSLinkerComponet>());;
        }
    }
}