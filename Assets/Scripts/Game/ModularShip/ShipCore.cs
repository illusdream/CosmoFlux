using System;
using System.Collections.Generic;
using Game.Ship;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace Game
{
    public struct MyChunkComponentData : IComponentData
    {
        public int num;
    }
    public class ShipCore : MonoBehaviour,IHostedLogicUpdate
    {
        public MonoECSLinker linker;
        [ShowInInspector]
        public ModularGraph Graph;

        public ShipEcsPhysicLinker ShipEcsPhysicLinker;

        public uint ID;

        public Transform ShipCenter;
        
        //记录功能组件和对应的Node，用于快速查找和遍历
        
        public event Action<ShipCore> AfterInitializeShipCore; 

        public event Action<BaseModular> OnModularAdded;
        
        public event Action<BaseModular> OnModularRemoved;

        public Dictionary<EModularType, List<BaseModular>> Modulars = new Dictionary<EModularType, List<BaseModular>>();
        [ShowInInspector]
        public Dictionary<uint,BaseModularNode> ModularNodes = new Dictionary<uint,BaseModularNode>();
        
        public async void Start()
        {
            Graph = new ModularGraph();

            var instance = Graph.Load();
            if (instance != null)
            {
                await instance.Rebuild(transform,this,ModularNodes);
                Graph = instance;
                InitializeShip();
                ShipManager.Instance.RegisterShip(this);
                CreateEntity();
            }
            else
            {
                //循环遍历
                var colliders = transform.GetComponentsInChildren<BaseModularNode>();
                Bounds bounds = new Bounds() {center = transform.position};
                foreach (var collider in colliders)
                {
                    ModularGraphNode node = new ModularGraphNode() { BaseNode = collider};
                    node.ModularID = (uint)collider.ModularID;
                    Graph.AppendNode(node);
                    collider.core = this;
                }
                InitializeShip();
                ShipManager.Instance.RegisterShip(this);
                CreateEntity();
            }

            
        }
        [ShowInInspector]
        public void CreateEntity()
        {
            linker.CreateLinkedEntity();
        }

        public void OnEnable()
        {
             ShipManager.Instance?.RegisterHostedUpdate(this);
        }

        [Button]
        public void Save()
        {
            Graph.Save();
        }

        public void OnDisable()
        {
            ShipManager.Instance.UnregisterHostedUpdate(this);
        }

        public void Update()
        {
        }

        public void OnDestroy()
        {
            linker.DestroyLinkedEntity();
        }

        public void OnDrawGizmos()
        {
            Graph?.DrawGizmos();
        }

        public void HostedLogicUpdate()
        {
            Graph.Update();
        }

        private void InitializeShip()
        {
            for (int i = 0; i < (int)EModularType.Count; i++)
            {
                Modulars.Add((EModularType)i, new List<BaseModular>());
            }
            foreach (var node in Graph.nodes)       
            {
                if (node.BaseNode)
                {
                    foreach (var modular in node.BaseNode.FunctionModulars)
                    {
                        switch (modular.ModularType)
                        {
                            case EModularType.Struct:
                                AddModular(EModularType.Struct, modular);
                                break;
                            case EModularType.Engine:
                                AddModular(EModularType.Engine, modular);
                                break;
                            case EModularType.Energy:
                                AddModular(EModularType.Energy, modular);
                                break;
                            case EModularType.Weapon:
                                AddModular(EModularType.Weapon, modular);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
               
            }
            AfterInitializeShipCore?.Invoke(this);

            void AddModular(EModularType type,BaseModular modular)
            {
                if (Modulars.TryGetValue(type, out List<BaseModular> modulars))
                {
                    modulars.Add(modular);
                }
                else
                {
                    Modulars.Add(type, new List<BaseModular>() { modular });
                }
            }
        }

        public bool QueryModular(uint modularID, out BaseModularNode modularNode)
        {
            return ModularNodes.TryGetValue(modularID, out modularNode);
        }
        

        public void AddModularToShip(BaseModularNode modular,uint UseModularID)
        {
            var graphNode = new ModularGraphNode() { BaseNode =modular };
      
            graphNode.ModularID =UseModularID;
            Graph.AppendNode(graphNode);
            
            modular.core = this;
            modular.InstanceID = (uint)ModularNodes.Count;
            ModularNodes.Add((uint)ModularNodes.Count, modular);
        }
    }
}