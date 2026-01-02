using Assets.Assets.Scripts.AI;
using Assets.Assets.Scripts.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts.Actions
{
    public class HealAction : BaseAction
    {
        private enum State
        {
            Aiming,
            Healing,
            Cooloff
        }

        [Header("Heal Settings")]
        [SerializeField] private int healAmount = 30;
        [SerializeField] private int healRange = 4;
        [SerializeField] private float aimingTime = 0.25f;

        private State state;
        private float stateTimer;
        private Unit targetUnit;
        private bool canHeal;

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.Aiming:
                    FaceTarget();
                    break;

                case State.Healing:
                    if (canHeal)
                    {
                        Heal();
                        canHeal = false;
                    }
                    break;

                case State.Cooloff:
                    break;
            }

            if (stateTimer <= 0f)
            {
                NextState();
            }
        }

        private void NextState()
        {
            switch (state)
            {
                case State.Aiming:
                    state = State.Healing;
                    stateTimer = 0.1f;
                    break;

                case State.Healing:
                    state = State.Cooloff;
                    stateTimer = unit
                        .GetAnimationController()
                        .GetComponentInChildren<Animator>()
                        .GetCurrentAnimatorStateInfo(0).length * 0.65f;
                    break;

                case State.Cooloff:
                    unit.GetAnimationController().Play(AnimationState.Idle);
                    ActionComplete();
                    break;
            }
        }

        public override string GetActionName()
        {
            return "Heal";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);

            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            state = State.Aiming;
            stateTimer = aimingTime;
            canHeal = true;
        }

        private void FaceTarget()
        {
            Vector3 dir = (targetUnit.transform.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 60f);
        }

        private void Heal()
        {
            unit.GetAnimationController().Play(AnimationState.Attack); // or Heal animation later
            targetUnit.GetHealthSystem().Heal(healAmount);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new();
            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -healRange; x <= healRange; x++)
            {
                for (int z = -healRange; z <= healRange; z++)
                {
                    GridPosition offset = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offset;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                        continue;

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                        continue;

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    // Only heal allies
                    if (targetUnit.IsEnemy() != unit.IsEnemy())
                        continue;

                    // Optional: skip full-health units
                    if (targetUnit.GetHealthSystem().GetHealthNormalized() >= 1f)
                        continue;

                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            throw new NotImplementedException();
        }
    }
}
