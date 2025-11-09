using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ModularGraphNode
    {
        //一个单独的ID ，用于序列化
        [ShowInInspector]
        public int ID { get; set; }
        
        //对应模块的ID，用于绑定对应的Mono对象
        [ShowInInspector]
        public uint ModularID { get; set; }
        [JsonIgnore]
        [ShowInInspector]
        private BaseModularNode _node;
        
        [JsonIgnore]
        public BaseModularNode BaseNode
        {
            get => _node;
            set
            {
                _node = value;
                Position = _node.transform.position;
            }
        }
        
        public Vector3 Position { get; set; }

        public void UpdateNode()
        {
            Position = _node.transform.position;
        }
        
    }
}