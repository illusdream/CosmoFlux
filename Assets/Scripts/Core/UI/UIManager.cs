using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    public class UIManager : ManagerSingleton<UIManager>
    {
        GameObject UIRoot;
            
        GameObject Bottom;
        GameObject Lower;
        GameObject Normal;
        GameObject Upper;
        GameObject Top;
        GameObject Debug;
        
        /// <summary>
        ///     UI层级间的SortOrder间隔
        /// </summary>
        private readonly int _UILayerInterval = 100;
        
        private GameObject eventHandler;
        
        
        [ShowInInspector] private Dictionary<Type, UIView> uiViews;
        private Dictionary<Type,AssetHandle> uiViewHandles;
        
        private Dictionary<Type,UIController> uiControllers;
        
        private UIConfig uiConfig;

        public override IEnumerator OnInit()
        {
            uiViews = new Dictionary<Type, UIView>();
            uiViewHandles = new Dictionary<Type, AssetHandle>();
            uiControllers = new Dictionary<Type, UIController>();
            
            uiConfig = Config.GetConfig<UIConfig>();
            InitUIBaseFramework();
            yield return null;
        }

        public override void OnUpdate()
        {
            foreach (var controller in uiControllers.Values)
            {
#if UNITY_EDITOR
                try { controller.OnUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.Update();
#endif
            }
            
            
            foreach (var view in uiViews.Values)
            {
#if UNITY_EDITOR
                try { view.OnUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.Update();
#endif
            }
        }
        public override void OnLateUpdate()
        {
            foreach (var controller in uiControllers.Values)
            {
#if UNITY_EDITOR
                try { controller.OnLateUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.Update();
#endif
            }
            foreach (var view in uiViews.Values)
            {
#if UNITY_EDITOR
                try { view.OnLateUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.LateUpdate();
#endif
            }
        }
        public override void OnLogicUpdate()
        {
            foreach (var controller in uiControllers.Values)
            {
#if UNITY_EDITOR
                try { controller.OnLogicUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.Update();
#endif
            }
        }
        public override void OnFixedUpdate()
        {
            foreach (var controller in uiControllers.Values)
            {
#if UNITY_EDITOR
                try { controller.OnLogicUpdate(); }
                catch (Exception e) { Console.WriteLine(e); throw; }
#else
                view.Update();
#endif
            }
        }
        public override void OnDestroy()
        {
            foreach (var uiPanel in uiViews)
            {
                uiPanel.Value.Destroy();
                GameObject.Destroy(uiPanel.Value.UIPanelObject);
            }
            uiViews.Clear();
            foreach (var controller in uiControllers)
            {
                controller.Value.OnDestroy();
            }
            uiControllers.Clear();
        }

        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
            
        }

        public void InitUIBaseFramework()
        {
            UIRoot = new GameObject("UIRoot");
            UIRoot.layer = LayerMask.NameToLayer("UI");
            UIRoot.transform.parent = ContainerObject.transform;

            Bottom = new GameObject("Bottom");
            Bottom.layer = LayerMask.NameToLayer("UI");
            Bottom.transform.parent = UIRoot.transform;

            Lower = new GameObject("Lower");
            Lower.layer = LayerMask.NameToLayer("UI");
            Lower.transform.parent = UIRoot.transform;

            Normal = new GameObject("Normal");
            Normal.layer = LayerMask.NameToLayer("UI");
            Normal.transform.parent = UIRoot.transform;

            Upper = new GameObject("Upper");
            Upper.layer = LayerMask.NameToLayer("UI");
            Upper.transform.parent = UIRoot.transform;

            Top = new GameObject("Top");
            Top.layer = LayerMask.NameToLayer("UI");
            Top.transform.parent = UIRoot.transform;

            Debug = new GameObject("Debug");
            Debug.layer = LayerMask.NameToLayer("UI");
            Debug.transform.parent = UIRoot.transform;

            if (uiConfig.UIEventHandler)
            {
                eventHandler = Object.Instantiate(uiConfig.UIEventHandler, ContainerObject.transform);
            }
            else
            {
                $"UI EventHandler预制体未注册，请在Config中填写".ErrorSelf();
            }
        }

        public (Transform, int) GetUILayerInfo(EUILayer layer)
        {
            switch (layer)
            {
                case EUILayer.Bottom:
                    return (Bottom.transform, (int)EUILayer.Bottom * _UILayerInterval);
                case EUILayer.Lower:
                    return (Lower.transform, (int)EUILayer.Lower * _UILayerInterval);
                case EUILayer.Normal:
                    return (Normal.transform, (int)EUILayer.Normal * _UILayerInterval);
                case EUILayer.Upper:
                    return (Upper.transform, (int)EUILayer.Upper * _UILayerInterval);
                case EUILayer.Top:
                    return (Top.transform, (int)EUILayer.Top * _UILayerInterval);
                case EUILayer.Debug:
                    return (Debug.transform, (int)EUILayer.Debug * _UILayerInterval);
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }

        public void LoadUIViewAsync<T>(Action<T> callback) where T : UIView
        {
            var type = typeof(T);
            if (uiViews.TryGetValue(type, out var value))
            {
                $"View{nameof(value)} 已加载过".WarningSelf();
                callback?.Invoke((T)value);
            }
            else
            {
                var uiViewInstance = Activator.CreateInstance<T>();
                Asset.LoadResourceAsync<GameObject>(uiViewInstance.PackageName, uiViewInstance.UIPath, LoadResourcePriority.UI, (handle =>
                {
                    var layerInfo = GetUILayerInfo(uiViewInstance.UILayer);
                    var operation= handle.InstantiateAsync(layerInfo.Item1);
                    operation.Completed += (o) =>
                    {
                        uiViewInstance.UIPanelObject = operation.Result;
                        uiViewInstance.Canvas = uiViewInstance.UIPanelObject.GetComponent<Canvas>();
                        uiViewInstance.UIPanelCanvasGroup = uiViewInstance.UIPanelObject.GetComponent<CanvasGroup>();
                        var cOffest = uiViewInstance.LayerOffest + layerInfo.Item2;
                        uiViewInstance.Canvas.sortingOrder = cOffest;

                        uiViewInstance.OnLoad();
                        uiViewInstance.InitUIPanel();
                        uiViews[type] = uiViewInstance;
                        uiViewHandles[type] = handle;
                        callback?.Invoke(uiViewInstance);
                    };
                }));
            }
            
        }

        public void GetUIPanelAsync<T>(Action<T> callback) where T : UIView
        {
            if (uiViews.TryGetValue(typeof(T), out var result) && result is T view)
            {
                callback?.Invoke(view);
            }
            else
            {
                LoadUIViewAsync(callback);
            }
        }

        public void UnLoadUIView<T>() where T : UIView
        {
            if (uiViews.ContainsKey(typeof(T)))
            {
                var cur = uiViews[typeof(T)];
                GameObject.Destroy(cur.UIPanelObject);
                if (uiViewHandles.TryGetValue(typeof(T),out var value))
                {
                    value.Release();
                }
                uiViews.Remove(typeof(T));
            }
        }

        public T GetUIController<T>() where T : UIController
        {
            if (uiControllers.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
            }
            else
            {
                var uiControllerInstance = Activator.CreateInstance<T>();
                uiControllerInstance.OnInitialize();
                uiControllers[typeof(T)] = uiControllerInstance;
                return uiControllerInstance;
            }
        }

    }
}