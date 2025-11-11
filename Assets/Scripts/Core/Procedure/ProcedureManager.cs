using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ilsFramework.Core
{
    public partial class ProcedureManager : ManagerSingleton<ProcedureManager>,IAssemblyForeach
    {
        [ShowInInspector]
        ProcedureController _procedureController;
        [ShowInInspector]
        StateMachine procedureStateMachine;
        ProcedureConfig procedureConfig;
        
        public bool GameProcedureEnabled { get; private set; }

        private Type procedureInitializerType;
        
        public override IEnumerator OnInit()
        {
            procedureConfig = Config.GetConfig<ProcedureConfig>();
            
            GameProcedureEnabled = procedureConfig.EnableCommenProcedure;
        
            _procedureController = new ProcedureController();
            procedureStateMachine = new StateMachine();
            FrameworkCore.Instance.AfterAllManagerInitialized += AfterAllManagerInitialized;
            yield return null;
        }

        private void AfterAllManagerInitialized()
        {
            if (procedureInitializerType != null && Activator.CreateInstance(procedureInitializerType) is ProcedureInitializer instance)
            {
                instance.InitializeProcedure(procedureStateMachine);
                instance.StartProcedure(procedureStateMachine);
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
                procedureStateMachine.Update(Time.deltaTime);
            }
            
        }

        public override void OnLateUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LateUpdate();
                procedureStateMachine.LateUpdate();
            }
        }

        public override void OnLogicUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.LogicUpdate();
                procedureStateMachine.LogicUpdate();
            }
        }

        public override void OnFixedUpdate()
        {
            if (GameProcedureEnabled)
            {
                _procedureController.FixedUpdate();
                procedureStateMachine.FixedUpdate();
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