using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }

        private List<Unit> unitList;
        private List<Unit> playerUnits;
        private List<Unit> NPCUnits;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            unitList = new List<Unit>();
            playerUnits = new List<Unit>();
            NPCUnits = new List<Unit>();
        }
        private void Start()
        {
            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

        }
        
        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e) 
        {
            Unit unit = sender as Unit;
            unitList.Add(unit);
            if (unit.IsNPC())
                NPCUnits.Add(unit);
            else
                playerUnits.Add(unit);
        }
        private void Unit_OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            unitList.Remove(unit);
            if (unit.IsNPC())
                NPCUnits.Remove(unit);
            else
                playerUnits.Remove(unit);
        }

        #region Getters
        public List<Unit> GetUnitList()
        {
            return unitList;
        }
        public List<Unit> GetPlayerUnits()
        {
            return playerUnits;
        }
        public List<Unit> GetNPCUnits()
        {
            return NPCUnits;
        }
        #endregion
    }
}
