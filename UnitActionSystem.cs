using UnityEngine;
using System;
using Assets.Assets.Scripts.Grid;
using Assets.Assets.Scripts.Actions;
using UnityEngine.EventSystems;
namespace Assets.Assets.Scripts
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }
        public event EventHandler OnSelectedUnitChanged;
        private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayer;
        private BaseAction selectedAction;
        private bool isBusy;
        private void Awake()
        {
            selectedUnit = null;
            if (Instance != null)
            {
                Debug.LogError("There is more than one UnitActionSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        //private void Start()
        //{
        //    SetSelectedUnit(selectedUnit);
        //}
        private void Update()
        {
            if (isBusy) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            DeSelectUnit();
            //if (selectedUnit == null) return;
            if (TryHandleUnitSelection()) return;
            HandleSelectedAction();
           
        }
        private void HandleSelectedAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(Mouseworld.GetPosition());
                if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    SetBusy();
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                }
            }
        }
        private void DeSelectUnit()
        {
            if (Input.GetMouseButtonDown(1))
            {
                selectedUnit = null;
                GridCellHighlight.Instance.Hide();
                OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool TryHandleUnitSelection()
        {
            // Block selecting anything while the selected unit is moving
            //if (selectedUnit != null && selectedUnit.IsMoving)
            //    return false;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayer))
                {
                    if (hit.transform.TryGetComponent<Unit>(out var unit))
                    {
                        if(unit == selectedUnit)
                            return false;
                        SetSelectedUnit(unit);
                        //GridCellHighlight.Instance.ShowCells(unit.GetMoveAction().GetValidActionGridPositionList(), 2f);
                        return true;
                    }
                }
            }

            return false;
        }
        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
            SetSelectedAction(unit.GetMoveAction());

            var moveAction = unit.GetMoveAction();
            var validPositions = moveAction.GetValidActionGridPositionList();

            GridCellHighlight.Instance.ShowCells(validPositions, 2f);

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetSelectedAction(BaseAction action)
        {
            selectedAction = action;
        }
        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }
        private void SetBusy()
        {
            isBusy = true;
        }
        private void ClearBusy()
        {
            isBusy = false;
        }
    }
}