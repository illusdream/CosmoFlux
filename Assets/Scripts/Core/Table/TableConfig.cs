using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("TableConfig")]
    public class TableConfig : ConfigScriptObject
    {
        public override string ConfigName => "配置表设置";

        [FolderPath]
        public string TargetFolder;
    }
}