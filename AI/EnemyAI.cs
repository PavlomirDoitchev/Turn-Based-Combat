using Assets.Assets.Scripts;
using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts.GameSystems;
using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForTurn,
        TakingTurn,
        Busy,
    }
    private State state;
    private void Awake()
    {
        state = State.WaitingForTurn;
    }
    float timer = 1f;
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }



    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        switch (state) 
        {
            case State.WaitingForTurn:
              
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAiAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else 
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:

                break;
        }
       
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 1f;
        }
        timer = 1f;

    }
    private void SetStateTakingTurn() 
    {
        timer = .5f;
        state = State.TakingTurn;
    }
    private bool TryTakeEnemyAiAction(Action onEnemyAIActionComplete) 
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetNPCUnits()) 
        {
            if(TryTakeEnemyAiAction(enemyUnit, onEnemyAIActionComplete))
            return true;
        }
        return false;
    }
    private bool TryTakeEnemyAiAction(Unit unit, Action onEnemyAIActionComplete) 
    {
        SpinAction spinAction = unit.GetSpinAction();
        GridPosition actionGridPosition = unit.GetGridPosition();
        if (!spinAction.IsValidActionGridPosition(actionGridPosition))
        {
            return false;
        }
        if(!unit.TrySpendActionPointsToTakeAction(spinAction))
        {
            return false;
        }
        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }   
}
