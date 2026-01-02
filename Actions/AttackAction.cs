using Assets.Assets.Scripts.AI;
using Assets.Assets.Scripts.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts.Actions
{
    public class AttackAction : BaseAction
    {
        [Flags]
        private enum AttackType
        {
            None = 0,
            Melee = 1 << 0,
            Ranged = 1 << 1,
        }
        private enum State
        {
            Aiming,
            Attacking,
            Cooloff
        }
        [SerializeField] private AttackType attackType;
        [SerializeField] private int meleeAttackRange = 1;
        [SerializeField] private int attackRange = 4;
        [SerializeField] private float stateTimer = 0.5f;
        [SerializeField] private ProjectileType projectileType;
        private State state;

        private Unit targetUnit;
        private bool canAttack;
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
                    FaceEnemy();
                    break;
                case State.Attacking:
                    if (canAttack)
                    {
                        Attack();
                        canAttack = false;
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
                    state = State.Attacking;
                    stateTimer = 0.1f;
                    break;
                case State.Attacking:
                    state = State.Cooloff;
                    stateTimer = unit.GetAnimationController().GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length * .65f;
                    break;
                case State.Cooloff:
                    ActionComplete();
                    unit.GetAnimationController().Play(AnimationState.Idle);
                    break;
            }
        }
        public override string GetActionName()
        {
            return "Attack";
        }
        public override List<GridPosition> GetValidActionGridPositionList() 
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            return GetValidActionGridPositionList(unitGridPosition);
        }
        public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            for (int x = -attackRange; x <= attackRange; x++)
            {
                for (int z = -attackRange; z <= attackRange; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    // Check if within attack range (Manhattan distance)
                    //int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    //if (testDistance > attackRange)
                    //{
                    //    continue;
                    //}


                    //Check if grid pos is empty
                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    // Check if units are on the same team
                    if (targetUnit.IsEnemy() == unit.IsEnemy())
                    {
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);

                }
            }
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);
            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            state = State.Aiming;
            float aimingStateTime = .25f;
            stateTimer = aimingStateTime;

            canAttack = true;
        }
        private void FaceEnemy()
        {
            Vector3 aimDirection = (targetUnit.transform.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 60f);
        }
        private void Attack()
        {
            unit.GetAnimationController().Play(AnimationState.Attack);

            if (attackType.HasFlag(AttackType.Ranged))
            {
                ProjectileManager.Instance.Fire(
                projectileType,
                unit.GetProjectileSpawnPoint(),
                targetUnit,
                () => targetUnit.Damage(40)
                );
            }
            else
            {
                targetUnit.Damage(40);
            }
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 100
            };
        }
        public int GetTargetCountAtPosition(GridPosition gridPosition) 
        {
            return GetValidActionGridPositionList(gridPosition).Count;
        }
    }
}
