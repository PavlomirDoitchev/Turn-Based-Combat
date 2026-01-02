using UnityEngine;
using System.Collections.Generic;
using Assets.Assets.Scripts.Grid;
using System;
using Assets.Assets.Scripts.AI;

namespace Assets.Assets.Scripts.Actions
{
    public class MoveAction : BaseAction
    {
        [Header("Stats")]
        [SerializeField] private int maxMoveDistance = 4;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 10f;
        float stoppingDistance = 0.1f;


        private List<Vector3> positionList;
        private int currentPositionIndex = 0;

        private bool isMoving = false;
        public bool IsMoving => isMoving;
        
        private void Update()
        {
            if (!isActive) { return; }
            Vector3 targetPosition = positionList[currentPositionIndex];
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            float dist = Vector3.Distance(transform.position, targetPosition);

            if (dist > stoppingDistance)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    unit.GetAnimationController().Play(AnimationState.Run);
                }

                transform.position += moveDirection * Time.deltaTime * moveSpeed;
            }
            else
            {
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count)
                {
                    //reached final position
                    isMoving = false;
                    isActive = false;
                    unit.GetAnimationController().Play(AnimationState.Idle);
                    ActionComplete();
                    return;
                }
                //if (isMoving)
                //{
                //    isMoving = false;
                //    unit.GetAnimationController().Play(AnimationState.Idle);
                //    ActionComplete();
                //}
            }
        }
        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
            currentPositionIndex = 0;
            positionList = new List<Vector3>();
            foreach (GridPosition pathGridPosition in pathGridPositionList) 
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition)); 
            }
            ActionStart(onActionComplete);
        }
        
        public override List<GridPosition> GetValidActionGridPositionList()
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
                    // check if within move distance
                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    // check if reachable
                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) 
                    {
                        continue;
                    }
                    int pathfindingDistanceMultiplier = 10;
                    if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                    {
                        //Path is too long
                        continue;
                    }
                    //Debug.Log(testGridPosition);

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }

        public override string GetActionName()
        {
            return "Move";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            int targetCountAtGridPosition = unit.GetAction<AttackAction>().GetTargetCountAtPosition(gridPosition);
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10,
            };
        }
    }
}
