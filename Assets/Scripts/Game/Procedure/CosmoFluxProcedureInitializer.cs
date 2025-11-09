using ilsFramework.Core;

namespace Game
{
    public class CosmoFluxProcedureInitializer : ProcedureInitializer
    {
        public override void InitializeProcedure(ProcedureController controller)
        {
            controller.AddState<InitializeState>();
            controller.AddState<MainGameState>();
            controller.AddState<TestGameState>();
            
            
            
            
            
            controller.StartState<InitializeState>();
        }
    }
}