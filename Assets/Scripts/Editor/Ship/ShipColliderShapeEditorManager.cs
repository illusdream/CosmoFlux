using System;
using System.Collections.Generic;
using Game;

namespace Editor
{
    public static class ShipColliderShapeEditorManager
    {
        public static Dictionary<BaseModularNode,Action<BoxColliderShape>> onShapeEditorChanged = new Dictionary<BaseModularNode,Action<BoxColliderShape>>();
        public static void RegisterChangeEvent(BaseModularNode baseModularNode,Action<BoxColliderShape> _onShapeEditorChanged)
        {
            if (ShipColliderShapeEditorManager.onShapeEditorChanged.TryGetValue(baseModularNode,out var value))
            {
                value += _onShapeEditorChanged;
            }
            else
            {
                onShapeEditorChanged.Add(baseModularNode,_onShapeEditorChanged);
            }
        }

        public static void UnRegisterChangeEvent(BaseModularNode baseModularNode, Action<BoxColliderShape> _onShapeEditorChanged)
        {
            if (onShapeEditorChanged.TryGetValue(baseModularNode,out var value))
            {
                value -= _onShapeEditorChanged;
            }
        }

        public static void UnRegisterNode(BaseModularNode baseModularNode)
        {
            onShapeEditorChanged.Remove(baseModularNode);
        }

        public static void InvokeOnShapeEditorChanged(BaseModularNode baseModularNode,BoxColliderShape value)
        {
            if (onShapeEditorChanged.TryGetValue(baseModularNode,out var _value))
            {
                _value?.Invoke(value);
            }
        }
    }
}