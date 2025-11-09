using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using Game;
using ilsFramework.Core;
using TMPro;
using UnityEngine.UI;

namespace ilsFramework.Core
{
#region AutoGenerate
    using UnityEngine;
    using UnityEngine.UI;
    using Game;

    public partial class UIModularListView : UIView
    {
        public UnityEngine.RectTransform trans_Content;
        public UnityEngine.UI.Button btn_Struct;
        public UnityEngine.UI.Button btn_Engine;
        public UnityEngine.UI.Button btn_Energy;
        public UnityEngine.UI.Button btn_Weapon;
        public UnityEngine.UI.ScrollRect scroll_CenterContent;
        public Game.UIList list_ViewPort;

        public override void AutoGenerateOnLoad()
        {
            base.AutoGenerateOnLoad();

            trans_Content = UIPanelObject.transform.Find("[trans]Content").GetComponent<UnityEngine.RectTransform>();
            btn_Struct = UIPanelObject.transform.Find("[trans]Content/Border/TopTabs/[btn]Struct").GetComponent<UnityEngine.UI.Button>();
            btn_Engine = UIPanelObject.transform.Find("[trans]Content/Border/TopTabs/[btn]Engine").GetComponent<UnityEngine.UI.Button>();
            btn_Energy = UIPanelObject.transform.Find("[trans]Content/Border/TopTabs/[btn]Energy").GetComponent<UnityEngine.UI.Button>();
            btn_Weapon = UIPanelObject.transform.Find("[trans]Content/Border/TopTabs/[btn]Weapon").GetComponent<UnityEngine.UI.Button>();
            scroll_CenterContent = UIPanelObject.transform.Find("[trans]Content/Border/[scroll]CenterContent").GetComponent<UnityEngine.UI.ScrollRect>();
            list_ViewPort = UIPanelObject.transform.Find("[trans]Content/Border/[scroll]CenterContent/[list]ViewPort").GetComponent<Game.UIList>();
        }

    }
#endregion






    public partial class UIModularListView : UIView
    {
        public override EUILayer UILayer => EUILayer.Normal;


        public override string PackageName => "UI";


        public override string UIPath =>"Assets/Assets/Prefab/UIView/UIModularListView.prefab";


        public override int LayerOffest => 0;
        
       
        
        private ModelReference<GameBuildStateModel> gameBuildStateModel = new ModelReference<GameBuildStateModel>();

        private ModelReference<GameBuildStateModel> _modelReference;

        public override void ScriptedOnLoad()
        {

        }

        public override void InitUIPanel()
        {
            base.InitUIPanel();
            _modelReference = new ModelReference<GameBuildStateModel>();
            trans_Content.localPosition = new Vector2(-1920, 0);
            UIPanelCanvasGroup.alpha = 0;
            

            list_ViewPort.MapBuildAction += MapBuildAction;
            BuildModularList(_modelReference.Value.SelectedEModularType);

            btn_Struct.onClick.AddListener(()=> _modelReference.Value.SelectedEModularType = EModularType.Struct);
            btn_Energy.onClick.AddListener(()=> _modelReference.Value.SelectedEModularType = EModularType.Energy);
            btn_Engine.onClick.AddListener(()=> _modelReference.Value.SelectedEModularType = EModularType.Engine);
            btn_Weapon.onClick.AddListener(()=> _modelReference.Value.SelectedEModularType = EModularType.Weapon);
        }




        private void MapBuildAction(GameObject arg1, Dictionary<string, Component> arg2)
        {
            arg2["GroupName"] = arg1.transform.Find("GroupName").GetComponent<TextMeshProUGUI>();
            arg2["GroupList"] = arg1.transform.Find("GroupList").GetComponent<UIList>();
        }

        public override void Open()
        {
            _modelReference.Value.PropertyChanged += ValueOnPropertyChanged;
            TimerManager.Instance.RegisterTimer(0.1f, 1, onCompleted: (_) =>
            {
                trans_Content.localPosition = new Vector2(-1920, 0);
                UIPanelCanvasGroup.alpha = 0;
                Sequence seq = DOTween.Sequence();
                seq.Append(trans_Content.DOLocalMoveX(0, 0.25f, true))
                    .Join(DOTween.To(()=>UIPanelCanvasGroup.alpha,(value)=>UIPanelCanvasGroup.alpha = value,1,0.25f))
                    .OnComplete(base.Open);
            });
        }

        public override void Close()
        {
            _modelReference.Value.PropertyChanged -= ValueOnPropertyChanged;
            Sequence seq = DOTween.Sequence();
            seq.Append(trans_Content.DOLocalMoveX(-1920, 0.25f, true))
                .Join(DOTween.To(()=>UIPanelCanvasGroup.alpha,(value)=>UIPanelCanvasGroup.alpha = value,0,0.25f))
                .OnComplete(base.Close);
        }
        
        
        private void DestroyOldModularList()
        {
            list_ViewPort.Clear();
        }

        private void BuildModularList(EModularType modularType)
        {
            Dictionary<string,List<tShipModular>> groupedByName = new Dictionary<string, List<tShipModular>>();

            var data = Table<tShipModular>.GetByExtraIndex(nameof(tShipModular.ModularType), modularType);

            foreach (var d in data)
            {
                if (groupedByName.TryGetValue(d.ModularGroup,out var list))
                {
                    list.Add(d);
                }
                else
                {
                    groupedByName.Add(d.ModularGroup,new List<tShipModular>() {d});
                }
            }
            
            foreach (var group in groupedByName)
            {
                var uiItem = list_ViewPort.AddItem();
                uiItem.gameObject.SetActive(true);
                uiItem.gameObject.transform.SetParent(list_ViewPort.transform, false);
                uiItem.GetComponent<TextMeshProUGUI>("GroupName").text = group.Key;
                var list = uiItem.GetComponent<UIList>("GroupList");
                list.MapBuildAction += GroupListBuildAction;
                foreach (var tShipModular in group.Value)
                {
                    var modularUI = list.AddItem();
                    modularUI.gameObject.SetActive(true);
                    modularUI.gameObject.transform.SetParent(list.transform, false);
                    Asset.LoadResourceAsync<Sprite>("ShipModular", tShipModular.Path.Replace(".prefab","Preview.png"), LoadResourcePriority.Default, (assethandle) =>
                    {
                        modularUI.GetComponent<Image>("Icon").sprite = assethandle.GetAssetObject<Sprite>();
                    });
                    modularUI.GetComponent<Button>("Button").onClick.AddListener(() =>
                    {
                        ChangeSelectedModular((int)tShipModular.ID);
                    });
                    
                }
            }
        }
        
        private void GroupListBuildAction(GameObject arg1, Dictionary<string, Component> arg2)
        {
            arg2["Icon"] = arg1.transform.Find("Icon").GetComponent<Image>();
            arg2["Button"] = arg1.GetComponent<Button>();
        }

        private void ChangeSelectedModular(int ModularID)
        {
            gameBuildStateModel.Value.UseModelID = ModularID;
        }
        
        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GameBuildStateModel.SelectedEModularType):
                {
                    DestroyOldModularList();
                    BuildModularList(gameBuildStateModel.Value.SelectedEModularType);
                    break;
                }
            }
        }
    }
}
