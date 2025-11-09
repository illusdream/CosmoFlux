using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework.Core.Editor
{
    [ScriptedImporter(1, new string[] { }, new string[] { "csv" })]
    public class CSVImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
           
        }
    }
    [CustomEditor(typeof(CSVImporter))]
    public class CSVImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            ApplyRevertGUI();
        }
    }
    /// <summary>
    /// 已弃用
    /// </summary>
    public class CSVProsser : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            return;
            foreach (var importedAsset in importedAssets)
            {
                if (!importedAsset.EndsWith(".csv"))
                    continue;
                Type targetTableItemType = null;
                var name = Path.GetFileNameWithoutExtension(importedAsset);
                var path = Path.GetDirectoryName(importedAsset);
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (targetTableItemType != null)
                    {
                        break;
                    }
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.Name == name && typeof(TableItem).IsAssignableFrom(type))
                        {
                            targetTableItemType = type;
                            break;
                        }
                    }
                }
                
                TableAsset instance = ScriptableObject.CreateInstance<TableAsset>();
                var list = CSVParser.ParseCSV(importedAsset,targetTableItemType);
                if (!list.Any())
                {
                    return;
                }
                instance.InitData(list);
                AssetDatabase.CreateAsset(instance, $"{path}/{name}.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }
}