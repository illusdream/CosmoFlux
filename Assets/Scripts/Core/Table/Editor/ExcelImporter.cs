using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ilsFramework.Core.Editor
{
    [ScriptedImporter(1, new string[] { "xlsx" }, new string[] {  })]
    public class ExcelImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            
        }
        

    }

    [CustomEditor(typeof(ExcelImporter))]
    public class ExcelImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            ApplyRevertGUI();
        }
    }

    public class ExcelProcessor : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                if (!importedAsset.EndsWith(".xlsx"))
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
                using (ExcelPackage package = new ExcelPackage(new FileInfo(importedAsset)))
                {
                    var sheet = package.Workbook.Worksheets[1];
                    var list = ExcelParser.ParseExcel(sheet,targetTableItemType);
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
}