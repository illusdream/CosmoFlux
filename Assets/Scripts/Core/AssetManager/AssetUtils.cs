using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    public static class Asset
    {
        private static IEnumerator InitPackage(string packageName,Action<ResourcePackage> callback)
        {
            return AssetManager.Instance.InitPackage(packageName, callback);
        }

        public static LoadPackageOperation InitPackage(string packageName)
        {
            return AssetManager.Instance.InitPackage(packageName);
        }
        
        //加载不使用原版协程，用UniTask代替,暂时就异步加载的方法，同步加载好像没那么好
        public static void LoadResourceAsync<T>(string packageName, string resourceLocation,LoadResourcePriority priority, Action<AssetHandle> callback) where T : UnityEngine.Object
        {
            AssetManager.Instance.LoadResourceAsync<T>(packageName, resourceLocation, priority, callback);
        }

        public static LoadResourceOperation<T> LoadResourceAsync<T>(string packageName, string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            return AssetManager.Instance.LoadResourceAsync<T>(packageName, resourceLocation, priority);
        }
        
        public static AssetHandle LoadResource<T>(string packageName, string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            return AssetManager.Instance.LoadResource<T>(packageName, resourceLocation, priority);
        } 
        
        public static void LoadResourceAsync<T>(string resourceLocation,LoadResourcePriority priority, Action<AssetHandle> callback) where T : UnityEngine.Object
        {
            AssetManager.Instance.LoadResourceAsync<T>(AssetManager.DefaultPackageName, resourceLocation, priority, callback);
        }
        
        public static LoadResourceOperation<T> LoadResourceAsync<T>(string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            return AssetManager.Instance.LoadResourceAsync<T>(AssetManager.DefaultPackageName, resourceLocation, priority);
        }
        
        public static AssetHandle LoadResource<T>(string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            return AssetManager.Instance.LoadResource<T>(AssetManager.DefaultPackageName, resourceLocation, priority);
        } 
        
        
        public static void LoadResourceByTagAsync<T>(string packageName, string tag, LoadResourcePriority priority, Action<AssetHandle> callback)where T : UnityEngine.Object
        {
            AssetManager.Instance.LoadResourceByTagAsync<T>(packageName, tag, priority, callback);
        }

        public static List<IEnumerator> LoadResourceByTagAsync<T>(string packageName, string tag, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            return AssetManager.Instance.LoadResourceByTagAsync<T>(packageName, tag, priority);
        }
    }
}