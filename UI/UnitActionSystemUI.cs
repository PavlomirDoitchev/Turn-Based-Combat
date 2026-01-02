using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Assets.Assets.Scripts.GameSystems;
namespace Assets.Assets.Scripts.UI
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainterTransform;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private UnitPortraitCamera portraitCamera;
        private List<ActionButtonUI> actionButtonsUIList;
        private void Awake()
        {
            actionButtonsUIList = new List<ActionButtonUI>();
        }
        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

            CreateUnitActionButtons();
            UpdateSelectedVisual();
        }
        private void CreateUnitActionButtons()
        {

            foreach (Transform buttonTransform in actionButtonContainterTransform)
            {
                Destroy(buttonTransform.gameObject);
            }
            actionButtonsUIList.Clear();
            if (UnitActionSystem.Instance.GetSelectedUnit() == null)
                return;
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainterTransform);
                ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(baseAction);

                actionButtonsUIList.Add(actionButtonUI);
            }
        }
        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }
        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }
        private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
        {
            UpdateActionPoints();
        }
        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit == null)
            {
                portraitCamera.SetUnit(null);
                portraitCamera.transform.position = new Vector3(0, -100, 0);


            }
            portraitCamera.SetUnit(selectedUnit);

            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoints();
        }
        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }
        private void UpdateSelectedVisual()
        {
            foreach (ActionButtonUI actionButtonUI in actionButtonsUIList)
            {
                actionButtonUI.UpdateSelectedVisual();
            }
        }
        private void UpdateActionPoints()
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit != null)
            {
                actionPointsText.text = selectedUnit.GetActionPoints().ToString();
            }
            else
                actionPointsText.text = "";
        }
    }
}