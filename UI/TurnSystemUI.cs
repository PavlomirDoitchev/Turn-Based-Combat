using Assets.Assets.Scripts.GameSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Assets.Scripts.UI
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI turnNumberText;

        private void Start()
        {
            endTurnButton.onClick.AddListener(() =>
            {
                TurnSystem.Instance.NextTurn();
                UpdateTurnNumberText();
            });
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            UpdateTurnNumberText();
        }
        
        private void UpdateTurnNumberText()
        {
            turnNumberText.text = TurnSystem.Instance.GetTurnNumber().ToString();
        }

        private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
        {
            UpdateTurnNumberText();
        }
    }
}
