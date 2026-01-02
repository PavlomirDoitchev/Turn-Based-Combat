using Assets.Assets.Scripts.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts.Actions
{
    public class SpinAction : BaseAction
    {
        public delegate void SpinCompleteDelegate();
        private float totalSpinAmount;
        private void Update()
        {
            if (!isActive)
            {
                return;
            }
            transform.Rotate(Vector3.up, 360 * Time.deltaTime);
            totalSpinAmount += 360 * Time.deltaTime;
            if (totalSpinAmount >= 360f)
            {
                isActive = false;
                onActionComplete();
            }
        }
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            return new List<GridPosition> { unitGridPosition };
        }
        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            this.onActionComplete = onActionComplete;
            isActive = true;
            totalSpinAmount = 0f;
        }

        public override string GetActionName()
        {
            return "Spin";
        }
        public override int GetActionPointsCost()
        {
            return 1;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 0
            };
        }
    }
}
