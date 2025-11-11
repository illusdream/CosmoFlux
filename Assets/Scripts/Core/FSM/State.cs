using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

namespace ilsFramework.Core
{
    public abstract class State
    {
        public StateMachine stateMachine { get;  set; }

        [ShowInInspector]
        public string Key { get; private set; }
        [ShowInInspector]
        public string Prefix { get; private set; }
        
        public string FullKey => string.IsNullOrEmpty(Prefix) ? Key: $"{Prefix}.{Key}";

        public State Parent;
        [ShowInInspector]
        public Dictionary<string, State> Children = new Dictionary<string, State>();
        
        public virtual State GetTransition() => null;
        
        public virtual void OnInit() {}
        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        protected virtual void OnUpdate(float deltaTime)
        {
            OnUnityUpdate();
        }
        
        public virtual void OnUnityUpdate() {}
        public virtual void OnLateUpdate() {}
        public virtual void OnFixedUpdate() {}
        public virtual void OnLogicUpdate() {}
        

        public void Enter()
        {
            OnEnter();
        }
        
        public void Update(float deltaTime) {
            OnUpdate(deltaTime);
        }

        public void Exit()
        {
            OnExit();
        }

        public virtual void AddChild(string key,State child)
        {
            var newPrefix =string.IsNullOrEmpty(Prefix) ? this.Key : string.Join('.', Prefix, this.Key);

            child.stateMachine = stateMachine;
            child.Parent = this;
            child.SetKeyInfo(newPrefix,key);
            Children[key] = child;
            stateMachine.states[string.Join('.',newPrefix,key)] = child;
            child.OnInit();
        }

        public virtual void RemoveSelf()
        {
            stateMachine.UnregisterState(FullKey);
        }

        public void SetKeyInfo(string prefix, string key)
        {
            this.Prefix = prefix;
            this.Key = key;
        }
        [ShowInInspector]
        public IEnumerable<State> GetPathToRoot()
        {
            for(var s = this; s != null; s = s.Parent)
                yield return s;
        }

        public virtual State GetInitialState()
        {
            return Children.Any() ? Children.First().Value : null;
        }

        public override string ToString()
        {
            return FullKey;
        }

        public virtual void AddExitTask(in TransitionSequencer sequencer)
        {

        }

        public virtual void AddEnterTask(in TransitionSequencer sequencer)
        {
           
        }

        public State GetStateInSameLayer(string key)
        {
            if(Parent != null && Parent.Children.TryGetValue(key, out var state))
                return state;
            return null;
        }

        public State GetStateInChild(string key)
        {
            if (Children.TryGetValue(key, out var state))
            {
                return state;
            }
            return null;
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

    }
}