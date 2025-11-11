namespace ilsFramework.Core
{
    public abstract class ProcedureInitializer
    {
        public abstract void InitializeProcedure(StateMachine controller);
        
        public abstract void StartProcedure(StateMachine controller);
    }
}