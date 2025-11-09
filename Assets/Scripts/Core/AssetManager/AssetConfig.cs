using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("Asset")]
    public partial class AssetConfig : ConfigScriptObject
    {
        public float NeedCleanUpMemoryThreshold;
        public override string ConfigName => "Asset";
    }
    
}