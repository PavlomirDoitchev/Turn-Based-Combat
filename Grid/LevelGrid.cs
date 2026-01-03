using System.Collections.Generic;
using UnityEngine;

namespace Assets.Assets.Scripts.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private int width;
        [SerializeField] private int height;
        private float cellSize = 2f;

        private GridSystem<GridObject> gridSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one LevelGrid! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }
        private void Start()
        {
            Pathfinding.Instance.Setup(width, height, cellSize);
            //GridGroundMesh.Instance?.RefreshWalkableQuads();
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

        public GridPosition GetGridPosition(Vector3 worldPosition) =>  gridSystem.GetGridPosition(worldPosition);
        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.HasAnyUnit();
        }
        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnit();
        }
    }
}
