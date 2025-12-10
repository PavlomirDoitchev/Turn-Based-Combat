using Assets.Assets.Scripts.Actions;
using Assets.Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Assets.Scripts.UI
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainterTransform;

        private void Start()
        {
            CreateUnitActionButtons();
        }
        private void CreateUnitActionButtons() 
        {
            Assets.Scripts.Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            foreach(BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                Instantiate(actionButtonPrefab, actionButtonContainterTransform);
            }
        }
        
    }
}
