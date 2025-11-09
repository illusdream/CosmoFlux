using System;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    /// View组件，通过这个可以快速获取到UI prefab/打开或关闭UI ，获取到UI组件
    /// 命名要求，对应的Prefab和Class 名字必须一样
    /// </summary>
    public abstract class UIView
    {
        public GameObject UIPanelObject { get; set; }

        public Canvas Canvas { get; set; }
        
        public CanvasGroup UIPanelCanvasGroup { get; set; }
        
        public abstract EUILayer UILayer { get; }
        
        public abstract string PackageName { get; }
        
        public abstract string UIPath { get;}
        
        public abstract int LayerOffest { get; }
        
        public bool IsOpen { get; set; }
        
        public event Action<UIView> OnInit;
        
        /// <summary>
        /// 该方法应该在开始动画结束之后才调用
        /// </summary>
        public event Action<UIView> OnOpen;
        
        /// <summary>
        /// 该方法应该在结束动画开始之前调用
        /// </summary>
        public event Action<UIView> OnClose;
        
        public event Action<UIView> OnDestroy;

        public void OnLoad()
        {
            AutoGenerateOnLoad();
            ScriptedOnLoad();
        }
        
        public virtual void AutoGenerateOnLoad()
        {
            
        }

        public virtual void ScriptedOnLoad()
        {
            
        }
        
        public virtual void InitUIPanel()
        {
            OnInit?.Invoke(this);
        }

        public virtual void Open()
        {
            UIPanelCanvasGroup.alpha = 1;
            UIPanelCanvasGroup.blocksRaycasts = true;
            UIPanelCanvasGroup.interactable = true;
            OnOpen?.Invoke(this);
            IsOpen = true;
        }

        public virtual void Close()
        {
            OnClose?.Invoke(this);
            UIPanelCanvasGroup.alpha = 0;
            UIPanelCanvasGroup.blocksRaycasts = false;
            UIPanelCanvasGroup.interactable = false;
            IsOpen = false;
        }

        public void Destroy()
        {
            OnDestroy?.Invoke(this);
        }

        /// <summary>
        /// 用于做动画什么的
        /// </summary>
        public virtual void OnUpdate()
        {
            
        }
        /// <summary>
        /// 用于做动画什么的
        /// </summary>
        public virtual void OnLateUpdate()
        {
            
        }
    }
}