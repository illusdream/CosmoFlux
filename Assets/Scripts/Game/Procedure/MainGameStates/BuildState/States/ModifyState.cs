using ilsFramework.Core;

namespace Game
{
    public partial class  GameBuilderState : SubProcedureController
    {
        public class ModifyState : ProcedureNode
        {
            private ModelReference<GameBuildStateModel> _modelReference;


            public override void OnInit()
            {
                _modelReference = new ModelReference<GameBuildStateModel>();
                base.OnInit();
            }

            public override void OnEnter()
            {
                _modelReference.Value.Stage = Stage.ModifyState;
                UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
                {
                    if (!view.IsOpen)
                        view.Open();
                });
                base.OnEnter();
            }
        }
    }
}