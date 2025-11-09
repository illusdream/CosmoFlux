using System;
using System.Collections;
using System.Resources;
using UnityEngine;
using YooAsset;

namespace ilsFramework.Core
{
    public class LoadResourceOperation<T> : IEnumerator where T : UnityEngine.Object
    {
        private enum LoadState
        {
            CheckPackage,
            InitializingPackage,
            LoadingAsset,
            Completed,
            Failed
        }

        private LoadState _currentState = LoadState.CheckPackage;
        private readonly LoadResourcePriority _priority;
        private ResourcePackage _package;
        private AssetHandle _assetHandle;
        private System.Action<AssetHandle> _onComplete;
        private System.Exception _exception;
        private LoadPackageOperation _loadPackageOperation;
        private string _location;
        //需要先加载包,然后再加载资源
        public LoadResourceOperation(LoadPackageOperation loadPackageOperation,string location,LoadResourcePriority priority)
        {
            _currentState = LoadState.InitializingPackage;
            _priority = priority;
            _loadPackageOperation = loadPackageOperation;
            _location = location;
        }
        //已经由包了
        public LoadResourceOperation(ResourcePackage package,string location,LoadResourcePriority priority)
        {
            _currentState = LoadState.LoadingAsset;
            _priority = priority;
            _package = package;
            _location = location;
        }

        // IEnumerator 接口实现
        public object Current => null;

        public bool MoveNext()
        {
            try
            {
                switch (_currentState)
                {
                    //直接跳转到加载包
                    case LoadState.CheckPackage:
                        return true;
                    case LoadState.InitializingPackage:
                        while (_loadPackageOperation.MoveNext())
                        {
                            return true;
                        }
                        _package = _loadPackageOperation.Package;
                        _currentState = LoadState.LoadingAsset;
                        return true;
                    case LoadState.LoadingAsset:
                        // 开始加载资源
                        _assetHandle = _package.LoadAssetAsync<T>(_location, priority: (uint)_priority);
                        // 等待资源加载完成
                        while (!_assetHandle.IsDone)
                        {
                            // 返回 true 表示需要继续执行
                            return true;
                        } 
                        // 资源加载完成
                        _currentState = _assetHandle.Status == EOperationStatus.Succeed
                            ? LoadState.Completed
                            : LoadState.Failed;
                        // 调用完成回调
                        _onComplete?.Invoke(_assetHandle);
                        return false; // 加载完成，不再继续
                    default:
                        return false; // 其他状态不再继续
                }
            }
            catch (System.Exception ex)
            {
                _exception = ex;
                _currentState = LoadState.Failed;
                Debug.LogError($"资源加载异常: {ex},发生阶段：{_currentState}");
                return false;
            }
        }

        public void Reset()
        {
            throw new System.NotSupportedException("不支持重置操作");
        }

        // 获取加载结果
        public AssetHandle GetResult()
        {
            if (_currentState == LoadState.Completed)
            {
                return _assetHandle;
            }

            if (_currentState == LoadState.Failed && _exception != null)
            {
                throw _exception;
            }

            throw new System.InvalidOperationException("资源加载尚未完成");
        }
        // 检查是否完成
        public bool IsDone => _currentState == LoadState.Completed || _currentState == LoadState.Failed;

        // 检查是否成功
        public bool IsSuccess => _currentState == LoadState.Completed;
    }
}
