namespace ilsFramework.Core.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class IDEJumpHelper
    {
        /// <summary>
        /// 跳转到指定脚本的特定行
        /// </summary>
        public static void OpenScriptAtLine(string scriptAssetPath, int lineNumber)
        {
            // 加载脚本资源
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptAssetPath);
            if (script != null)
            {
                AssetDatabase.OpenAsset(script, lineNumber);
            }
            else
            {
                Debug.LogError($"找不到脚本: {scriptAssetPath}");
            }
        }
    
        /// <summary>
        /// 跳转到指定类型的定义
        /// </summary>
        public static void OpenTypeDefinition(System.Type type)
        {
            // 查找该类型的 MonoScript
            MonoScript[] allScripts = Resources.FindObjectsOfTypeAll<MonoScript>();
            foreach (MonoScript script in allScripts)
            {
                if (script.GetClass() == type)
                {
                    AssetDatabase.OpenAsset(script);
                    return;
                }
            }
            Debug.LogError($"找不到类型 {type.Name} 的脚本文件");
        }
    
        /// <summary>
        /// 跳转到当前选中的脚本
        /// </summary>
        public static void OpenSelectedScript()
        {
            Object selected = Selection.activeObject;
            if (selected is MonoScript)
            {
                AssetDatabase.OpenAsset(selected);
            }
        }
    }
}