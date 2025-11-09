using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ilsFramework.Core
{
    public class TransitionSequencer
    {
        public bool InTransition { get; private set; }
        
        public Queue<StateTask> TransitionQueue { get; private set; } = new Queue<StateTask>();

        private List<UniTask> waitList = new List<UniTask>();

        public void Enqueue(StateTask stateTask)
        {
            TransitionQueue.Enqueue(stateTask);
        }
        
        public async Cysharp.Threading.Tasks.UniTaskVoid BeginTransition(Action startAction, Action endAction)
        {
            waitList.Clear();
            InTransition = true;
            startAction?.Invoke();
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue().action is { } result)
                {
                    waitList.Add(result.Invoke());
                }

            }
            
            await UniTask.WhenAll(waitList);

            waitList.Clear();
            endAction?.Invoke();
            while (TransitionQueue.Count > 0)
            {
                if (TransitionQueue.Dequeue().action is { } result)
                {
                    waitList.Add(result.Invoke());
                }
            }
            await UniTask.WhenAll(waitList);
            InTransition = false;
        }
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