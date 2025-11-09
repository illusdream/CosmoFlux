using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public static class ModularTextureHelper
    {
        [MenuItem("Tools/生成模型对应的图片")]
        private static void CaptureAndSave()
    {
        int captureWidth = 720;
        int captureHeight = 405;
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var targetCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        var targetObject = GameObject.Find("Modular");

        var StructtfolderPath = "Assets/Assets/ShipModular/Struct";
        var CoreFolderPath = "Assets/Assets/ShipModular/Core";
        var EngineFolderPath = "Assets/Assets/ShipModular/Engine";
        var EnergyFolderPath = "Assets/Assets/ShipModular/Energy";

// 确保保存目录存在


// 遍历预制体
        var alpha =  PlayerSettings.preserveFramebufferAlpha;
       // PlayerSettings.preserveFramebufferAlpha = true;
        string[] guids = AssetDatabase.FindAssets("", new[] { StructtfolderPath,CoreFolderPath,EngineFolderPath,EnergyFolderPath });
        foreach (string guid in guids)
        {
            // 清空目标对象的所有子对象
            while (targetObject.transform.childCount > 0)
            {
                Transform child = targetObject.transform.GetChild(0);
                Undo.DestroyObjectImmediate(child.gameObject);
            }

            var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (!prefab)
            {
                continue;
            }

            // 实例化预制体
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.Euler(22.5f, -135, 22.5f), targetObject.transform);

            // 获取边界
            var bound = targetObject.GetComponentInChildren<MeshRenderer>().bounds;
            
            // 创建支持透明的RenderTexture
            RenderTexture renderTexture = RenderTexture.GetTemporary(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
            // 设置相机
            targetCamera.orthographicSize = bound.size.y / 2f;
            targetCamera.targetTexture = renderTexture;
            //targetCamera.clearFlags = CameraClearFlags.SolidColor;
           // targetCamera.backgroundColor = new Color(0, 0, 0, 0); // 完全透明背景

            // 渲染
            targetCamera.Render();

            // 将RenderTexture转换为Texture2D
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            texture.Apply();

            // 重置相机设置
            targetCamera.targetTexture = null;
            RenderTexture.active = null;

            // 编码为PNG
            byte[] bytes = texture.EncodeToPNG();

            // 保存文件
            var savePath = prefabPath.Replace( Path.GetFileName(prefabPath),"");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string filename = Path.GetFileNameWithoutExtension(prefabPath) +"Preview" + ".png";
            string path = Path.Combine(savePath, filename);
            File.WriteAllBytes(path, bytes);

            // 清理资源
            Object.DestroyImmediate(texture);
            Object.DestroyImmediate(renderTexture);

            Debug.Log("Capture saved to: " + path);
        }

// 刷新AssetDatabase
        AssetDatabase.Refresh();
        PlayerSettings.preserveFramebufferAlpha = alpha;
    }
    }
}