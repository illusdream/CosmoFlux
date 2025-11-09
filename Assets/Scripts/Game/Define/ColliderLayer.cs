using UnityEngine;

namespace Game
{
    public static class ColliderLayer
    {
        public static int Bound = LayerMask.NameToLayer("Bound");
        
        public static LayerMask BoundMask = LayerMask.GetMask("Bound");

        public static uint ShipModularSelfLayer = (uint)ECSColliderLayer.ShipCollider;
        
        public static uint ShipModularCollisionLayer = (uint)ECSColliderLayer.ShipCollider;
    }

    public enum ECSColliderLayer : uint
    {
        Base = 0,
        ShipCollider = 1 << 1,
        
    }
}