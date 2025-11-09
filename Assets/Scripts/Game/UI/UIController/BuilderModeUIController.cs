using System.Collections.Generic;
using System.ComponentModel;
using ilsFramework.Core;
using TMPro;

namespace Game
{
    public class BuilderModeUIController : UIController
    {
        private ModelReference<GameBuildStateModel> _buildStateModel = new ModelReference<GameBuildStateModel>();
        
        public override void OnInitialize()
        {
            base.OnInitialize();
            _buildStateModel.Value.PropertyChanged += BuildStateModelOnPropertyChanged;
        }

        private void BuildStateModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(GameBuildStateModel.UseModelID):
                {
                    if (_buildStateModel.Value.UseModelID > 0)
                    {
                        UIManager.Instance.GetUIPanelAsync<UIModularListView>(view => view.Close());
                    }
                    break;
                }
            }
        }

        public void SwitchBuilderListUI()
        {
            UIManager.Instance.GetUIPanelAsync<UIModularListView>((view) =>
            {
                if (view.IsOpen)
                {
                    view.Close();
                }
                else
                {
                    view.Open();
                }
            });
        }

        public void SwitchMainBuildUI()
        {
            UIManager.Instance.GetUIPanelAsync<UIMainBuildView>((view) =>
            {
                if (view.IsOpen)
                {
                    view.Close();
                }
                else
                {
                    view.Open();
                }
            });
        }
        
    }
}