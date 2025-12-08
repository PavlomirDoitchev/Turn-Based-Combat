using UnityEngine;
using System;
namespace Assets.Assets.Scripts
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }
        public event EventHandler OnSelectedUnitChanged;
        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayer;
        private void Awake()
        {
            if (Instance != null) 
            {
                Debug.LogError("There is more than one UnitActionSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (TryHandleUnitSelection()) return;
                // Block moving the unit while it is already moving
                if (selectedUnit != null && !selectedUnit.IsMoving)
                    selectedUnit?.Move(targetPosition: Mouseworld.GetPosition());

            }
            if (Input.GetMouseButtonDown(1))
            {
                selectedUnit = null;
                OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            }
        }
            private bool TryHandleUnitSelection()
        {
            // Block selecting anything while the selected unit is moving
            if (selectedUnit != null && selectedUnit.IsMoving)
                return false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayer))
            {
                if (hit.transform.TryGetComponent<Unit>(out var unit))
                {
                    SetSelectedUnit(unit);
                    return true;
                }
            }

            return false;
        }
        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }   
    }
}
