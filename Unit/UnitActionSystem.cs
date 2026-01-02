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
                GridCellHighlight.Instance.ConfirmActionAt(mouseGridPosition);
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
        public void AutoDeSelectUnit()
        {
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

            SetSelectedAction(unit.GetAction<MoveAction>());

            RefreshSelectedActionGridVisual();

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetSelectedAction(BaseAction action)
        {
            selectedAction = action;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);

            GridCellHighlight.Instance.SetActionColor(
                GetActionColor(action)
            );

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
        #region Grid Visuals
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
        private Color GetActionColor(BaseAction action)
        {
            return action switch
            {
                MoveAction => new Color(0.3f, 0.6f, 0f, 0.6f),
                AttackAction => new Color(1f, 0.3f, 0.3f, 0.6f),
                HealAction => new Color(0.3f, 1f, 0.3f, 0.6f),
                _ => Color.white
            };
        }
        #endregion
        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            RefreshSelectedActionGridVisual();
        }
    }
}