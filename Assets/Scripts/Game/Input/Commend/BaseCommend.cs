using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class BaseCommend : ICommend
    {
        [ShowInInspector]
        public int FrameIndex { get; private set; }
        
        public BaseCommend()
        {
            FrameIndex = FrameworkCore.Instance.LogicFrameIndex;
        }
    }
}