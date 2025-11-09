using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using ilsFramework;
using ilsFramework.Core;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : ManagerSingleton<InputManager>
{
    public string InputModifierDataBaseName = "InputModifierDB";
    
    private PlayerInput _mainInputAction;
    private InputActionAsset _inputActionAsset;
    
   // [ShowInInspector]
   // public static PlayerInputHandler PlayerInputHandler { get; private set; }
    
    //系统用
    [ShowInInspector]
    public static BuilderActionCollector BuilderActionCollector { get; private set; }
    //系统用
    [ShowInInspector]
    public static PlayActionCollector PlayActionCollector { get; private set; }
    
    public override IEnumerator OnInit()
    {
        _mainInputAction = new PlayerInput();
        _inputActionAsset = _mainInputAction.asset;

        using (var connection=  DataBase.GetPersistentConnection(InputModifierDataBaseName))
        {
            if (connection.TryGetTable<InputModifierInfo>(out var value))
            {
                LoadAllModifierAction(value.ToList());
            }
        }
        _mainInputAction.Play.Enable();
        _mainInputAction.UI.Enable();
        _mainInputAction.BuildMode.Enable();

        var BuilderActionGO = new GameObject("BuilderAction");
        BuilderActionGO.transform.SetParent(ContainerObject.transform);
        BuilderActionCollector = BuilderActionGO.AddComponent<BuilderActionCollector>();
        
        var PlayActionGO = new GameObject("PlayAction");
        PlayActionGO.transform.SetParent(ContainerObject.transform);
        PlayActionCollector = PlayActionGO.AddComponent<PlayActionCollector>();
        yield return null;
    }

    public override void OnUpdate()
    {
       
    }

    public override void OnLateUpdate()
    {
      
    }

    public override void OnLogicUpdate()
    {
       // PlayerInputHandler.LogicUpdate();
    }

    public override void OnFixedUpdate()
    {
       
    }

    public override void OnDestroy()
    {
     
    }

    public override void OnDrawGizmos()
    {
      
    }

    public override void OnDrawGizmosSelected()
    {
        
    }


    private void LoadAllModifierAction(List<InputModifierInfo> modifiers)
    {
        foreach (var info in modifiers)
        {
            var action = _mainInputAction.FindAction(info.GUID);
            action?.LoadBindingOverridesFromJson(info.ModifierJson);

        }
    }

    public void SaveBinding(InputAction action)
    {
        if (_mainInputAction.Contains(action))
        {
            using (var connection = GetInputModifierConnection())
            {
                connection.CreateTable<InputModifierInfo>();
                if (connection.TryGetTable<InputModifierInfo>(out _))
                {
                    connection.InsertOrReplace(new InputModifierInfo() { GUID = action.id.ToString(), ModifierJson = action.SaveBindingOverridesAsJson() });
                }
            }

        }
    }

    /// <summary>
    /// 重置对应按键的绑定
    /// </summary>
    /// <param name="action"></param>
    public void ResetBinding(InputAction action)
    {
        using (var sqliteconnection = GetInputModifierConnection())
        {
            var actionBinding =  sqliteconnection.Table<InputModifierInfo>().Any(info => info.GUID == action.id.ToString());
            if (actionBinding)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    action.RemoveBindingOverride(i);
                }
                sqliteconnection.Delete<InputModifierInfo>(action.id.ToString());
            }
        }
    }
    /// <summary>
    /// 重置对应Map的按键绑定（GamePlay或UI）
    /// </summary>
    /// <param name="actionMapNameOrID">名字或者其ID</param>
    public void ResetAllBindings(string actionMapNameOrID)
    {
        using (var sqliteconnection = GetInputModifierConnection())
        {
            foreach (var action in _inputActionAsset.FindActionMap(actionMapNameOrID))
            {
                var actionBinding =  sqliteconnection.Table<InputModifierInfo>().Any(info => info.GUID == action.id.ToString());
                if (actionBinding)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        action.RemoveBindingOverride(i);
                    }
                    sqliteconnection.Delete<InputModifierInfo>(action.id.ToString());
                }
            }
        }
    }
    /// <summary>
    /// 重置对应Map的按键绑定（GamePlay或UI）
    /// </summary>
    /// <param name="actionMap">actionMap实例</param>
    public void ReloadAllBindings(InputActionMap actionMap)
    {
        using (var sqliteconnection = GetInputModifierConnection())
        {
            foreach (var action in actionMap)
            {
                var actionBinding =  sqliteconnection.Table<InputModifierInfo>().Any(info => info.GUID == action.id.ToString());
                if (actionBinding)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        action.RemoveBindingOverride(i);
                    }
                    sqliteconnection.Delete<InputModifierInfo>(action.id.ToString());
                }
            }
        }
    }
    
    /// <summary>
    /// 重置所有按键设置的绑定
    /// </summary>
    public void ResetAllBindings()
    {
        using (var sqliteconnection = GetInputModifierConnection())
        {
            var allBindings = sqliteconnection.Table<InputModifierInfo>().ToList();
            foreach (var inputModifierInfo in allBindings)
            {
                inputModifierInfo.ModifierJson.LogSelf();
              var  action = _mainInputAction.FindAction(inputModifierInfo.GUID);
                if (action != null)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        action.RemoveBindingOverride(i);
                    }
                }
            }
            

            sqliteconnection.DeleteAll<InputModifierInfo>();
        }
    }

    public void SetPlayActionEnabledInput(bool enabled)
    {
        if (enabled)
            _mainInputAction.Play.Enable();
        else
            _mainInputAction.Play.Disable();
    }

    public void SetUIActionEnabledInput(bool enabled)
    {
        if (enabled)
            _mainInputAction.UI.Enable();
        else
            _mainInputAction.UI.Disable();
    }

    public void SetBuildModeEnabledInput(bool enabled)
    {
        if (enabled)
            _mainInputAction.BuildMode.Enable();
        else
            _mainInputAction.BuildMode.Disable();
    }

    public PlayerInput GetCurrentInputAction()
    {
        return _mainInputAction;
    }

    private SQLiteConnection GetInputModifierConnection()
    {
        return DataBase.GetPersistentConnection(InputModifierDataBaseName);
    }


}