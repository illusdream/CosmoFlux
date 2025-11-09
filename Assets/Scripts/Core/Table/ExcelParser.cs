using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using OfficeOpenXml;
using UnityEngine;

namespace ilsFramework.Core
{
    public class ExcelParser
    {
        public static List<TableItem> ParseExcel(ExcelWorksheet sheet, Type itemType)
        {
            var result = new List<TableItem>();
            if (sheet.Dimension.End.Row < 4) // 至少需要有标题行和一行数据
                return result;
            //行
            int rowCount = sheet.Dimension.End.Row;
            //列
            int columnCount = sheet.Dimension.End.Column;
            
            Dictionary<string,int> columnNameToSheetIndex = new Dictionary<string, int>();
            
            var fieldMap = new Dictionary<string, FieldInfo>();
            var fields = itemType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                fieldMap[field.Name] = field;
            }
            
            //构建名称到Index的映射

            for (int i = 4; i <= rowCount; i++)
            {
                
                TableItem item = (TableItem)Activator.CreateInstance(itemType);
                
                for (int j = 1; j <=columnCount; j++)
                {
                    var header = sheet.Cells[1,j].Value;
                    if (header is string _header && fieldMap.ContainsKey(_header))
                    {
                        //_header.LogSelf();
                        FieldInfo field = fieldMap[_header];
                        SetFieldValue(field, item, sheet.Cells[i,j].Value);
                    }
                }

                result.Add(item);
            }
            
            return result;
        }
        // 设置字段值（支持基本数据类型）
        private static void SetFieldValue(FieldInfo field, object obj, object value)
        {
            if (value is null)
                return;

            try
            {
                Type fieldType = field.FieldType;
                
                if (fieldType == typeof(string) && value is string)
                {
                    field.SetValue(obj, value);
                }
                else if (fieldType == typeof(int))
                {
                    field.SetValue(obj,Convert.ToInt32(value));
                }
                else if (fieldType == typeof(float))
                {
                    field.SetValue(obj,Convert.ToSingle(value));
                }
                else if (fieldType == typeof(bool))
                {
                    field.SetValue(obj,Convert.ToBoolean(value));
                }
                else if (fieldType.IsEnum)
                {
                    value.LogSelf();
                    field.SetValue(obj,Convert.ToInt32(value));
                }
                else if (fieldType == typeof(uint))
                {
                    field.SetValue(obj,Convert.ToUInt32(value));
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