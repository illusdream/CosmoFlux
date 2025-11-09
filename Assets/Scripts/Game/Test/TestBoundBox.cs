using System;
using UnityEngine;

namespace Game
{
    public class TestBoundBox : MonoBehaviour
    {
        public Renderer render;

        public void OnDrawGizmos()
        {
            if (render)
            {
                Gizmos.DrawWireCube(render.bounds.center, render.bounds.size);
            }
        }
    }
}