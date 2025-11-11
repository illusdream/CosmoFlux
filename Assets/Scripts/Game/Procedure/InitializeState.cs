using ilsFramework.Core;

namespace Game
{
    public class InitializeState : State
    {
        public const string StateKey = "InitializeState";

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override State GetTransition()
        {
            if (!Config.GetConfig<ProcedureConfig>().EnableTestScene)
            {
                return GetStateInSameLayer(MainGameState.MainGameStateKey);
            }
            else
            {
                return GetStateInSameLayer(TestGameState.MainGameStateKey);
            }
            return null;
        }

        public override void OnUnityUpdate()
        {
            base.OnUnityUpdate();
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
        
        
    }
}
