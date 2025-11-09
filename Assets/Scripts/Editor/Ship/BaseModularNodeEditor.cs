using System;
using System.Linq;
using Game;
using ilsFramework.Core;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BaseModularNode))]
    public class BaseModularNodeEditor :  OdinEditor
    {
        public BoxBoundsHandle boundsHandle;

        public BoxColliderShape currentEditBoxColliderShape = null;

        private bool originIsHiddenTool;
        protected override void OnEnable()
        {
            ShipColliderShapeEditorManager.RegisterChangeEvent(this.target as BaseModularNode,OnChangeEditorBoxColliderShape);
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            var value = (target as BaseModularNode);
            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck() && boundsHandle != null && currentEditBoxColliderShape != null )
            {
                EditorUtility.SetDirty(value.gameObject);
                SceneView.RepaintAll();
            }
            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            var value = (target as BaseModularNode);
            if (boundsHandle == null || currentEditBoxColliderShape == null || !value)
            {
                return;
            }
            EditorGUI.BeginChangeCheck();
            var rot =value.transform.rotation *currentEditBoxColliderShape.rotation;
            var color = Color.white;
            if (value.BuildHelperBound == currentEditBoxColliderShape)
            {
                color = Color.yellow;
            }
            else if (value.CollisonBounds.Contains(currentEditBoxColliderShape))
            {
                color = Color.green;
            }
            else
            {
                return;
            }
            using (new Handles.DrawingScope(Matrix4x4.TRS(value.transform.position, rot, value.transform.lossyScale)))
            {
                boundsHandle.center = currentEditBoxColliderShape.center;
                boundsHandle.size = currentEditBoxColliderShape.size;
                boundsHandle.handleColor = color;
                boundsHandle.wireframeColor = color;
                boundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                     currentEditBoxColliderShape.center = boundsHandle.center;
                     currentEditBoxColliderShape.size = boundsHandle.size;
                     EditorUtility.SetDirty(value.gameObject);
                     SceneView.RepaintAll();
                }
            }
            
        

        }

        protected override void OnDisable()
        {
            ShipColliderShapeEditorManager.UnRegisterChangeEvent(this.target as BaseModularNode,OnChangeEditorBoxColliderShape);
            ShipColliderShapeEditorManager.UnRegisterNode(this.target as BaseModularNode);
            if (currentEditBoxColliderShape != null)
            {
                Tools.hidden = originIsHiddenTool ;
            }
            base.OnDisable();
        }

        private void OnChangeEditorBoxColliderShape(BoxColliderShape boxColliderShape)
        {
            if (currentEditBoxColliderShape != boxColliderShape)
            {
                boundsHandle = new();
                boundsHandle.axes =  PrimitiveBoundsHandle.Axes.All;
                boundsHandle.size = boxColliderShape.size;
                boundsHandle.center = boxColliderShape.center;
                currentEditBoxColliderShape = boxColliderShape;
                if (currentEditBoxColliderShape == null)
                {
                    originIsHiddenTool =  Tools.hidden;
                }
                Tools.hidden = true;
                SceneView.RepaintAll();
            }
            else
            {
                boundsHandle = null;
                currentEditBoxColliderShape = null;
                Tools.hidden = originIsHiddenTool;
                SceneView.RepaintAll();
            }

           
        }
    }
}