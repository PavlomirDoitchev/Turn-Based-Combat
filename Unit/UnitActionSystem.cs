using UnityEngine;
using System;
using Assets.Assets.Scripts.Grid;
using Assets.Assets.Scripts.Actions;
using UnityEngine.EventSystems;
using Assets.Assets.Scripts.GameSystems;
namespace Assets.Assets.Scripts
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;
        private Unit selectedUnit;
        private BaseAction selectedAction;

        [SerializeField] private LayerMask unitLayer;

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
        private void Start()
        {
            //SetSelectedUnit(selectedUnit);
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        }
        private void Update()
        {
            if (isBusy) return;
            if (!TurnSystem.Instance.IsPlayerTurn()) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            GridCellHighlight.Instance.UpdateHover(Mouseworld.GetPosition());
            DeSelectUnit();
            //if (selectedUnit == null) return;
            if (TryHandleUnitSelection()) return;
            HandleSelectedAction();

        }
        private void HandleSelectedAction()
        {
            if (Input.GetMouseButtonDown(0) && selectedUnit != null)
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(Mouseworld.GetPosition());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }
                if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    return;
                }
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                RefreshSelectedActionGridVisual();
                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }
        private void DeSelectUnit()
        {
            if (!Input.GetMouseButtonDown(1))
                return;

            if (selectedUnit == null)
                return;
            if (isBusy) return;

            selectedUnit = null;
            selectedAction = null;

            GridCellHighlight.Instance.Hide();
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool TryHandleUnitSelection()
        {
            if (!Input.GetMouseButtonDown(0))
                return false;

            // If we have a selected action, check if click is meant for action targeting
            if (selectedAction != null && selectedUnit != null)
            {
                GridPosition mouseGridPosition =
                    LevelGrid.Instance.GetGridPosition(Mouseworld.GetPosition());

                if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    // Action targeting has priority over unit selection
                    return false;
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayer))
            {
                if (hit.transform.TryGetComponent<Unit>(out var unit))
                {
                    if (unit == selectedUnit)
                        return false;

                    if (unit.IsNPC())
                        return false;

                    SetSelectedUnit(unit);
                    return true;
                }
            }

            return false;
        }
        private void SetSelectedUnit(Unit unit)
        {

            selectedUnit = unit;

            SetSelectedAction(unit.GetMoveAction());

            RefreshSelectedActionGridVisual();

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetSelectedAction(BaseAction action)
        {
            selectedAction = action;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
            //if (action.GetActionPointsCost() > selectedUnit.GetActionPoints())
            //{
            //    GridCellHighlight.Instance.Hide();
            //    return;
            //}
            RefreshSelectedActionGridVisual();
        }
        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }
        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }
        private void SetBusy()
        {
            isBusy = true;
            OnBusyChanged?.Invoke(this, isBusy);
        }
        private void ClearBusy()
        {
            isBusy = false;
            OnBusyChanged?.Invoke(this, isBusy);
            RefreshSelectedActionGridVisual();
        }
        private void RefreshSelectedActionGridVisual()
        {
            if (TurnSystem.Instance.IsPlayerTurn() == false && selectedUnit != null)
            {
                GridCellHighlight.Instance.Hide();
                return;
            }
            if (selectedAction == null)
            {
                GridCellHighlight.Instance.Hide();
                return;
            }

            if (TurnSystem.Instance.IsPlayerTurn() && selectedAction.GetActionPointsCost() > selectedUnit.GetActionPoints() && !isBusy)
            {
                GridCellHighlight.Instance.Hide();
                return;
            }

            var validPositions = selectedAction.GetValidActionGridPositionList();
            GridCellHighlight.Instance.ShowCells(validPositions, 2f);
        }
        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            RefreshSelectedActionGridVisual();
        }
    }
}