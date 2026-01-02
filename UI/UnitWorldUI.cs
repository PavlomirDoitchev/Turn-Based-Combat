using Assets.Assets.Scripts.GameSystems;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Assets.Scripts.UI
{
    public class UnitWorldUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Unit unit;
        [SerializeField] private Image healthBarImage;

        [Header("Health Bar Animation")]
        [SerializeField] private float healthBarLerpSpeed = 8f;

        [Header("Health Bar Flash")]
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private Color healFlashColor = Color.green;
        [SerializeField] private float flashDuration = 0.1f;

        private HealthSystem healthSystem;
        private float targetFillAmount;
        private Color originalHealthBarColor;
        private Coroutine flashCoroutine;

        private void Start()
        {
            UpdateActionPointsText();
            Unit.OnAnyActionPointsChanged += Unit_OnActionPointsChanged;

            healthSystem = unit.GetHealthSystem();
            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
            healthSystem.OnDamaged += HealthSystem_OnDamaged;
            healthSystem.OnDied += HealthSystem_OnDied;
            healthSystem.OnHealed += HealthSystem_OnHealed;

            originalHealthBarColor = healthBarImage.color;

            targetFillAmount = healthSystem.GetHealthNormalized();
            healthBarImage.fillAmount = targetFillAmount;
        }
        private void Awake()
        {
            if (unit.IsNPC()) 
            {
                actionPointsText.enabled = false;
            }
        }

        private void Update()
        {
            healthBarImage.fillAmount = Mathf.Lerp(
                healthBarImage.fillAmount,
                targetFillAmount,
                Time.deltaTime * healthBarLerpSpeed
            );
        }


        private void Unit_OnActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointsText();
        }

        private void UpdateActionPointsText()
        {
            actionPointsText.text = unit.GetActionPoints().ToString();
        }


        private void HealthSystem_OnHealthChanged(int currentHealth, int maxHealth)
        {
            targetFillAmount = healthSystem.GetHealthNormalized();
        }

        private void HealthSystem_OnDamaged()
        {
            StartFlash(damageFlashColor);
        }
        private void HealthSystem_OnDied() 
        {
            healthBarImage.enabled = false;
            actionPointsText.enabled = false;
            Destroy(gameObject, .1f);
        }
        private void HealthSystem_OnHealed()
        {
            StartFlash(healFlashColor);
        }

        private void StartFlash(Color flashColor)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(FlashRoutine(flashColor));
        }

        private IEnumerator FlashRoutine(Color flashColor)
        {
            healthBarImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            healthBarImage.color = originalHealthBarColor;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            Unit.OnAnyActionPointsChanged -= Unit_OnActionPointsChanged;

            if (healthSystem != null)
            {
                healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
                healthSystem.OnDamaged -= HealthSystem_OnDamaged;
                healthSystem.OnHealed -= HealthSystem_OnHealed;
                healthSystem.OnDied -= HealthSystem_OnDied;

            }
        }
    }
}
