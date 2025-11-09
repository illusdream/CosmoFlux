using System;
using System.Collections.Generic;
using Game.Input;
using ilsFramework.Core;
using Sirenix.OdinInspector;

namespace Game
{
    public class CommendCollector
    {
        public bool Enabled { get; private set; }
        [ShowInInspector]
        protected Dictionary<string,CommendCollection> _commendCollections = new Dictionary<string, CommendCollection>();

        public void AddCommendCollection(string collectionName)
        {
            _commendCollections[collectionName] = new CommendCollection(50);
        }
        
        public void AddCommend(string commendType, ICommend commend)
        {
            if (_commendCollections.TryGetValue(commendType, out CommendCollection collection))
            {
                collection.Add(commend);
            }
        }
        
        public void Query<T>(string commendType,int firstIndex, int lastIndex, List<T> result) where T : ICommend
        {
            if (!Enabled)
            {
                return;
            }

            if (_commendCollections.TryGetValue(commendType, out CommendCollection collection))
            {
                collection.Query(firstIndex, lastIndex, result);
            }
        }

        public void Update()
        {
            foreach (var collection in _commendCollections)
            {
                collection.Value.Update();
            }
        }

        public bool CheckCurrent<T>(string commendType,out T result) where T : class, ICommend
        {
            result = null;
            if (!Enabled)
            {
                return false;
            }
            if (_commendCollections.TryGetValue(commendType, out CommendCollection collection))
            {
                return collection.CheckCurrent(out result);
            }
            return false;
        }

        public void Clear(string commendType)
        {
            if (_commendCollections.TryGetValue(commendType, out CommendCollection collection))
            {
                collection.Clear();
            }
        }

        public void Clear()
        {
            _commendCollections.Clear();
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }
    }
}