using System;
using System.Collections;
using YooAsset;

namespace ilsFramework.Core
{
    /// <summary>
    /// 将一整个LoadPackage的过程打包到这里面
    /// </summary>
    public class LoadPackageOperation : IEnumerator
    {
        private IEnumerator LoadPackageEnumerator;
        
        public bool IsLoaded { get; private set; } = false;
        
        public ResourcePackage Package { get; private set; }

        public LoadPackageOperation(Func<string,Action<ResourcePackage>,IEnumerator> loadFunc,string packageName)
        {
            LoadPackageEnumerator = loadFunc.Invoke(packageName, (package) =>
            {
                Package = package;
            });
        }

        public bool MoveNext()
        {
            while (LoadPackageEnumerator.MoveNext())
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("不支持重置操作");;
        }

        public object Current => null;
    }
}