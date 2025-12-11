using Assets.Assets.Scripts.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Assets.Scripts.UI
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button actionButton;


        public void SetBaseAction(BaseAction action) 
        {
            textMeshPro.text = action.GetActionName().ToUpper();
            actionButton.onClick.AddListener(() =>
            {
                UnitActionSystem.Instance.SetSelectedAction(action);
            });
        }
    }
}
