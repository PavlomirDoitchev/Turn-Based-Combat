using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("References")]
        private GridPosition gridPosition;
        private MoveAction moveAction;

        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
        }
        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        }

        private void Update()
        {
            
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if(newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }
        public MoveAction GetMoveAction()
        {
            return moveAction;
        }
        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
    }
}
