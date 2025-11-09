namespace Game
{
    using Unity.Burst;
    using Unity.Entities;
    using Unity.Physics.Systems; // BuildPhysicsWorld 所在命名空间，按实际包调整
// 注意：命名空间/类型可能因版本略有差异，请按你项目里对应类型修改

   // [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PhysicsSystemGroup))] // 确保这个 ECB 的 Playback 在构建物理世界之前执行
    [UpdateAfter(typeof(ShipColliderPreBuildSystem))]
    public partial class PrePhysicsCommandBufferSystem : EntityCommandBufferSystem
    {
        // 继承即可，EntityCommandBufferSystem 内部会在 OnUpdate 时播放其内部 ECB
    }

}