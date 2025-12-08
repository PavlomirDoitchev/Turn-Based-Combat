using TMPro;
using UnityEngine;

namespace Assets.Assets.Scripts.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;


        private GridObject gridObject;

        public void SetGridObject(GridObject gridObject)
        {
            this.gridObject = gridObject;
        }

        private void Update()
        {
            textMeshPro.text = gridObject.ToString();
        }

    }

}
