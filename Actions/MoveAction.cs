using UnityEngine;
using System.Collections.Generic;
using Assets.Assets.Scripts.Grid;

namespace Assets.Assets.Scripts.Actions
{
    // Attach this to Units that can move
    public class MoveAction : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private Unit unit;

        [Header("Stats")]
        [SerializeField] private int maxMoveDistance = 4;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        float stoppingDistance = 0.1f;


        private Vector3 targetPosition;

        private bool isMoving = false;
        public bool IsMoving => isMoving;
        private void Awake()
        {
            unit = GetComponent<Unit>();
            targetPosition = transform.position;
        }
        private void Update()
        {
            float dist = Vector3.Distance(transform.position, targetPosition);

            if (dist > stoppingDistance)
            {
                GridCellHighlight.Instance.Hide();
                if (!isMoving)
                {
                    isMoving = true;
                    animator.CrossFadeInFixedTime("Walk_Forward", .1f);
                }

                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * Time.deltaTime * moveSpeed;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                    animator.CrossFadeInFixedTime("idle", .1f);
                    GridCellHighlight.Instance.ShowCells(GetValidActionGridPositionList(), 2f);

                }
            }
        }
        public void Move(GridPosition gridPosition)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        }
        public bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }   
        public List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) 
                    {
                        continue;
                    }

                    //check if we're trying to add the unit's current position
                    if (unitGridPosition == testGridPosition)
                    {
                        continue;
                    }

                    //check if there's any unit on the test grid position
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    //Debug.Log(testGridPosition);

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }
    }
}
