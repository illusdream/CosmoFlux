using ilsFramework.Core;
using UnityEngine.SceneManagement;

namespace Game
{
    public class TestGameState : ProcedureNode
    {
        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnEnter()
        {
            SceneManager.LoadScene((int)EScene.Test);
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public override void OnLogicUpdate()
        {
            base.OnLogicUpdate();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}