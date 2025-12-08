using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }
        [SerializeField] private Transform gridDebugObjectPrefab;
        GridSystem gridSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one LevelGrid! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            gridSystem = new GridSystem(10, 10, 2f);
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }
        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);

        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }
        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return gridSystem.GetGridPosition(worldPosition);
        }
        /// <summary>
        /// Sets the unit's new grid position and clears its old grid position.
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="fromGridPos"></param>
        /// <param name="toGridPos"></param>
        public void UnitMoveGridPosition(Unit unit, GridPosition fromGridPos, GridPosition toGridPos) 
        {
            RemoveUnitAtGridPosition(fromGridPos, unit);
            AddUnitAtGridPosition(toGridPos, unit);
        }
    }
}
