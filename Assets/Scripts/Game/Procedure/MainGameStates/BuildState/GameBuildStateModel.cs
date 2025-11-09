using ilsFramework.Core;
using UnityEngine;

namespace Game
{
    public class GameBuildStateModel : BaseModel
    {
        private int _UseModelID;

        public int UseModelID
        {
            get => _UseModelID;
            set
            {
                SetField(ref _UseModelID, value);
            }
        }

        private GameBuilderState.Stage _Stage;

        public GameBuilderState.Stage Stage
        {
            get => _Stage;
            set => SetField(ref _Stage, value);
        }
        
        /// <summary>
        /// 选择到任意模块的时候添加的材质
        /// </summary>
        public Material SelectedMaterial;
        
        /// <summary>
        /// 显示的模块
        /// </summary>
        public Material ShowModelMaterial;
        
        /// <summary>
        /// Modify
        /// </summary>
        public Material ModifyModelMaterial;

        private BaseModularNode _SelectedNode;

        public BaseModularNode SelectedNode
        {
            get => _SelectedNode;
            set => SetField(ref _SelectedNode, value);
        }
        
        private Vector3 _HitNormal;

        public Vector3 HitNormal
        {
            get => _HitNormal;
            set => SetField(ref _HitNormal, value);
        }
        
        private Vector3 _HitPoint;

        public Vector3 HitPoint
        {
            get => _HitPoint;
            set => SetField(ref _HitPoint, value);
        }

        public Vector3 _EularAngleOfEditorGO;

        public Vector3 EularAngleOfEditorGO
        {
            get => _EularAngleOfEditorGO;
            set => SetField(ref _EularAngleOfEditorGO, value);
        }

        private bool _IsInSnapMode = false;

        public bool IsInSnapMode
        {
            get => _IsInSnapMode;
            set => SetField(ref _IsInSnapMode, value);
        }
        
        public EModularType _SelectedEModularType;

        public EModularType SelectedEModularType
        {
            get => _SelectedEModularType;
            set => SetField(ref _SelectedEModularType, value);
        }

        private ShipCore _EditingShipCore;

        public ShipCore EditingShipCore
        {
            get => _EditingShipCore;
            set => SetField(ref _EditingShipCore, value);
        }
        
        public override void Initialize()
        {
            _UseModelID = -1;
            Asset.LoadResourceAsync<Material>("Assets/Assets/Base/Material/ModularSelect.mat", LoadResourcePriority.Default, (handle =>
            {
                SelectedMaterial = handle.GetAssetObject<Material>();
            }));
            Asset.LoadResourceAsync<Material>("Assets/Assets/Base/Material/ModularShow.mat", LoadResourcePriority.Default, (handle =>
            {
                ShowModelMaterial = handle.GetAssetObject<Material>();
            }));
            Asset.LoadResourceAsync<Material>("Assets/Assets/Base/Material/ModularModify.mat", LoadResourcePriority.Default, (handle =>
            {
                ModifyModelMaterial = handle.GetAssetObject<Material>();
            }));
        }
        
    }
}