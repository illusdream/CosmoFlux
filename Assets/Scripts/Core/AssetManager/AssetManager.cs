using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Profiling;
using YooAsset;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    //重构这坨东西，EditorUI由YooAsset负责，
    public class AssetManager : ManagerSingleton<AssetManager>
    {
        /// <summary>
        /// 基础资源包，最基本的资源放在这里面
        /// </summary>
        public const string DefaultPackageName = "Base";
        
        public ResourcePackage BaseResourcePackage { get; private set; }

        public const string PackageCurrentReleaseVersion = "1.0.0";
        [ShowInInspector]
        private Dictionary<string, ResourcePackage> packages;
        
        public override IEnumerator OnInit()
        {
            packages = new Dictionary<string, ResourcePackage>();
            YooAssets.Initialize();
            yield return InitPackage(DefaultPackageName, (package) =>
            {
                BaseResourcePackage = package;
            });
            yield return InitPackage("ShipModular", (package) =>
            {
                BaseResourcePackage = package;
            });
        }
        
        public IEnumerator InitPackage(string packageName,Action<ResourcePackage> callback)
        {
            var package = YooAssets.CreatePackage(packageName);
            string correctUpdateVersionName;
#if UNITY_EDITOR
            var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;
            correctUpdateVersionName = "Simulate";
#else
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            correctUpdateVersionName = PackageCurrentReleaseVersion;
#endif
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
            while (!initOperation.IsDone)
            {
                yield return null;
            }
            if (!CheckAndLogInitPackageInfo(initOperation))
            {
                yield break;
            }

            var requestPackageVersionAsync = package.RequestPackageVersionAsync();
            yield return requestPackageVersionAsync;            
            while (!requestPackageVersionAsync.IsDone)
            {
                yield return null;
            }
            if (!CheckAndLogRequestPackageInfo(requestPackageVersionAsync))
            {
                yield break;
            }

            //传入的版本信息更新资源清单
            var updatePackageManifestAsync = package.UpdatePackageManifestAsync(correctUpdateVersionName);
            yield return updatePackageManifestAsync;
            while (!updatePackageManifestAsync.IsDone)
            {
                yield return null;
            }
            if (!CheckAndLogUpdatePackageManifestInfo(updatePackageManifestAsync))
            {
                yield break;
            }
            packages[packageName] = package;
            callback?.Invoke(package);
        }

        public LoadPackageOperation InitPackage(string packageName)
        {
            return new LoadPackageOperation(InitPackage,packageName);
        }

        private bool CheckAndLogInitPackageInfo(InitializationOperation operation)
        {
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"资源包:{operation.PackageName}初始化成功！");
                return true;
            }
            else
            {
                Debug.LogError($"资源包:{operation.PackageName}初始化失败：{operation.Error}");
                return false;
            }
        }
        
        private bool CheckAndLogRequestPackageInfo(RequestPackageVersionOperation operation)
        {
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"资源包:{operation.PackageName}获取版本成功！");
                return true;
            }
            else
            {
                Debug.LogError($"资源包:{operation.PackageName}获取版本失败：{operation.Error}");
                return false;
            }
        }
        
        private bool CheckAndLogUpdatePackageManifestInfo(UpdatePackageManifestOperation operation)
        {
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"资源包:{operation.PackageName}更新资源清单成功！");
                return true;
            }
            else
            {
                Debug.LogError($"资源包:{operation.PackageName}更新资源清单失败：{operation.Error}");
                return false;
            }
        } 
        
        public override void OnUpdate()
        {

        }
        
        public override void OnLateUpdate()
        {
            
        }

        public override void OnLogicUpdate()
        {
            CheckMemoryUsed();
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnDestroy()
        {
            
        }

        public override void OnDrawGizmos()
        {
           
        }

        public override void OnDrawGizmosSelected()
        {
            
        }

        private void CheckTargetPackage(string packageName, Action<ResourcePackage> callback)
        {
            if (packages.TryGetValue(packageName,out var value))
            {
                callback?.Invoke(value);
            }
            else
            {
                MonoManager.Instance.StartCoroutine(InitPackage(packageName, callback));

            }
        }
        //加载不使用原版协程，用UniTask代替,暂时就异步加载的方法，同步加载好像没那么好
        public void LoadResourceAsync<T>(string packageName, string resourceLocation,LoadResourcePriority priority, Action<AssetHandle> callback) where T : UnityEngine.Object
        {
            CheckTargetPackage(packageName, package =>
            {
                var assetHandle = package.LoadAssetAsync<T>(resourceLocation,priority:(uint)priority);
                assetHandle.Completed += (assetHandle) =>
                {
                    if (assetHandle.Status == EOperationStatus.Succeed)
                    {
                        callback?.Invoke(assetHandle);
                    }
                };
            });
        }

        public void LoadResourceByTagAsync<T>(string packageName, string tag, LoadResourcePriority priority, Action<AssetHandle> callback)where T : UnityEngine.Object
        {
            CheckTargetPackage(packageName, package =>
            {
                foreach (var info in package.GetAssetInfos(tag))
                {
                    var assetHandle = package.LoadAssetAsync<T>(info.Address,priority:(uint)priority);
                    assetHandle.Completed += (assetHandle) =>
                    {
                        if (assetHandle.Status == EOperationStatus.Succeed)
                        {
                            callback?.Invoke(assetHandle);
                        }
                    };
                }
            });
        }


        
        public LoadResourceOperation<T> LoadResourceAsync<T>(string packageName, string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            if (packages.TryGetValue(packageName,out var value))
            {
                return new LoadResourceOperation<T>(value, resourceLocation, priority);
            }
            return new LoadResourceOperation<T>(InitPackage(packageName), resourceLocation, priority);;
        }
        public List<IEnumerator> LoadResourceByTagAsync<T>(string packageName, string tag, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            var result = new List<IEnumerator>();
            if (!packages.TryGetValue(packageName,out var value))
            {
                result.Add(InitPackage(packageName));
            }
            foreach (var info in value.GetAssetInfos(tag))
            {
                result.Add(new LoadResourceOperation<T>(value, info.Address, priority));
            }
            return result;
        }

        public AssetHandle LoadResource<T>(string packageName, string resourceLocation, LoadResourcePriority priority) where T : UnityEngine.Object
        {
            if (!packages.TryGetValue(packageName, out var value))
            {
                Debug.LogError($"资源包:{packageName}未加载,或正在异步加载中，最好使用异步方法");
                return null;
            }
            return value.LoadAssetSync<T>(resourceLocation);
        }

        /// <summary>
        /// 检测内存占用，并在合适的时机执行清理无用资源
        /// </summary>
        private void CheckMemoryUsed()
        {
           // Debug.Log(Profiler.GetTotalAllocatedMemoryLong()/ (1024.0 * 1024.0));
        }
    }
}