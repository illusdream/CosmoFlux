using System;
using System.Collections.Generic;
using System.ComponentModel;
using ilsFramework.Core;
using TMPro;

namespace ilsFramework.Core
{
#region AutoGenerate
    using UnityEngine;
    using Game;

    public partial class UIMainBuildView : UIView
    {
        public UnityEngine.RectTransform trans_MainControlPanel;
        public Game.UIList list_MainControlPanel;

        public override void AutoGenerateOnLoad()
        {
            base.AutoGenerateOnLoad();

            trans_MainControlPanel = UIPanelObject.transform.Find("BottomAnchor/[trans,list]MainControlPanel").GetComponent<UnityEngine.RectTransform>();
            list_MainControlPanel = UIPanelObject.transform.Find("BottomAnchor/[trans,list]MainControlPanel").GetComponent<Game.UIList>();
        }

    }
#endregion

    public partial class UIMainBuildView : UIView
    {
        public override EUILayer UILayer => EUILayer.Normal;
        public override string PackageName => "UI";
         public override string UIPath =>"Assets/Assets/Prefab/UIView/UIMainBuildView.prefab";
         public override int LayerOffest => 0;

         private ModelReference<GameBuildStateModel> _modelReference;

         private List<(string,string)> selectStageOperationPrompts = new List<(string,string)>() {("Z","Choose Modular")};
         private List<(string,string)> PlaceStageOperationPrompts = new List<(string,string)>() {("Tab","Snap Mode"),("Z","Choose Modular")};
        public override void ScriptedOnLoad()
        {

        }

        public override void InitUIPanel()
        {
            base.InitUIPanel();
            list_MainControlPanel.MapBuildAction = OperationPromptMapBuildAction;
            _modelReference = new ModelReference<GameBuildStateModel>();
        }

        public override void Open()
        {
            _modelReference.Value.PropertyChanged += ValueOnPropertyChanged;
            UpdateOperationPrompt();
            base.Open();
        }



        public override void Close()
        {
            _modelReference.Value.PropertyChanged -= ValueOnPropertyChanged;
            base.Close();
        }
        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GameBuildStateModel.Stage):
                {
                    UpdateOperationPrompt();
                    break;
                }
            }
        }

        private void UpdateOperationPrompt()
        {
            switch (_modelReference.Value.Stage)
            {
                case GameBuilderState.Stage.ModifyState:
                    break;
                case GameBuilderState.Stage.PlaceState:
                    SetOperationPrompt(PlaceStageOperationPrompts);
                    break;
                case GameBuilderState.Stage.SelectState:
                    SetOperationPrompt(selectStageOperationPrompts);
                    break;
                case GameBuilderState.Stage.ShowModularState:
                    list_MainControlPanel.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SetOperationPrompt(List<(string, string)> operations)
        {
            list_MainControlPanel.Clear();
            foreach (var operation in operations)
            {
                operation.Item1.LogSelf();
                var item = list_MainControlPanel.AddItem();
                item.gameObject.SetActive(true);
                item.gameObject.transform.SetParent(list_MainControlPanel.transform, false);
                item.GetComponent<TextMeshProUGUI>("ButtonName").text = operation.Item1;
                item.GetComponent<TextMeshProUGUI>("OperationName").text = operation.Item2;
            }
        }
        
        private void OperationPromptMapBuildAction(GameObject arg1, Dictionary<string, Component> arg2)
        {
            arg2["ButtonName"] = arg1.transform.Find("Icon/ButtonName").GetComponent<TextMeshProUGUI>();
            arg2["OperationName"] = arg1.transform.Find("OperationName").GetComponent<TextMeshProUGUI>();
        }

    }
}
