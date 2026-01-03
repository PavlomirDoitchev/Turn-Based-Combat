using Assets.Assets.Scripts;
using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.AI;
using Assets.Assets.Scripts.GameSystems;
using Assets.Assets.Scripts.Grid;
using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Waiting,
        TakingTurn,
        Busy
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.Waiting;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
            return;

        switch (state)
        {
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeTurn())
                        state = State.Busy;
                    else
                        TurnSystem.Instance.NextTurn();
                }
                break;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 0.5f;
        }
    }

    private bool TryTakeTurn()
    {
        foreach (Unit enemy in UnitManager.Instance.GetNPCUnits())
        {
            if (TryTakeAction(enemy))
                return true;
        }
        return false;
    }

    private bool TryTakeAction(Unit unit)
    {
        EnemyAIAction bestAction = EvaluateBestAction(unit);
        if (bestAction == null)
            return false;

        if (!unit.TrySpendActionPointsToTakeAction(bestAction.baseAction))
            return false;

        bestAction.baseAction.TakeAction(bestAction.gridPosition, () =>
        {
            state = State.TakingTurn;
            timer = 0.3f;
        });

        return true;
    }
    private EnemyAIAction EvaluateBestAction(Unit unit)
    {
        EnemyAIAction bestAttack = null;
        EnemyAIAction bestMove = null;
        foreach (BaseAction action in unit.GetBaseActionArray())
        {
            if (!unit.CanSpendActionPointsToTakeAction(action))
                continue;

            foreach (GridPosition pos in action.GetValidActionGridPositionList())
            {
                int score = ScoreAction(unit, action, pos);

                EnemyAIAction candidate = new EnemyAIAction
                {
                    baseAction = action,
                    gridPosition = pos,
                    actionValue = score
                };

                if (action is AttackAction)
                {
                    if (bestAttack == null || score > bestAttack.actionValue)
                        bestAttack = candidate;
                }
                else if (action is MoveAction)
                {
                    if (bestMove == null || score > bestMove.actionValue)
                        bestMove = candidate;
                }
            }
        }

        if (bestAttack != null)
            return bestAttack;

        return bestMove;
    }
    private int ScoreAction(Unit unit, BaseAction action, GridPosition pos)
    {
        int score = 0;

        // ATTACK
        if (action is AttackAction attack)
        {
            Unit target = LevelGrid.Instance.GetUnitAtGridPosition(pos);

            score += 1000;

            // Prefer low-health targets
            score += Mathf.RoundToInt(
                (1 - target.GetHealthSystem().GetHealthNormalized()) * 500);

            return score;
        }

        // MOVE
        if (action is MoveAction)
        {
            if (unit.GetAction<AttackAction>()
            .GetValidActionGridPositionList(unit.GetGridPosition())
            .Count > 0)
            {
                // Already can attack — moving is bad
                score -= 500;
            }
            Unit target = AIUtilities.GetClosestPlayer(unit, pos);
            if (target == null)
                return -1000;

            int currentDist =
                AIUtilities.GetDistance(unit.GetGridPosition(), target.GetGridPosition());
            int newDist =
                AIUtilities.GetDistance(pos, target.GetGridPosition());

            int distanceGain = currentDist - newDist;
            score += distanceGain * 10;

            // LOOKAHEAD: can I attack next turn from here?
            int futureAttackScore = unit
                .GetAction<AttackAction>()
                .GetValidActionGridPositionList(pos)
                .Count * 50;

            score += futureAttackScore;

            return score;
        }

        return score;
    }
}
