using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    [AutoBuildOrLoadConfig("Procedure")]
    public class ProcedureConfig : ConfigScriptObject
    {
        public override string ConfigName => "GameControl";

        [LabelText("是否启用正常的游戏流程")]
        [ToggleLeft]
        public bool EnableCommenProcedure;
        
        [LabelText("启用测试场景")]
        [ToggleLeft]
        public bool EnableTestScene;
    }
}