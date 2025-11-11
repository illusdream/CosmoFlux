using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ilsFramework.Core
{
    public class TransitionSequencer
    {
        public bool InTransition { get; private set; }
        
        public StateMachine StateMachine { get; private set; }

        public Queue<UniTask?> TransitionQueue { get; private set; } = new Queue<UniTask?>();

        private List<UniTask> waitList = new List<UniTask>();
        
        private Stack<State> bufferStack = new Stack<State>();

        public TransitionSequencer(StateMachine machine)
        {
            StateMachine = machine;
        }

        public void Enqueue(UniTask? stateTask)
        {
            TransitionQueue.Enqueue(stateTask);
        }

        public async Cysharp.Threading.Tasks.UniTaskVoid BeginTransition(Action startAction, Action endTaskAction, Action endEnterAction)
        {
            if(InTransition)
                return;
            waitList.Clear();
            bufferStack.Clear();
            InTransition = true;
            startAction?.Invoke();
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue() is { } resultTask)
                {
                    waitList.Add(resultTask);
                }

            }

            await UniTask.WhenAll(waitList);

            waitList.Clear();
            endTaskAction?.Invoke();
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue() is { } resultTask)
                {
                    waitList.Add(resultTask);
                }
            }

            await UniTask.WhenAll(waitList);
            endEnterAction?.Invoke();
            InTransition = false;
        }


        public async UniTaskVoid BeginTransition(State from, State lca, State to)
        {
            if(InTransition)
                return;
            InTransition = true;
            waitList.Clear();
            bufferStack.Clear();
            
            for (var s = from; s != null && s != lca; s = s.Parent)
            {
                var state = StateMachine.chain.Pop();
                state.AddExitTask(this);
            }
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue() is { } resultTask)
                {
                    waitList.Add(resultTask);
                }

            }

            await UniTask.WhenAll(waitList);

            waitList.Clear();

            for(var s = to; s != null && s != lca; s = s.Parent)
                bufferStack.Push(s);

            foreach (var state in bufferStack)
            {
                state.AddEnterTask(this);
            }
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue() is { } resultTask)
                {
                    waitList.Add(resultTask);
                }
            }

            await UniTask.WhenAll(waitList);
            while (bufferStack.Count > 0)
                StateMachine.chain.Push(bufferStack.Pop());
            InTransition = false;
        }

        public class StateTask
        {
            public StateTask(Func<UniTask> action)
            {
                this.action = action;
            }

            public readonly Func<UniTask> action;
        }
    }
}