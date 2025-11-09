using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace ilsFramework.Core
{
    public class HostedUpdateList<T>
    {
        [ShowInInspector]
        private readonly List<T> _hostedUpdateList;
        [ShowInInspector]
        private readonly List<T> _frameAddList;
        [ShowInInspector]
        private readonly List<T> _frameRemoveList;
        
        private readonly Action<T> detailUpdateCallback;

        public HostedUpdateList(Action<T> detailUpdateCallback)
        {
            this.detailUpdateCallback = detailUpdateCallback;
            _hostedUpdateList = new List<T>();
            _frameAddList = new List<T>();
            _frameRemoveList = new List<T>();
        }
        
        public void Update()
        {
            UpdateAllChangeForHostedUpdateList();
            
            if (detailUpdateCallback is null)
            {
                return;
            }
            
            foreach (var hostedUpdate in _hostedUpdateList)
            {
                detailUpdateCallback.Invoke(hostedUpdate);
            }
        }

        private void UpdateAllChangeForHostedUpdateList()
        {
            foreach (var add in _frameAddList)
            {
                _hostedUpdateList.Add(add);
            }

            _frameAddList.Clear();

            foreach (var remove in _frameRemoveList)
            {
                _hostedUpdateList.Remove(remove);
            }
            
            _frameRemoveList.Clear();
        }

        public void AddUpdate(T update)
        {
            _frameAddList.Add(update);
        }

        public void RemoveUpdate(T update)
        {
            _frameRemoveList.Add(update);
        }
    }
}