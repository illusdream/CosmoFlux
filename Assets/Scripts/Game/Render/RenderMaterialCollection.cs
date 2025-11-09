using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class RenderMaterialCollection : MonoBehaviour
    {
        public Dictionary<string, Material> materials = new Dictionary<string, Material>();

        public Renderer renderer;
        public void Start()
        {
            if (renderer)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    materials.Add($"Base{i}", renderer.materials[i]);
                }
            }
        }

        public void Reset()
        {
            renderer = GetComponent<Renderer>();
        }

        public void AddMaterial(string key,Material mat)
        {
            materials.Add(key, mat);
            renderer.materials = materials.Values.ToArray();
        }

        public void RemoveMaterial(string key)
        {
            materials.Remove(key);
            renderer.materials = materials.Values.ToArray();
        }

        public void ResetMaterials(List<(string, Material)> materials)
        {
            this.materials.Clear();
            foreach (var material in materials)
            {
                this.materials.Add(material.Item1, material.Item2);
            }
            renderer.materials = this.materials.Values.ToArray();
        }
        
        
    }
}