using System.Collections.Generic;
using System.Linq;

namespace ilsFramework.Core
{
    public class FSMStateChain
    {
        //第一个节点是root，最后一个是叶子
        public LinkedList<State> states = new LinkedList<State>();
        
        public State currentState =>states.Count >0 ? states.Last.Value : null;
        

        public void Clear()
        {
            states.Clear();
        }
        public void Push(State state)
        {
            states.AddLast(state);
            state.Enter();
        }

        public State Pop()
        {
            var state = states.Last();
            states.RemoveLast();
            state.Exit();
            return state;
        }
        
        public IEnumerable<State> GetStates()
        {
            foreach(var state in states)
                yield return state;
        }

        public IEnumerable<State> GetReverseStates()
        {
            LinkedListNode<State> current = states.Last;
            while (current != null)
            {
                yield return current.Value;
                current = current.Previous;
            }
        }
        
        public void Update()
        {
            
        }
    }
}