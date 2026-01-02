using UnityEngine;
using System;

namespace Assets.Assets.Scripts.GameSystems
{
    public class TurnSystem : MonoBehaviour
    {
        public static TurnSystem Instance { get; private set; }
        public event EventHandler OnTurnChanged;
        private int turnNumber = 1;
        private bool isPlayerTurn = true;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one TurnSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public void NextTurn()
        {
            isPlayerTurn = !isPlayerTurn;
            UnitActionSystem.Instance.AutoDeSelectUnit();
            if (isPlayerTurn)
                turnNumber++;
            
            OnTurnChanged?.Invoke(this, EventArgs.Empty);
        }
        public int GetTurnNumber()
        {
            return turnNumber;
        }
        public bool IsPlayerTurn()
        {
            return isPlayerTurn;
        }
    }
}