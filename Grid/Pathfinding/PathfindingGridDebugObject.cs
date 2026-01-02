using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Assets.Assets.Scripts.Grid
{
    public class PathfindingGridDebugObject : GridDebugObject
    {
        [SerializeField] private TextMeshPro gCostText, hCostText, fCostText;
        private PathNode pathNode;
        public override void SetGridObject(object gridObject)
        {
            pathNode = (PathNode)gridObject;
            base.SetGridObject(gridObject); 
        }
        protected override void Update()
        {
            base.Update();
            gCostText.text = "G: " + pathNode.GetGCost().ToString();
            hCostText.text = "H: " + pathNode.GetHCost().ToString();
            fCostText.text = "F: " + pathNode.GetFCost().ToString();
        }
    }
}
