using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class BoxColliderShape
    {
        public Vector3 center;
        public Vector3 size = Vector3.one;
        public Quaternion rotation = Quaternion.identity;
    }
}