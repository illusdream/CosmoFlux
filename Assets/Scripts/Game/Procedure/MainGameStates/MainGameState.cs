using System;
using ilsFramework.Core;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainGameState : SubProcedureController
    {
        public override void OnInit()
        {
            this.AddState<NormalGameState>();
            this.AddState<GameBuilderState>();
            
            CameraManager.Instance.RegisterCamera<BuilderBaseCamera>();
            CameraManager.Instance.RegisterCamera<ShipControlBaseCamera>();
            base.OnInit();
        }

        public override void OnEnter()
        {
            SceneManager.LoadScene((int)EScene.Test);
            SceneHandler.SceneOnLoaded += SceneHandlerOnSceneOnLoaded;
            
            base.OnEnter();
        }

        private void SceneHandlerOnSceneOnLoaded(EventArgs obj)
        {
            StartState<NormalGameState>();
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
            SceneHandler.SceneOnLoaded -= SceneHandlerOnSceneOnLoaded;
            base.OnExit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}