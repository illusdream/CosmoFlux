using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("UIConfig")]
    public class UIConfig : ConfigScriptObject
    {
        public override string ConfigName => "UIConfig";

        public GameObject UIEventHandler;
        
        public Vector2 ReferenceResolution;
        
        public string ResourceUITag = "UI";
        [FolderPath]
        public string UIViewCSharpFileFolderPath;
        
        public string UIViewCSharpNamespace;
    }
}