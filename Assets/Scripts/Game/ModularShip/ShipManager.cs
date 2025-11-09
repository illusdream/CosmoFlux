using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ilsFramework.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Sirenix.OdinInspector;

namespace Game
{
    public class ShipManager : ManagerSingleton<ShipManager>
    {
        [ShowInInspector]
        Dictionary<uint,ShipCore> ships = new Dictionary<uint, ShipCore>();

        private uint index = 0;
        
        [ShowInInspector]
        public static ShipCore PlayerControlShip { get; private set; }
        
        [ShowInInspector]
        public static ShipCore BuildTargetShip { get;  set; }
        
        public override IEnumerator OnInit()
        {
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

        public void RegisterShip(ShipCore ship)
        {
            if (!ships.Any())
            {
                PlayerControlShip = ship;
            }
            ships[index] = ship;
            ship.ID = index;
            index++;
        }

        public bool QueryShip(uint shipID,out ShipCore ship)
        {
            return ships.TryGetValue(shipID, out ship);
        }

        public bool QueryModular(uint shipID, uint modularInstanceID, out BaseModularNode modularNode)
        {
            modularNode = null;
            return QueryShip(shipID,out var ship) && ship.QueryModular(modularInstanceID,out modularNode);
        }
    }
}