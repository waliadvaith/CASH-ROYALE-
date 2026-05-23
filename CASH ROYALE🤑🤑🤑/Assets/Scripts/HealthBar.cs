using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health targetHealth;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            // Subscribe to the health change event
            targetHealth.onHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            // Unsubscribe when disabled to avoid memory leaks
            targetHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    void UpdateHealthBar(float currentHP, float maxHP)
    {
        if (slider != null)
        {
            // This calculates the 0.0 to 1.0 value the slider needs
            slider.value = currentHP / maxHP;
        }
    }
}