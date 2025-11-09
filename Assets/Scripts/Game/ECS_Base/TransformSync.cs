using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class TransformSync : IComponentData
    {
        public Transform ManagedTransform;
    }
}