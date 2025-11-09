using ilsFramework.Core.Old;
namespace ilsFramework.Core
{
    public class ProcedureNode : FSMState
    {
        
        public virtual void ChangeStateByPopStack()
        {
            ((ProcedureController)Owner).ChangeProcedureByPopStack(); 
        }

        public virtual void OnGizmos()
        {
            
        }
    }
}