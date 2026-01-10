using Assets.Assets.Scripts.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Assets.Scripts;
using UnityEngine.EventSystems;

namespace Assets.Assets.Scripts.UI
{
    public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private Button actionButton;
        [SerializeField] private GameObject selectedVisualGameObject;
        [Header("Glow")]
        [SerializeField] private float normalGlow = 1f;
        [SerializeField] private float hoverGlow = 4f;
        [SerializeField] private float glowLerpSpeed = 10f;
        float targetGlow;
        Image image;
        private Material outlineMaterial;
        private BaseAction baseAction;
        private void Awake()
        {
            image = GetComponent<Image>();
            image.material = Instantiate(image.material);
            outlineMaterial = image.material;
            outlineMaterial.SetFloat("_Glow", 0f);
        }
        private void Update()
        {
            float current = outlineMaterial.GetFloat("_Glow");
            outlineMaterial.SetFloat(
                "_Glow",
                Mathf.Lerp(current, targetGlow, Time.deltaTime * glowLerpSpeed)
            );
        }
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
            outlineMaterial.SetFloat("_OutlineAlpha", selectedVisualGameObject.activeSelf ? 1.0f : 0.0f);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            targetGlow = hoverGlow;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetGlow = normalGlow;
        }
    }
}
