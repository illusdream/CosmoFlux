using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core.Editor
{
    public class StateMachineInspectorDrawer : OdinValueDrawer<StateMachine>
    {
   
        public static State SelectState;
        
        private double lastClickTime;
        private const double doubleClickThreshold = 0.2; // 双击时间阈值（秒）

        private Texture2D backgroundTexture;
        private Texture2D selectedBackgroundTexture;
        private Texture2D _excutingBackgroundTexture;
        private Texture2D _unExcutingBackgroundTexture;
        bool stateFoldout = true;
        
        bool allStatesFoldout = false;
        
        
        protected override void DrawPropertyLayout(GUIContent label)
        {

            // 创建 GUIStyle
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.background = backgroundTexture;
            
            GUILayout.BeginVertical();
            {
                GUIStyle styleExcuting = new GUIStyle(GUI.skin.label);
                styleExcuting.normal.background = ValueEntry.SmartValue.IsExcuting ? _excutingBackgroundTexture : _unExcutingBackgroundTexture;
                using (new GUIBackgroundColorChanger(Color.white))
                using (new GUIContentColorChanger(Color.black))
                {
                    GUILayout.Label( (ValueEntry.SmartValue.IsExcuting ? "执行中" : "未执行"),styleExcuting);
                }
                
                
                
                int prefix = 0;
                stateFoldout = SirenixEditorGUI.Foldout(stateFoldout,"当前状态调用链",GUI.skin.label);
                if (stateFoldout)
                {
                      if (ValueEntry.SmartValue?.chain != null && ValueEntry.SmartValue?.chain.states.Count > 0)
                      {
                          using (new GUIBackgroundColorChanger(Color.grey))
                          {
                              using (new GUIColorChanger(Color.white))
                              {
                                  foreach (var state in ValueEntry.SmartValue.chain.GetStates().Skip(1))
                                  {
                                      if(state == SelectState)
                                          style.normal.background = selectedBackgroundTexture;
                                      GUILayout.Label(string.Concat(new string('\t', prefix), state.Key,$"({state.GetType().GetNiceName()})"),style);

                                      Rect textAreaRect = GUILayoutUtility.GetLastRect();

                                      Event evt = Event.current;
                                      if (evt.type == EventType.MouseDown && evt.button == 0 && textAreaRect.Contains(evt.mousePosition))
                                      {
                                          SelectState = state;

                                          double currentTime = EditorApplication.timeSinceStartup;
                                          if (currentTime - lastClickTime < doubleClickThreshold && state == SelectState)
                                          {
                                              IDEJumpHelper.OpenTypeDefinition(state.GetType());
                                              lastClickTime = 0;
                                          }
                                          else
                                          {
                                              lastClickTime = currentTime;
                                          }
                                      }
                                
                                      prefix++;
                                      if(state == SelectState)
                                          style.normal.background = backgroundTexture;
                                  }
                              }
                          }

                      }
                      else
                      {
                          GUILayout.Label("No CurrentState",style);
                      }
                }
                
                allStatesFoldout = SirenixEditorGUI.Foldout(allStatesFoldout,"全部状态");
                if (allStatesFoldout)
                {
                    if (ValueEntry?.SmartValue?.Root == null)
                    {
                        GUILayout.Label("No CurrentState",style);
                    }
                    else
                    {

                            using (new GUIBackgroundColorChanger(Color.grey))
                            {
                                using (new GUIColorChanger(Color.white))
                                {
                                    foreach (var state in GetAllStates(ValueEntry.SmartValue.Root).Skip(1))
                                    {
                                        if (state.Item1 == SelectState)
                                            style.normal.background = selectedBackgroundTexture;
                                        GUILayout.Label(string.Concat(new string('\t', state.depth-1), state.Item1.Key, $"({state.Item1.GetType().GetNiceName()})"), style);

                                        Rect textAreaRect = GUILayoutUtility.GetLastRect();

                                        Event evt = Event.current;
                                        if (evt.type == EventType.MouseDown && evt.button == 0 && textAreaRect.Contains(evt.mousePosition))
                                        {
                                            SelectState = state.Item1;

                                            double currentTime = EditorApplication.timeSinceStartup;
                                            if (currentTime - lastClickTime < doubleClickThreshold)
                                            {
                                                Debug.Log("Double Clicked on TextArea!");

                                                IDEJumpHelper.OpenTypeDefinition(state.Item1.GetType());

                                                lastClickTime = 0;
                                            }
                                            else
                                            {
                                                lastClickTime = currentTime;
                                            }
                                        }
                                        if (state.Item1 == SelectState)
                                            style.normal.background = backgroundTexture;
                                    }
                                }
                          

                      }

                    }
                }
                
            }
            GUILayout.EndVertical();
        }

        protected override void Initialize()
        {
            SelectState = null;
             backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0,new Color(0.2f, 0.2f, 0.2f, 1));
            backgroundTexture.Apply();

            selectedBackgroundTexture = new Texture2D(1, 1);
            selectedBackgroundTexture.SetPixel(0, 0,Color.grey);
            selectedBackgroundTexture.Apply();
            
            _excutingBackgroundTexture= new Texture2D(1, 1);
            _excutingBackgroundTexture.SetPixel(0, 0,new Color(0f, 0.8f, 0f, 1));
            _excutingBackgroundTexture.Apply();
            
            _unExcutingBackgroundTexture= new Texture2D(1, 1);
            _unExcutingBackgroundTexture.SetPixel(0, 0,new Color(0.8f, 0.0f, 0f, 1));
            _unExcutingBackgroundTexture.Apply();
            base.Initialize();
        }

        private List<(State, int depth)> GetAllStates(State root)
        {
            List<(State, int depth)> states = new List<(State, int depth)>();

            Stack<(State, int depth)> stack = new Stack<(State, int depth)>();
            stack.Push((root,0));
        
            while (stack.Count > 0)
            {
                (State, int depth) node = stack.Pop();
                states.Add(node); // 访问节点
            
                // 将子节点逆序压入栈中，保证遍历顺序正确
                foreach (var stateChild in node.Item1.Children.Values.Reverse())
                {
                    stack.Push((stateChild,node.depth+1));
                }
            }

            return states;
        }
        
    }
}