using ilsFramework.Core;

namespace Game
{
    public class CosmoFluxProcedureInitializer : ProcedureInitializer
    {
        public override void InitializeProcedure(StateMachine controller)
        {
           // controller.AddState<InitializeState>();
           // controller.AddState<MainGameState>();
           // controller.AddState<TestGameState>();
            controller.RegisterState(InitializeState.StateKey,new InitializeState());
            controller.RegisterState(MainGameState.MainGameStateKey,new MainGameState());
          //  controller.RegisterState("2",new TestGameState());
            
            
            
            
           // controller.StartState<InitializeState>();
        }

        public override void StartProcedure(StateMachine controller)
        {
            controller.StartFrom(InitializeState.StateKey);
        }
    }
}