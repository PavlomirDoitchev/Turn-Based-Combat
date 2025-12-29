using Assets.Assets.Scripts.GameSystems;
using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
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
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TurnSystem.Instance.NextTurn();
        }
    }
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        timer = 1f;

    }
}
