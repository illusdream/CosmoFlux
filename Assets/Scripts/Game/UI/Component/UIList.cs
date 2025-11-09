using System;
using System.Collections;
using System.Collections.Generic;
using ilsFramework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    /// <summary>
    /// 相当于一个简易的List，用来快速添加一个模板物体，而不用额外加载资源，同时方便删除
    /// </summary>
    [UIBinder("list")]
    public class UIList : MonoBehaviour,IEnumerable<UIItem>
    {
        public Transform content;

        public GameObject TempleteGameObject;

        public Action<GameObject, Dictionary<string, Component>> MapBuildAction;
        
        public List<UIItem> items = new List<UIItem>();
        
        private Dictionary<Transform, UIItem> map = new Dictionary<Transform, UIItem>();
        public void Reset()
        {
            content = transform;
        }

        public UIItem Insert(int index)
        {
            if (!CheckUIListValidity())
                return null;
            var instance = CreateUIItem();
            items.Insert(index, instance);
            return instance;
        }

        public UIItem AddFront()
        {
            if (!CheckUIListValidity())
                return null;
            var instance = CreateUIItem();
            items.Insert(0,instance);
            return instance;
            return null;
        }

        public UIItem AddItem()
        {
            if (!CheckUIListValidity())
                return null;
            var instance = CreateUIItem();
            items.Add(instance);
            return instance;
        }

        public void RemoveItem(UIItem item)
        {
            if (!CheckUIListValidity())
                return;
            if (items.Remove(item))
            {
                Object.Destroy(item.gameObject);
            }
        }

        public void RemoveItemByTransform(Transform item)
        {
            if (!CheckUIListValidity())
            {
                return;
            }

            if (map.TryGetValue(item,out var value))
            {
                RemoveItem(value);
            }
        }

        public void RemoveBack()
        {
            if (!CheckUIListValidity())
                return;
            Object.Destroy(items[^1].gameObject);
            items.RemoveAt(items.Count - 1);
        }

        public void Clear()
        {
            foreach (var uiItem in items)
            {
                Object.Destroy(uiItem.gameObject);
            }
        }

        public bool CheckUIListValidity()
        {
            if (!TempleteGameObject)
            {
                $"没有初始化{nameof(TempleteGameObject)}".ErrorSelf();
                return false;
            }
            return true;
        }

        public GameObject GetUIItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return items[index].gameObject;
        }

        private UIItem CreateUIItem()
        {
            var obj = Object.Instantiate(TempleteGameObject);
            var instance = new UIItem(obj, MapBuildAction);
            map[obj.transform] = instance;
            return instance;
        }

        public IEnumerator<UIItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public UIItem this [int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new IndexOutOfRangeException();
                return items[index];
            }
        }
    }

    /// <summary>
    /// 简易的UI组件集合
    /// </summary>
    public class UIItem
    {
        public Transform transform { get;private set; }
        
        public GameObject gameObject { get;private set; }

        public Dictionary<string,Component> components { get;private set; }

        public UIItem(GameObject gameObject, Action<GameObject, Dictionary<string, Component>> temp = null)
        {
            this.gameObject = gameObject;
            this.transform = gameObject.GetComponent<Transform>();
            components = new Dictionary<string, Component>();
            temp?.Invoke(gameObject, components);
        }

        public T GetComponent<T>(string name) where T : Component
        {
            return (T)components[name];
        }
    }
}