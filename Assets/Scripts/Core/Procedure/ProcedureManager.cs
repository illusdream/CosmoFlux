using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace ilsFramework.Core
{
    public partial class ProcedureManager : ManagerSingleton<ProcedureManager>,IAssemblyForeach
    {
        [ShowInInspector]
        ProcedureController _procedureController;
    
        ProcedureConfig procedureConfig;
        
        public bool GameProcedureEnabled { get; private set; }

        private Type procedureInitializerType;

        [ShowInInspector]
        public StateMachine StateMachine { get; private set; } = new StateMachine();
        public override IEnumerator OnInit()
        {
            procedureConfig = Config.GetConfig<ProcedureConfig>();
            
            GameProcedureEnabled = procedureConfig.EnableCommenProcedure;
        
            _procedureController = new ProcedureController();
            FrameworkCore.Instance.AfterAllManagerInitialized += AfterAllManagerInitialized;
            var state = new TestState();
            StateMachine.RegisterState("1",state);
            StateMachine.RegisterState("2",new TestState());
            StateMachine.RegisterState("1.1",new TestState());
            StateMachine.RegisterState("1.2",new TestState());
            
            yield return null;
        }

        private void AfterAllManagerInitialized()
        {
            if (procedureInitializerType != null && Activator.CreateInstance(procedureInitializerType) is ProcedureInitializer instance)
            {
                instance.InitializeProcedure(_procedureController);

            }
        }


        public void ForeachCurrentAssembly(Type[] types)
        {
            foreach (var type in types)
            {
                if (typeof(ProcedureInitializer).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    procedureInitializerType = type;
                    break;
                }
            }
        }

        public override void OnUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.Update();
            }
        }

        public override void OnLateUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LateUpdate();
            }
        }

        public override void OnLogicUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LogicUpdate();
            }
        }

        public override void OnFixedUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.FixedUpdate();
            }
        }

        public override void OnDestroy()
        {
           _procedureController.OnDestroy();
        }

        public override void OnDrawGizmos()
        {
           _procedureController.DrawGizmos();
        }

        public override void OnDrawGizmosSelected()
        {
          
        }

    }
}