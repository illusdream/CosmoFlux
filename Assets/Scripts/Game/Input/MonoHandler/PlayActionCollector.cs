using System;
using System.Collections.Generic;
using Game.Input;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayActionCollector : MonoBehaviour,IHostedLogicUpdate
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
            Collector?.SetEnabled(false);
            InputManager.Instance?.UnregisterHostedUpdate(this);
        }

        public void InitializeAllCommendCallBacks()
        {
            Collector.AddCommendCollection(InputPlayAction.Move);
            Collector.AddCommendCollection(InputPlayAction.Look);
        }

        public void LogicUpdate()
        {
            playerInput ??= InputManager.Instance.GetCurrentInputAction();
            var moveValue = playerInput.Play.Move.ReadValue<Vector2>();
            Collector.AddCommend(InputPlayAction.Move, new ValueCommend<Vector2>(moveValue));
            
            var lookValue = playerInput.Play.Look.ReadValue<Vector2>();
            Collector.AddCommend(InputPlayAction.Look, new ValueCommend<Vector2>(lookValue));
        }
        
        
        public void Query<T>(string commendType,int firstIndex, int lastIndex, List<T> result) where T : ICommend
        {
            Collector.Query(commendType, firstIndex, lastIndex, result);
        }
        

        public bool CheckCurrent<T>(string commendType,out T result) where T : class, ICommend
        {
            return Collector.CheckCurrent(commendType, out result);
        }
    }
}