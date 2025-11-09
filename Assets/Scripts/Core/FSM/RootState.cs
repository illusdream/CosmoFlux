namespace ilsFramework.Core
{
    public class RootState : State
    {
        public RootState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            SetKeyInfo("","Root");
        }
        
        public override void AddChild(string key, State child)
        {
            child.stateMachine = stateMachine;
            child.Parent = this;
            child.SetKeyInfo("",key);
            Children[key] = child;
            stateMachine.states[key] = child;
        }
    }
}