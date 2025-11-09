using System;
using System.Collections.Generic;
using ilsFramework.Core;
using UnityEngine;

namespace Game
{
    public class TestBuilder : MonoBehaviour
    {
        public GameObject Prefab;

        public GameObject Instance;

        Collider _collider;

        RaycastHit[] _hits;
        
        RaycastHit hit;
        float _lastTime;

        private Vector3 offestP;
        
        public Material BuildMaterial;
        
        public Material BaseMaterial;
        
        public Material originMaterial;

        public float OffestPercent =1;

        public void Start()
        {
            _hits = new RaycastHit[10];
        }

        public void Update()
        { 
            _lastTime -= Time.fixedDeltaTime;

            if (_collider&&Instance &&_collider.gameObject == Instance)
            {
                Destroy(Instance);
                Instance = null;
            }
            
            
            if (Instance)
            {
                if (_collider)
                {
                    float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
                    OffestPercent += scroll;
                    OffestPercent = Mathf.Clamp(OffestPercent,-1,1);
                    Instance.SetActive(true);
                    foreach (var componentInChild in Instance.GetComponentsInChildren<Collider>())
                    {
                        if (_collider.gameObject.layer == ColliderLayer.Bound)
                        {
                            continue;
                        }
                        componentInChild.enabled = false;
                    }
                    var size = Instance.GetComponent<BaseModularNode>().SnapBound.bounds.extents;
                    var obb = CalculateOBBFromBoxCollider(Instance.GetComponent<BaseModularNode>().SnapBound);
                    var signDir = GetSignDirection(hit.normal);
                    var off = GetDistanceToEdgeInDirection(obb, hit.normal) *OffestPercent;
                    var offest = new Vector3(signDir.x * size.x, signDir.y * size.y, signDir.z * size.z);
                    offestP =  hit.normal * off;
                    Instance.transform.position = hit.point + hit.normal * off; 
                    if (UnityEngine.Input.GetMouseButtonDown(0) &&_lastTime <0)
                    {
                        foreach (var componentInChild in Instance.GetComponentsInChildren<Collider>())
                        {
                            componentInChild.enabled = true;
                        }
                        Instance.GetComponent<MeshRenderer>().material=BaseMaterial;
                        if (_collider.GetComponent<BaseModularNode>() is BaseModularNode node )
                        {
                            var graphNode = new ModularGraphNode() { BaseNode = Instance.GetComponent<BaseModularNode>() };
                            node.core.Graph.AppendNode(graphNode);
                        }
                        Instance.transform.SetParent(_collider.GetComponent<BaseModularNode>().core.transform,true);
                        Instance = null;
                        _lastTime = 0.2f;
                    }
                }
                else
                {
                    Instance.SetActive(false);
                }
            }
            if (UnityEngine.Input.GetMouseButtonDown(0) && !Instance &&_lastTime <0)
            {
                Instance = Instantiate(Prefab);
                BaseMaterial = Instance.GetComponent<MeshRenderer>().material;
                Instance.GetComponent<MeshRenderer>().material=BuildMaterial;
                foreach (var componentInChild in Instance.GetComponentsInChildren<Collider>())
                {
                   // componentInChild.enabled = false;
                }

                OffestPercent = 1;
                _lastTime = 0.2f;
            }
            
            _collider = null;
            int count = Physics.RaycastNonAlloc(new Ray(transform.position + transform.TransformDirection(Vector3.down * 0.05f),
                transform.TransformDirection(Vector3.forward * 100)), _hits,100,ColliderLayer.BoundMask);
            float length = int.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (_hits[i].collider&& _hits[i].transform.gameObject !=Instance )
                {
                    if (_collider)
                    {
                        if (_hits[i].distance < length)
                        {
                            length = _hits[i].distance;
                            if (_collider)
                            {
                                _collider.GetComponent<MeshRenderer>().materials = new [] {originMaterial};
                            }
                            _collider = _hits[i].collider;
                            originMaterial = _collider.GetComponent<MeshRenderer>().material;
                            hit = _hits[i];
                        }
                    }
                    else
                    {
                        _collider = _hits[i].collider;
                        originMaterial = _collider.GetComponent<MeshRenderer>().material;
                        hit = _hits[i];
                    }
                }
            }
        }

        public void FixedUpdate()
        {
         
        }

        //获取一个指定到一个方向上的法向量
        public Vector3 GetSignDirection(Vector3 direction)
        {
            var temp = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));

            return new Vector3(Mathf.Floor(direction.x/temp), Mathf.Floor(direction.y/temp), Mathf.Floor(direction.z/temp));
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position +transform.TransformDirection(Vector3.down * 0.05f), transform.TransformDirection(Vector3.forward * 100));
           // Gizmos.DrawRay(transform.position +transform.TransformDirection(Vector3.right * 0.2f), transform.TransformDirection(Vector3.forward * 100));
           if (_collider)
           {
              // Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
           }

           if (hit.collider != null)
           {
               Gizmos.color = Color.blue;
               Gizmos.DrawRay(hit.point,offestP);
           }

           if (Instance)
           {
               Collider[] colliders = new Collider[10];
               var count  = Physics.OverlapBoxNonAlloc(Instance.transform.position, Instance.GetComponent<BaseModularNode>().SnapBound.size/2f, colliders, Instance.transform.rotation,
                   ColliderLayer.BoundMask);
               Gizmos.DrawWireCube(Instance.transform.position,Instance.GetComponent<BaseModularNode>().SnapBound.size/2f);
               for (int i = 0; i < count; i++)
               {
                   var collider = colliders[i];
                   var nodeBase = collider.transform.GetComponent<BaseModularNode>();
                   if (nodeBase != null)
                   {
                        Gizmos.DrawWireCube(collider.transform.position, collider.bounds.size);
                   }
               }
           }
        }
        public struct OBB
        {
            public Vector3 center;
            public Vector3 size;
            public Vector3[] axes;
        }
        public OBB CalculateOBBFromBoxCollider(BoxCollider collider)
        {
            OBB obb = new OBB();
        
            // 获取BoxCollider的中心和大小（本地坐标）
            Vector3 center = collider.center;
            Vector3 size = collider.size;
        
            // 考虑物体的缩放
            Vector3 lossyScale = collider.transform.lossyScale;
            size = Vector3.Scale(size, lossyScale);
        
            // 获取物体的旋转
            Quaternion rotation = collider.transform.rotation;
        
            // 计算OBB的中心（世界坐标）
            obb.center = collider.transform.TransformPoint(center);
        
            // 计算OBB的三个轴方向（世界坐标）
            obb.axes = new Vector3[3];
            obb.axes[0] = rotation * Vector3.right;   // X轴
            obb.axes[1] = rotation * Vector3.up;      // Y轴
            obb.axes[2] = rotation * Vector3.forward; // Z轴
        
            // OBB的大小就是BoxCollider的大小（考虑缩放）
            obb.size = size;
        
            return obb;
        }
    
        // 计算在指定方向上从中心到OBB边缘的距离
        public float GetDistanceToEdgeInDirection(OBB obb, Vector3 direction)
        {
            // 确保方向是单位向量
            direction.Normalize();
        
            // 计算方向向量在OBB三个轴上的投影长度
            float projX = Mathf.Abs(Vector3.Dot(direction, obb.axes[0]));
            float projY = Mathf.Abs(Vector3.Dot(direction, obb.axes[1]));
            float projZ = Mathf.Abs(Vector3.Dot(direction, obb.axes[2]));
        
            // 计算在指定方向上OBB的半长
            float halfLength = 
                (projX * obb.size.x * 0.5f) + 
                (projY * obb.size.y * 0.5f) + 
                (projZ * obb.size.z * 0.5f);
        
            return halfLength;
        }
    }
}