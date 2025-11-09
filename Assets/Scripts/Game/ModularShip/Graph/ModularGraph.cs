using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ilsFramework.Core;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 模块的图，用于实现链接飞船,以及查询功能，不将其作为Mono，保持数据结构的独立性
    /// </summary>
    [Serializable]
    public class ModularGraph
    {
        [ShowInInspector]
        public HashSet<ModularGraphNode> nodes = new HashSet<ModularGraphNode>();
        
        [JsonIgnore]
        public Dictionary<int,ModularGraphNode> nodesByID = new Dictionary<int, ModularGraphNode>();
        [JsonIgnore]
        public Dictionary<BaseModularNode, ModularGraphNode> nodesByBaseNode = new Dictionary<BaseModularNode, ModularGraphNode>();
        [ShowInInspector]
        [JsonIgnore]
        public Dictionary<int,HashSet<int>> AdjacencyList = new Dictionary<int,HashSet<int>>();
        [JsonConverter(typeof(UnorderedIntPairDictionaryConverter))]
        public Dictionary<UnorderedIntPair,ModularEdge> edges = new Dictionary<UnorderedIntPair,ModularEdge>();

        private bool IsReBuild = false;
        
        public void AddNode(ModularGraphNode graphNode)
        {
            nodes.Add(graphNode);
            //分配一个ID给新的node
            graphNode.ID = Guid.NewGuid().GetHashCode();
            
            nodesByID.Add(graphNode.ID, graphNode);
            nodesByBaseNode[graphNode.BaseNode] = graphNode;
            AdjacencyList[graphNode.ID] = new HashSet<int>();
        }

        public void Connect(ModularGraphNode graphNodeFirst, ModularGraphNode graphNodeSecond)
        {
            if (graphNodeFirst == graphNodeSecond || !nodes.Contains(graphNodeFirst) || !nodes.Contains(graphNodeSecond))
            {
                return;
            }

            var key = new UnorderedIntPair(graphNodeFirst.ID, graphNodeSecond.ID);
            if (!edges.TryGetValue(key,out _))
            {
                ConnectToAdjacencyList(graphNodeFirst.ID, graphNodeSecond.ID);
                var edge = new ModularEdge(graphNodeFirst, graphNodeSecond);
                edges[key] = edge;
            }
        }

        public void ConnectToAdjacencyList(int nodeID1, int nodeID2)
        {
            if (AdjacencyList.TryGetValue(nodeID1, out HashSet<int> firstEdges))
            {
                firstEdges.Add(nodeID2);
            }
            else
            {
                AdjacencyList[nodeID1] = new HashSet<int>() { nodeID2 };
            }

            if (AdjacencyList.TryGetValue(nodeID2, out HashSet<int> secondEdges))
            {
                secondEdges.Add(nodeID1);
            }
            else
            {
                AdjacencyList[nodeID2] = new HashSet<int>() { nodeID1 };
            }
        }

        public void RemoveNode(ModularGraphNode graphNode)
        {
            if (nodes.Contains(graphNode))
            {
                //找到对应的所有连接
               var adjNodeIDs = this.AdjacencyList[graphNode.ID];
                foreach (var id in adjNodeIDs)
                {
                   Disconnect(graphNode.ID, id);
                }
                
                nodes.Remove(graphNode);
                nodesByID.Remove(graphNode.ID);
                nodesByBaseNode.Remove(graphNode.BaseNode);
            }
        }

        public void Disconnect(ModularEdge edge)
        {
            Disconnect(edge.From, edge.To);
        }

        public void Disconnect(ModularGraphNode graphNodeFirst, ModularGraphNode graphNodeSecond)
        {
            Disconnect(graphNodeFirst.ID, graphNodeSecond.ID);
        }

        public void Disconnect(int idFirst, int idSecond)
        {
            //Vector2
            if (AdjacencyList.TryGetValue(idFirst, out HashSet<int> firstAdjacencyList) && AdjacencyList.TryGetValue(idSecond, out HashSet<int> secondAdjacencyList))
            {
                firstAdjacencyList.Remove(idSecond);
                secondAdjacencyList.Remove(idFirst);
                
                //删去字典中的值
                edges.Remove(new UnorderedIntPair(idFirst, idSecond));
            }
        }

        public ModularGraphNode GetNodeByID(int graphNodeID)
        {
            return nodesByID[graphNodeID];
        }

        public HashSet<int> GetAdjacencyList(int graphNodeID)
        {
            return AdjacencyList[graphNodeID];
        }

        public void AppendNode(ModularGraphNode graphNode)
        {
            AddNode(graphNode);
            //遍历查找所有对应的模块
;
            CheckNodeConnections(graphNode);
        }

        private void CheckNodeConnections(ModularGraphNode graphNode)
        {
            var snapBound = graphNode.BaseNode;
            return;
            Collider[] colliders = new Collider[10];
            var count  = Physics.OverlapBoxNonAlloc(snapBound.transform.position, snapBound.SnapBound.size/2f, colliders, snapBound.transform.rotation,
                ColliderLayer.BoundMask);
            for (int i = 0; i < count; i++)
            {
                var collider = colliders[i];
                var nodeBase = collider.transform.parent.GetComponent<BaseModularNode>();
                if (nodeBase != null)
                {
                    if (nodesByBaseNode.TryGetValue(nodeBase, out ModularGraphNode _graphNode))
                    {
                        Connect(_graphNode,graphNode);
                    }
                }
            }
        }

        public void Save()
        {
            // 序列化为JSON字符串
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            string json = JsonConvert.SerializeObject(this,settings);

            // 保存路径（使用Unity的持久化数据路径）
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");

            // 写入文件
            File.WriteAllText(path, json);

            Debug.Log($"数据已保存至: {path}");
        }

        public ModularGraph Load()
        {
            string path = Path.Combine(Application.persistentDataPath, "savefile.json");
            path.LogSelf();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var settings = new JsonSerializerSettings();
                ModularGraph data = JsonConvert.DeserializeObject<ModularGraph>(json,settings);
                return data;
            }
            else
            {
                Debug.LogWarning("存档文件不存在！");
                return null;
            }
        }

        public async UniTask Rebuild(Transform parent,ShipCore core,Dictionary<uint,BaseModularNode> modularNodes)
        {
            IsReBuild = true;
            var temp = nodes.ToList();
            List<UniTask> tasks = new List<UniTask>();
            foreach (var node in temp)
            {
                if (Table<tShipModular>.TryGet(node.ModularID,out var tShipModular))
                {
                    tasks.Add(LoadSingleNode(node, tShipModular));
                }
            }

            await UniTask.WhenAll(tasks);
            
            foreach (var edge in edges)
            {
                ConnectToAdjacencyList(edge.Value.From, edge.Value.To);
            }

            async UniTask LoadSingleNode(ModularGraphNode node,tShipModular tShipModular)
            {
                var load = Asset.LoadResourceAsync<GameObject>("ShipModular", tShipModular.Path, LoadResourcePriority.Default);
                await load;
                var Instantiate = load.GetResult().InstantiateAsync(parent);
                await Instantiate;
                    
                var instance = Instantiate.Result;
                instance.transform.position = node.Position;
                node.BaseNode = instance.GetComponent<BaseModularNode>();
                node.BaseNode.core = core;
                node.BaseNode.InstanceID = (uint)modularNodes.Count;
                nodesByBaseNode[node.BaseNode] = node;
                nodesByID[node.ID] = node;
                modularNodes.Add(node.BaseNode.InstanceID,node.BaseNode);
                
            }
        }

        public void Update()
        {
            if (IsReBuild)
            {
                return;
            }
            foreach (var node in nodes)
            {
                node.UpdateNode();
            }
        }
        
        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var node in nodes)
            {
                if (node.BaseNode)
                {
                    Gizmos.DrawCube(node.BaseNode.transform.position,node.BaseNode.transform.localScale *0.1f);
                }
                
            }

            foreach (var edgesInNode in edges.Values)
            {

                    if (nodesByID.TryGetValue(edgesInNode.From,out var firstNode) && nodesByID.TryGetValue(edgesInNode.To,out var secondNode) && firstNode.BaseNode && secondNode.BaseNode)
                    {
                        Gizmos.DrawLine(firstNode.BaseNode.transform.position,secondNode.BaseNode.transform.position);
                    }
                

            }
        }
    }
}