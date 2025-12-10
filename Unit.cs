using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        [Header("References")]
        private GridPosition gridPosition;

        // Actions
        private MoveAction moveAction;
        private SpinAction spinAction;


        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            spinAction = GetComponent<SpinAction>();
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
        public GridPosition GetGridPosition() => gridPosition;
        
        // Expose Actions
        public MoveAction GetMoveAction() => moveAction;
        public SpinAction GetSpinAction() => spinAction;
    }
}
