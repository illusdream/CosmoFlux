using System;
using System.Collections.Generic;
using System.Linq;
using Game.Input;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Game.Ship
{
    public class ShipMover : MonoBehaviour
    {
        public Rigidbody Rigidbody;
        
        public ShipCore ship;

        public const float MinForwardSpeed = 0.2f;
        
        public const float MinBackwardSpeed = 0.1f;

        public Vector3 AxialThrust;
        
        public PlayActionCollector PlayActionCollector;
        
        public MonoECSLinker MonoECSLinker;
        public void Awake()
        {
            ship.AfterInitializeShipCore += AfterInitializeShipCore;
        }

        private void AfterInitializeShipCore(ShipCore obj)
        {
            //计算推力
            CalculateStatus(obj.Modulars[EModularType.Engine].Cast<ModularEngine>());
        }

        public void Start()
        {
            
        }

        private void CalculateStatus(IEnumerable<ModularEngine> modularEngines)
        {

        }

        public void FixedUpdate()
        {
            Rigidbody.ResetCenterOfMass();
            if (PlayActionCollector.CheckCurrent<ValueCommend<Vector2>>(InputPlayAction.Move,out var result))
            {
                Rigidbody.AddForce(CameraManager.Instance.GetCameraInstance<ShipControlBaseCamera>().CameraObject.transform.forward * result.Value.y,ForceMode.Force);
                Rigidbody.AddTorque(transform.forward *result.Value.x, ForceMode.Force);
            }
            
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawSphere(Rigidbody.worldCenterOfMass, 0.1f);
        }
        [ShowInInspector]
        public void Test(Vector3 velocity, Vector3 augleVelocity)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (!entityManager.Exists(MonoECSLinker.physicsEntity)) return;

            var command = new SetVelocityCommand
            {
                Linear = velocity,
                Angular = augleVelocity
            };

            // 如果实体已有VelocityCommand，则修改它；否则添加一个新组件
            if (entityManager.HasComponent<SetVelocityCommand>(MonoECSLinker.physicsEntity))
            {
                entityManager.SetComponentData(MonoECSLinker.physicsEntity, command);
            }
            else
            {
                entityManager.AddComponentData(MonoECSLinker.physicsEntity, command);
            }
        }
    }
}