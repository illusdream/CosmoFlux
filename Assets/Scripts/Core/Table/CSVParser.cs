using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ilsFramework.Core
{
    public class CSVParser
    {
        public static List<TableItem> ParseCSV(string filePath, Type itemType)
        {
            var items = new List<TableItem>();
            var lines = File.ReadAllLines(filePath);

            //标题行 -解释行 - 数据定义行 -数据行
            if (lines.Length < 4) // 至少需要有标题行和一行数据
                return items;

            // 获取标题行（列名）
            string[] headers = ParseCSVLine(lines[0]);

            // 创建字段映射字典（字段名 -> FieldInfo）
            var fieldMap = new Dictionary<string, FieldInfo>();
            var fields = itemType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                fieldMap[field.Name] = field;
            }

            // 处理数据行
            for (int i = 3; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                string[] values = ParseCSVLine(lines[i]);
                TableItem item = (TableItem)Activator.CreateInstance(itemType);

                for (int j = 0; j < Mathf.Min(headers.Length, values.Length); j++)
                {
                    string header = headers[j].Trim();

                    if (fieldMap.ContainsKey(header))
                    {
                        FieldInfo field = fieldMap[header];
                        SetFieldValue(field, item, values[j]);
                    }
                }

                items.Add(item);
            }

            return items;
        }

        // 解析CSV行（处理逗号在引号内的情况）
        private static string[] ParseCSVLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            int startIndex = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    values.Add(ProcessValue(line.Substring(startIndex, i - startIndex)));
                    startIndex = i + 1;
                }
            }

            // 添加最后一个值
            values.Add(ProcessValue(line.Substring(startIndex)));

            return values.ToArray();
        }

        // 处理CSV值（去除引号和解码）
        private static string ProcessValue(string value)
        {
            value = value.Trim();

            // 处理被引号包围的值
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
                value = value.Replace("\"\"", "\""); // 处理双引号转义
            }

            return value;
        }

        // 设置字段值（支持基本数据类型）
        private static void SetFieldValue(FieldInfo field, object obj, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            try
            {
                Type fieldType = field.FieldType;

                if (fieldType == typeof(string))
                {
                    field.SetValue(obj, value);
                }
                else if (fieldType == typeof(int))
                {
                    field.SetValue(obj, int.Parse(value));
                }
                else if (fieldType == typeof(float))
                {
                    field.SetValue(obj, float.Parse(value));
                }
                else if (fieldType == typeof(bool))
                {
                    field.SetValue(obj, bool.Parse(value));
                }
                else if (fieldType.IsEnum)
                {
                    field.SetValue(obj, Enum.Parse(fieldType, value));
                }
                else if (fieldType == typeof(uint))
                {
                    field.SetValue(obj, uint.Parse(value));
                }
                // 可以添加更多类型的支持...
                else
                {
                    Debug.LogWarning($"不支持的类型: {fieldType.Name} 用于字段 {field.Name}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"设置字段 {field.Name} 值时出错: {e.Message}");
            }
        }
    }
}
