using System;
using System.Collections.Generic;
using Game.Input;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class BuilderActionCollector : MonoBehaviour,IHostedLogicUpdate
    {
        [ShowInInspector]
        public CommendCollector Collector;

        public PlayerInput playerInput;
        public void Awake()
        {
            playerInput = InputManager.Instance.GetCurrentInputAction();
            Collector = new CommendCollector();
            InitializeAllCommendCallBacks();
        }

        public void OnEnable()
        {
            Collector.SetEnabled(true);
            InputManager.Instance.RegisterHostedUpdate(this);
        }

        public void HostedLogicUpdate()
        {
            Collector.Update();
            LogicUpdate();
        }

        public void OnDisable()
        {
            Collector.SetEnabled(false);
            InputManager.Instance.UnregisterHostedUpdate(this);
        }

        public void OnDestroy()
        {
            UnInitializeAllCommendCallBacks();
        }

        public void InitializeAllCommendCallBacks()
        {
            Collector.AddCommendCollection(InputBuildAction.SwitchBuildMode);
            playerInput.BuildMode.SwitchBuildMode.performed += SwitchBuildModeOnperformed;
            Collector.AddCommendCollection(InputBuildAction.LeftClick);
            playerInput.BuildMode.LeftClick.performed += LeftClickOnperformed;
            Collector.AddCommendCollection(InputBuildAction.OpenModularList);
            playerInput.BuildMode.OpenModularList.performed += OpenModularListOnperformed;
            Collector.AddCommendCollection(InputBuildAction.SwitchSnapMode);
            playerInput.BuildMode.SwitchSnapMode.performed += SwitchSnapModeOnperformed;
        }



        public void UnInitializeAllCommendCallBacks()
        {
            playerInput.BuildMode.SwitchBuildMode.performed -= SwitchBuildModeOnperformed;
            playerInput.BuildMode.LeftClick.performed -= LeftClickOnperformed;
            playerInput.BuildMode.OpenModularList.performed -= OpenModularListOnperformed;
            playerInput.BuildMode.SwitchSnapMode.performed -= SwitchSnapModeOnperformed;
        }
        private void SwitchSnapModeOnperformed(InputAction.CallbackContext obj)
        {
            Collector.AddCommend(InputBuildAction.SwitchSnapMode,new BaseCommend());
        }
        private void OpenModularListOnperformed(InputAction.CallbackContext obj)
        {
            Collector.AddCommend(InputBuildAction.OpenModularList,new BaseCommend());
        }

        private void LeftClickOnperformed(InputAction.CallbackContext obj)
        {
            Collector.AddCommend(InputBuildAction.LeftClick,new BaseCommend());
        }

        private void SwitchBuildModeOnperformed(InputAction.CallbackContext obj)
        {
            Collector.AddCommend(InputBuildAction.SwitchBuildMode,new BaseCommend());
        }

        public void LogicUpdate()
        {
        
        }

        public void Query<T>(string commendType,int firstIndex, int lastIndex, List<T> result) where T : ICommend
        {
            Collector.Query(commendType, firstIndex, lastIndex, result);
        }
        

        public bool CheckCurrent<T>(string commendType,out T result) where T : class, ICommend
        {
            return Collector.CheckCurrent(commendType, out result);
        }

        public void Clear(string commendType)
        {
            Collector.Clear(commendType);
        }
    }
}