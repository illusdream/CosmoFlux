using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;

namespace ilsFramework.Core
{
    public class StateMachine
    {
        //基础查询
        [ShowInInspector]
        public Dictionary<string,State> states = new Dictionary<string, State>();
        [ShowInInspector]
        public FSMStateChain chain = new FSMStateChain();
        
        public State Root { get; private set; }
        
        List<string> stateNamesBuffer = new List<string>();
        
        private TransitionSequencer transitionSequencer = new TransitionSequencer();

        private string curStateKey;
        private List<string> curStateKeyList;
        
        private Stack<State> BufferStack = new Stack<State>();
        private HashSet<State> findLcaBuffer = new HashSet<State>();
        
        private (State from,State lca, State to)? transition;

        public bool IsExcuting { get; private set; } = false;
        public StateMachine()
        {
            Root = new RootState(this);
            states.Add("Root", Root);
        }
        [ShowInInspector]
        public void UpdateState(float deltaTime)
        {
            foreach (var state in chain.GetStates())
            {
                state.LogSelf();
                state.Update(deltaTime);
            }
        }

        public void TransitionUpdate()
        {
            
        }

        public void SequencerUpdate()
        {
            
        }

        public void Update(float deltaTime)
        {
            if(!transitionSequencer.InTransition)
                TransitionUpdate();
            if (transitionSequencer.InTransition)
                SequencerUpdate();
            if(!transitionSequencer.InTransition)
                UpdateState(deltaTime);
        }
        
        [ShowInInspector]
        public void RegisterState(string stateName,State state)
        {
            string prefix ="";
            string key = stateName;
            int lastDotIndex = stateName.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                prefix = stateName.Substring(0, lastDotIndex);
                key = stateName.Substring(lastDotIndex + 1);
            }

            RegisterState(prefix, key, state);
        }
        public void RegisterState(string prefix,string key,State state)
        {
            if (string.IsNullOrEmpty(prefix))
            {

                Root.AddChild(key,state);
                return;
            }
            if (states.TryGetValue(prefix, out State existingState))
            {
                existingState.AddChild(key, state);
            }
            else
            {
                $"不存在前置状态 key：{prefix}".ErrorSelf();
            }
        }

        public void UnregisterState(string stateName)
        {
            BufferStack.Clear();
            if (states.TryGetValue(stateName, out State state))
            {
                BufferStack.Push(state);
                state.Parent.Children.Remove(state.Key);

                while (BufferStack.Any())
                {
                   var  cur = BufferStack.Pop();
                   foreach (var childState in cur.Children.Values)
                   {
                       BufferStack.Push(childState);
                       childState.Parent = null;
                   }
                   cur.Children.Clear();
                   states.Remove(cur.FullKey);
                }
            }
        }
        [ShowInInspector]
        public State GetLca(string stateFrom, string stateTo)
        {
            
            if (states.TryGetValue(stateFrom, out State state) && states.TryGetValue(stateTo, out State nextState))
            {
                return GetLca(state, nextState);
            }

            return null;
        }
        public State GetLca(State stateFrom, State stateTo)
        {
            findLcaBuffer.Clear();
            foreach (var state in stateFrom.GetPathToRoot())
            {
                findLcaBuffer.Add(state);
            }
            foreach (var state in stateTo.GetPathToRoot())
            {
                if (findLcaBuffer.Contains(state))
                {
                    state.FullKey.LogSelf();
                    return state;
                }
            }
            return null;
        }
        [ShowInInspector]
        public void StartFrom(string stateStart)
        {
            State currentStartState = null;
            if (states.TryGetValue(stateStart, out State state))
            {
                chain.Clear();
                currentStartState = state;
                var next = currentStartState.GetInitialState();
                while (next != null)
                {
                    currentStartState = next;
                    next = currentStartState.GetInitialState();
                }
                StartEnterChain(currentStartState, null);
                IsExcuting = true;
            }
        }
        [ShowInInspector]
        public void ChangeState(string stateTo)
        {
            if (states.TryGetValue(stateTo, out State state))
            {
                ChangeState(state);
            }
        }
        
        public void ChangeState(State targetState)
        {
#if UNITY_EDITOR
            if (!states.ContainsKey(targetState.FullKey))
            {
                $"不存在该状态机中的状态：{targetState.FullKey}".ErrorSelf();
                return;
            }
#endif
            if(transitionSequencer.InTransition)
                BufferStack.Clear();
            var lca = GetLca(chain.currentState, targetState);
            if (lca != null)
            {
                transitionSequencer.BeginTransition(()=> StartExitChain(chain.currentState,lca),()=>StartEnterChain(targetState, lca));
            }
        }

        private void StartExitChain(State start, State lca)
        {
            start.LogSelf();
            for (var s = start; s != null && s != lca; s = s.Parent)
            {
                var state = chain.Pop();
                state.AddExitTask(transitionSequencer);
            }
        }

        private void StartEnterChain(State targetState,State lca)
        {
            BufferStack.Clear();
            for(var s = targetState; s != null && s != lca; s = s.Parent)
                BufferStack.Push(s);

            foreach (var state in BufferStack)
            {
                state.AddEnterTask(transitionSequencer);
            }
            while (BufferStack.Count > 0)
                chain.Push(BufferStack.Pop());
        }
    }
}