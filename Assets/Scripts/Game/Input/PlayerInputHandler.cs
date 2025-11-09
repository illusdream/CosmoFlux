using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace Game
{
    public class PlayerInputHandler
    {
        private PlayerInput input;

        public InputActionTracker<Vector2> Move { get; private set; }

        public InputActionTracker<Vector2> Look { get; private set; }

        public InputActionTracker Shift { get; private set; }

        public InputActionTracker Jump { get; private set; }

        /// <summary>
        /// 可以存储的逻辑帧数目
        /// </summary>
        private int stackFrameCount;

        public float InputBufferTime = 1;

        [ShowInInspector]
        private InputFrameInfoList InputFrames;

        

        public void InitAll(PlayerInput _input)
        {
            input =_input;
            stackFrameCount = (int)(InputBufferTime * Config.GetFrameworkConfig().LogicUpdateCountPerScecond);
            InputFrames = new InputFrameInfoList(stackFrameCount);
            
           // Move = new InputActionTracker<Vector2>(input.Player.Move);
          //  Move.performed += context => AddSingleFrameInfo(EPlayerInput.Move, context);

            //Look = new InputActionTracker<Vector2>(input.Player.Look);
            // Look.performed += context => AddSingleFrameInfo(EPlayerInput.Look, context);

            //Shift = new InputActionTracker(input.Player.Shift);
            //Shift.performed += context => AddSingleFrameInfo(EPlayerInput.Shift, context);

            //Jump = new InputActionTracker(input.Player.Jump);
            //Jump.performed += context => AddSingleFrameInfo(EPlayerInput.Jump, context);

            input.BuildMode.SwitchBuildMode.performed += context => AddSingleFrameInfo(EPlayerInput.SwitchBuildMode, context);

            input.BuildMode.LeftClick.performed += context => AddSingleFrameInfo(EPlayerInput.LeftClick_BuildMode, context);
            
            input.BuildMode.OpenModularList.performed += context => AddSingleFrameInfo(EPlayerInput.OpenModularList, context);
        }

        private void AddSingleFrameInfo(EPlayerInput playerInput, InputAction.CallbackContext context)
        {
            var result = new SingleInputInfo()
            {
               
                FrameID = FrameworkCore.Instance.LogicFrameIndex,
                PlayerInput = playerInput,
                InputInteraction = context.interaction switch
                {
                    HoldInteraction => EInputInteraction.Hold,
                    MultiTapInteraction => EInputInteraction.MultiTap,
                    PressInteraction => EInputInteraction.Press,
                    SlowTapInteraction => EInputInteraction.SlowTap,
                    TapInteraction => EInputInteraction.Tap,
                    null => EInputInteraction.None,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
            InputFrames.Current.AddInputInfo(result);
        }

        public void LogicUpdate()
        {
            InputFrameInfo currentFrameInfo = new InputFrameInfo();
            InputFrames.Add(currentFrameInfo);
        }

        public bool HasTriggered(EPlayerInput playerInput, EInputInteraction interaction, float duration)
        {
            //根据时间转换到具体的表中
            return InputFrames.HasTriggered(playerInput, interaction, duration);
        }

        public void OnDisable()
        {
            
        }
    }
}
