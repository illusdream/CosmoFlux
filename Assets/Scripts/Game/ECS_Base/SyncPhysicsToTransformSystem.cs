using Unity.Entities;
using Unity.Transforms;

namespace Game
{
    [UpdateInGroup(typeof(TransformSystemGroup))]
    public partial class SyncPhysicsToTransformSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .ForEach((in LocalTransform translation, in TransformSync syncData) =>
                {
                    // 确保 Transform 没有在游戏逻辑中被意外销毁
                    if (!syncData.ManagedTransform)
                    {
                        return;
                    }
                    syncData.ManagedTransform.SetPositionAndRotation(translation.Position, translation.Rotation);
                })
                .Run();
        }
    }
}