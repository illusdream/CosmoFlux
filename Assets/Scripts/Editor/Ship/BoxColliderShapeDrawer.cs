using Game;
using ilsFramework.Core;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BoxColliderShapeDrawer : OdinValueDrawer<BoxColliderShape>
    {
        protected override void Initialize()
        {
            this.Property.SerializationRoot.ValueEntry.WeakSmartValue.LogSelf();
            base.Initialize();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = this.ValueEntry.SmartValue;

            SirenixEditorGUI.BeginLegendBox();
            {
                if (label != null)
                {
                    GUILayout.Label(label.text);
                }
                else
                {
                    
                }

                EditorGUILayout.BeginHorizontal();
                   
                EditorGUILayout.BeginVertical(GUILayout.Width(24));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(EditorGUIUtility.IconContent("EditCollider"),GUILayout.Width(24),GUILayout.Height(24)))
                {
                    ShipColliderShapeEditorManager.InvokeOnShapeEditorChanged(ValueEntry.Property.SerializationRoot.ValueEntry.WeakSmartValue as BaseModularNode, value);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();
                value.center = EditorGUILayout.Vector3Field("center", value.center);
                value.size = EditorGUILayout.Vector3Field("size", value.size);
                value.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("rotation", value.rotation.eulerAngles));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(ValueEntry.Property.SerializationRoot.ValueEntry.WeakSmartValue as BaseModularNode);
                    SceneView.RepaintAll();
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
            }
            SirenixEditorGUI.EndLegendBox();
        }
    }
}