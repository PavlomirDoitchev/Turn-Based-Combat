using UnityEngine;
using System.Collections.Generic;
using Assets.Assets.Scripts.Grid;

namespace Assets.Assets.Scripts.Actions
{
    // Attach this to Units that can move
    public class MoveAction : BaseAction
    {
        [SerializeField] private Animator animator;
        

        [Header("Stats")]
        [SerializeField] private int maxMoveDistance = 4;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        float stoppingDistance = 0.1f;


        private Vector3 targetPosition;
        
        private bool isMoving = false;
        public bool IsMoving => isMoving;
        protected override void Awake()
        {
            base.Awake();
            targetPosition = transform.position;
        }
        private void Update()
        {
            if (!isActive) { return; }
            float dist = Vector3.Distance(transform.position, targetPosition);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            if (dist > stoppingDistance)
            {
                GridCellHighlight.Instance.Hide();
                if (!isMoving)
                {
                    isMoving = true;
                    animator.CrossFadeInFixedTime("Walk_Forward", .1f);
                }

                transform.position += moveDirection * Time.deltaTime * moveSpeed;
            }
            else
            {
                if (isMoving)
                {
                    isMoving = false;
                    animator.CrossFadeInFixedTime("idle", .1f);
                    GridCellHighlight.Instance.ShowCells(GetValidActionGridPositionList(), 2f);
                    isActive = false;
                }
            }
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
        public void Move(GridPosition gridPosition)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            isActive = true;
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
