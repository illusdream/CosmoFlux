using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Game
{
    [BurstCompile]
    [UpdateBefore(typeof(SimulationSystemGroup))]
    public partial struct VelocityCommandSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb_SetVelocity = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (velocityCommand, physicsVelocity, entity) in SystemAPI.Query<RefRO<SetVelocityCommand>, RefRW<PhysicsVelocity>>().WithEntityAccess())
            {
                physicsVelocity.ValueRW.Linear = velocityCommand.ValueRO.Linear;
                physicsVelocity.ValueRW.Angular = velocityCommand.ValueRO.Angular;
                
                ecb_SetVelocity.RemoveComponent<SetVelocityCommand>(entity);
            }
            
            var ecb_AddLinear = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (velocityCommand, physicsVelocity, entity) in SystemAPI.Query<RefRO<AddLinearVelocityCommand>, RefRW<PhysicsVelocity>>().WithEntityAccess())
            {
                physicsVelocity.ValueRW.Linear = velocityCommand.ValueRO.Value;
                
                ecb_AddLinear.RemoveComponent<SetVelocityCommand>(entity);
            }
            
            var ecb_AddAngle = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (velocityCommand, physicsVelocity, entity) in SystemAPI.Query<RefRO<AddAngleVelocityCommand>, RefRW<PhysicsVelocity>>().WithEntityAccess())
            {
                physicsVelocity.ValueRW.Angular = velocityCommand.ValueRO.Value;
                
                ecb_AddAngle.RemoveComponent<SetVelocityCommand>(entity);
            }
        }
    }
}