using System;
using ilsFramework.Core;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainGameState : State
    {
        public const string MainGameStateKey = "MainGameState";
        public override void OnInit()
        {
            this.AddChild(NormalGameState.StateKey,new NormalGameState());
            this.AddChild(InMainMenuState.StateKey,new InMainMenuState());
            this.AddChild(GameBuilderState.StateKey,new GameBuilderState());
            CameraManager.Instance.RegisterCamera<BuilderBaseCamera>();
            CameraManager.Instance.RegisterCamera<ShipControlBaseCamera>();
            base.OnInit();
        }

        public override State GetInitialState()
        {
            return GetStateInChild(InMainMenuState.StateKey);
        }
        
        public override void OnEnter()
        {
            
            //
            //SceneHandler.SceneOnLoaded += SceneHandlerOnSceneOnLoaded;
            
            base.OnEnter();
        }

        private void SceneHandlerOnSceneOnLoaded(EventArgs obj)
        {
            //StartState<NormalGameState>();
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


    }
}