using System;
using System.Collections.Generic;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// 基础的模块节点，这个用于将Unity的物理与实际的飞船结合起来
    /// </summary>
    public class BaseModularNode : MonoBehaviour
    {
        public uint InstanceID;
        
        [LabelText("模块质量")]
        public float Mass;
        
        /// <summary>
        /// 基础的建造辅助线框，以及用于链接至飞船网络
        /// </summary>
        [LabelText("辅助建造包围盒")]
        [VerticalGroup("辅助建造包围盒")]
        public BoxCollider SnapBound;

        public ShipCore core;

        public int ModularID;

        public List<BaseModular> FunctionModulars = new List<BaseModular>();

        [LabelText("辅助建造包围盒")]
        public BoxColliderShape BuildHelperBound;
        [LabelText("组件碰撞箱")]
        public List<BoxColliderShape> CollisonBounds = new List<BoxColliderShape>();


        public void InvokeCollision(ShipCollisionEvent shipCollisionEvent)
        {
            $"{gameObject.name}Invoke!".LogSelf();
        }
        
        #region Editor
#if UNITY_EDITOR
        [LabelText("包围盒额外量")]
        [VerticalGroup("辅助建造包围盒")]
        public Vector3 SnapBoundExtend;
        
        [Button("计算辅助建造包围盒")]
        [VerticalGroup("辅助建造包围盒")]
        public void BuildSnapBound()
        {
            //遍历子物体，以便获取所有可能的碰撞箱，然后计算Bound，并将其赋给BaseBound
            var colliders = transform.GetComponentsInChildren<Collider>();
            Bounds bounds = new Bounds() {center = transform.position};
            foreach (var collider in colliders)
            {
                if (collider == SnapBound)
                {
                    continue;
                }
                bounds.Encapsulate(collider.bounds);
            }
            bounds.Expand(SnapBoundExtend);
            if (!SnapBound)
            {
                if (transform.Find("BuildBound") is Transform bb)
                {
                    SnapBound = bb.gameObject.AddComponent<BoxCollider>();
                }
                else
                {
                    var go = new GameObject("BuildBound");
                    go.transform.SetParent(transform);
                    SnapBound = go.AddComponent<BoxCollider>();
                }
             
            }
            SnapBound.center =transform.InverseTransformPoint(bounds.center);
            SnapBound.gameObject.layer = ColliderLayer.Bound;
            SnapBound.size = bounds.size;
        }

#endif
        #endregion
    
    }
    
    


}