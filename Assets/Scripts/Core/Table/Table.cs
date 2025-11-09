using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    public abstract class BaseTable
    {
        public abstract void InitData(TableAsset tableAsset);
    }
    
    /// <summary>
    /// 运行时数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Table<T> : BaseTable where T : TableItem
    {
        [ShowInInspector]
        private Dictionary<uint, T> _mainDictionary;
        [ShowInInspector]
        private Dictionary<string,BaseIndexCollection> _extraIndexDictionary;
        
        public static Table<T> Instance { get; private set; }
        
        public override void InitData(TableAsset tableAsset)
        {
            _mainDictionary = new Dictionary<uint, T>();
            _extraIndexDictionary = new Dictionary<string, BaseIndexCollection>();
            
            Dictionary<string,FieldInfo> indexFieldInfos = new Dictionary<string, FieldInfo>();
            foreach (var field in typeof(T).GetFields())    
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
                _extraIndexDictionary.Add(index.Key,instance as BaseIndexCollection);
            }
            
            foreach (var item in tableAsset)
            {
                if (item is T _item)
                {
                    _mainDictionary.Add(item.ID, _item);

                    foreach (var info in indexFieldInfos)
                    {
                        _extraIndexDictionary[info.Key].AddItem(info.Value.GetValue(item),item.ID);
                    }
                }
            }
            
            OnInitData(tableAsset);
        }

        public virtual void OnInitData(TableAsset tableAsset)
        {

        }
        
        
        //static

        public static int Count()
        {
            return Instance._mainDictionary.Count;
        }

        public static T Get(uint id)
        {
            if (Instance._mainDictionary.TryGetValue(id, out T item))
            {
                return item;
            }
            $"Table:{typeof(T).Name}  ID:{id} 不存在".WarningSelf();
            return null;
        }

        public static bool TryGet(uint id, out T item)
        {
            return Instance._mainDictionary.TryGetValue(id, out item);
        }

        public static List<T> GetByExtraIndex<TIndex>(string extraIndexName, TIndex index)
        {
            List<T> result = new List<T>();
            if (Instance._extraIndexDictionary.TryGetValue(extraIndexName, out BaseIndexCollection collection) && collection is IndexCollection<TIndex> _indexCollection)
            {
                var ids = _indexCollection.GetResult(index);
                foreach (var id in ids)
                {
                    if (TryGet(id,out var item))
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
            return result;
            
        }
    }
}