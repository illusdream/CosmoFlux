using System;
using ilsFramework.Core;
using UnityEngine;

namespace Game
{
    public class TestCollider  : MonoBehaviour
    {
        public void OnCollisionStay(Collision other)
        {
            "Test".LogSelf();
        }
    }
}