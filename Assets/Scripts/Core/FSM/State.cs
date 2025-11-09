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
        
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate(float deltaTime) { }

        public void Enter()
        {
            $"Entering state {FullKey}".LogSelf();
            OnEnter();
        }
        
        public void Update(float deltaTime) {
            OnUpdate(deltaTime);
        }

        public void Exit()
        {
            $"Exiting state {FullKey}".LogSelf();
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
            sequencer.Enqueue(new StateTask(()=>UniTask.Create(async () =>
            {
                "Test".LogSelf();
                await UniTask.WaitForSeconds(1);
                "Test1".LogSelf();
            })));
        }

        public virtual void AddEnterTask(in TransitionSequencer sequencer)
        {
            sequencer.Enqueue(new StateTask(null));
        }
    }
}