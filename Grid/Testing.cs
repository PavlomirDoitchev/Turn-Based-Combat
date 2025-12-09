using UnityEngine;

namespace Assets.Assets.Scripts.Grid
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] Unit unit;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                unit.GetMoveAction().GetValidActionGridPositionList();
            }
        }
    }
}
