#if UNITY_EDITOR
using ilsFramework.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ilsFramework.Core.Editor
{
    public static class UIEditorHandler
    {
        [MenuItem("GameObject/ilsFramework/UI/BasePanel", false)]
        static void CreateCustomObject(MenuCommand menuCommand) {
            // 创建新的GameObject
            GameObject customGO = new GameObject("BasePanel");
            
            customGO.layer = LayerMask.NameToLayer("UI");
            
            // 添加自定义组件
            customGO.AddComponent<RectTransform>();
            customGO.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = customGO.AddComponent<CanvasScaler>();
            customGO.AddComponent<GraphicRaycaster>();
            customGO.AddComponent<CanvasGroup>();
            
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referencePixelsPerUnit = 100f;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
            scaler.referenceResolution = Config.GetConfigInEditor<UIConfig>().ReferenceResolution;
            
            GameObjectUtility.SetParentAndAlign(customGO, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(customGO, "Create " + customGO.name);
            Selection.activeObject = customGO;
        }
    }
}
#endif