using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    [Serializable]
    public abstract class BaseIndexCollection
    {
        public string IndexName;
        
        public abstract void AddItem(object key,uint id);
        
    }
    
    
    [Serializable]
    public class IndexCollection<T> : BaseIndexCollection
    {
        [ShowInInspector]
        public Dictionary<T,List<uint>> IndexDictionary = new Dictionary<T,List<uint>>();
        public override void AddItem(object key, uint id)
        {
            if (key is T _key)
            {
                if (IndexDictionary.TryGetValue(_key,out var list))
                {
                    list.Add(id);
                }
                else
                {
                    IndexDictionary.Add(_key, new List<uint> { id });
                }
            }
        }

        public List<uint> GetResult(T _key)
        {
            if (IndexDictionary.TryGetValue(_key, out var result))
            {
                return result;
            }
            return new List<uint>();
        }
    }
}