using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Assets.Scripts.AI;
namespace Assets.Assets.Scripts.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        protected Unit unit;
        protected bool isActive;
        protected Action onActionComplete;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }
        public abstract string GetActionName();
        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
        public abstract List<GridPosition> GetValidActionGridPositionList();
        public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }
        public virtual int GetActionPointsCost()
        {
            return 1;
        }
        protected void ActionStart(Action onActionComplete)
        {
            isActive = true;
            this.onActionComplete = onActionComplete;
        }
        protected void ActionComplete()
        {
            isActive = false;
            onActionComplete();
        }
        public Unit GetUnit()
        {
            return unit;
        }
        public EnemyAIAction GetBestEnemyAIAction()
        {
            List<EnemyAIAction> enemyAIActions = new List<EnemyAIAction>();
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            foreach (GridPosition gridPosition in validGridPositionList)
            {
                EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
                enemyAIActions.Add(enemyAIAction);
            }
            if (enemyAIActions.Count > 0) // Has possible actions
            {
                enemyAIActions.Sort((a, b) => b.actionValue - a.actionValue); // Decision making: choose the action with the highest action value
                return enemyAIActions[0];
            }
            else // No possible action
                return null;
        }
        public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
    }
}
