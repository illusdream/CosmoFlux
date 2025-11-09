using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ilsFramework.Core
{
    public class baseAsset : ScriptableObject
    {
        
    }
    
    public class TableAsset : baseAsset
    {
        [SerializeReference]
        public List<TableItem> items = new List<TableItem>();

        //预处理所有的索引数据
       // [SerializeReference]
       // public List<BaseIndexCollection> indexes = new List<BaseIndexCollection>();
        
        /// <summary>
        /// 额外的索引存储
        /// </summary>
        public List<string> ExtraIndexes;
        public BaseTable CreateRunTimeTable()
        {
            //生成实际的类型
            var typeItem = items[0].GetType();
            var tableType = typeof(Table<>).MakeGenericType(typeItem);
            var instance = (BaseTable)Activator.CreateInstance(tableType);
            tableType.GetMethod("InitData").Invoke(instance,new object[] {this });
            tableType.GetProperty("Instance").SetValue(instance,instance);
            return instance;
        }

        public IEnumerator<TableItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public void InitData(List<TableItem> items)
        {
            //构建索引表
            Dictionary<string,FieldInfo> indexFieldInfos = new Dictionary<string, FieldInfo>();
            Dictionary<string,BaseIndexCollection> indexesBuffer = new Dictionary<string, BaseIndexCollection>();
            foreach (var field in items[0].GetType().GetFields())    
            {
                if (field.IsDefined(typeof(IndexAttribute)) && (field.FieldType.IsValueType || field.FieldType == typeof(string)) )
                {
                    indexFieldInfos.Add(field.Name, field);
                }
            }

            foreach (var index in indexFieldInfos)
            {
                var collectionType = typeof(IndexCollection<>);
                var cType = collectionType.MakeGenericType(index.Value.FieldType);
                var instance = Activator.CreateInstance(cType);
                (instance as BaseIndexCollection).IndexName = index.Key;
                indexesBuffer.Add(index.Key,instance as BaseIndexCollection);
            }

            foreach (var item in items)
            {
                this.items.Add(item);
            }
        }

        public void Add(TableItem item)
        {
           items.Add(item);
        }

        public void Clear()
        {
            
            items.Clear();
        }

        public bool Contains(TableItem item)
        {
            return items.Contains(item);
        }

        public void CopyTo(TableItem[] array, int arrayIndex)
        {
           items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TableItem item)
        {
           return items.Remove(item);
        }

        public int Count =>items.Count;
        public bool IsReadOnly => false;
    }
}