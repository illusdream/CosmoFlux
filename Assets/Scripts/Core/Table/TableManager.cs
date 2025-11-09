using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    public class TableManager : ManagerSingleton<TableManager>
    {
        [ShowInInspector]
        Dictionary<Type,BaseTable> _tables = new Dictionary<Type, BaseTable>();
        public override IEnumerator OnInit()
        {
            //找到对应配置表文件夹，然后加载
            foreach (var iEnumerator in Asset.LoadResourceByTagAsync<ScriptableObject>(AssetManager.DefaultPackageName,"Table",LoadResourcePriority.Default))
            {
                yield return iEnumerator;
                if (iEnumerator is LoadResourceOperation<ScriptableObject> operation)
                {
                    while (!operation.IsDone)
                    {
                        yield return null;
                    }
                    var asset = operation.GetResult().GetAssetObject<TableAsset>();

                    var tableInstance = asset.CreateRunTimeTable();
                    _tables.Add(tableInstance.GetType(), tableInstance);
                }
                
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
    }
}