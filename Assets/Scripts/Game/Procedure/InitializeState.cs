using ilsFramework.Core;

namespace Game
{
    public class InitializeState : ProcedureNode
    {
        public override void ChangeStateByPopStack()
        {
            base.ChangeStateByPopStack();
        }

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            if (!Config.GetConfig<ProcedureConfig>().EnableTestScene)
            {
                ChangeState<MainGameState>();
            }
            else
            {
                ChangeState<TestGameState>();
            }
            base.OnUpdate();
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnLogicUpdate()
        {
            base.OnLogicUpdate();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        
    }
}
