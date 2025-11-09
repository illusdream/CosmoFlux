using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core.Editor
{
    //配置表生成工具
    public class TableGenerator
    {
        //需要先配置文件夹
        [MenuItem("ilsFramework/更新配置表",priority = 10)]
        public static void GenerateAllTable()
        {
            string TablePath = Config.GetConfigInEditor<TableConfig>().TargetFolder;
            string pathWithoutAsset = TablePath.Remove(0,6);
            FileUtils.AssetFolder_CheckOrCreateFolder(TablePath);
            List<string> csvFiles = new List<string>();
            
            // 获取所有CSV文件
            string[] files = Directory.GetFiles(TablePath, "*.xlsx", SearchOption.AllDirectories);
            csvFiles.AddRange(files);

            foreach (var csvFile in csvFiles)
            {
                AssetDatabase.ImportAsset(csvFile);
            }
        }

        public static void GenerateTableExcel()
        {
            var list = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && typeof(TableItem).IsAssignableFrom(type))
                    {
                        list.Add(type);   
                    }
                }
            }

            foreach (var type in list)
            {
                string TablePath = Config.GetConfigInEditor<TableConfig>().TargetFolder;
                string pathWithoutAsset = TablePath.Remove(0,6);
                var fileName = type.Name + ".xlsx";
            }
        }
    }
}