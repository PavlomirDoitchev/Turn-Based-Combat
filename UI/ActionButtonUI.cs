using Assets.Assets.Scripts.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Assets.Scripts;

namespace Assets.Assets.Scripts.UI
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button actionButton;
        [SerializeField] private GameObject selectedVisualGameObject;
        private BaseAction baseAction;
        public void SetBaseAction(BaseAction action) 
        {
            this.baseAction = action;
            textMeshPro.text = action.GetActionName().ToUpper();
            actionButton.onClick.AddListener(() =>
            {
                UnitActionSystem.Instance.SetSelectedAction(action);
            });
        }
        public void UpdateSelectedVisual() 
        {
            BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
            selectedVisualGameObject.SetActive(selectedBaseAction == baseAction);
          
        }
    }
}
