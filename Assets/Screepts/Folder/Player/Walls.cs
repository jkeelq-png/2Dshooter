using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    private int _currentHP;

    [Header("UI Elements (Опционально)")]
    [SerializeField] private Slider wallHPSlider; // Слайдер здоровья стены (если захотите полоску на экране)

    private void Start()
    {
        _currentHP = maxHP;

        if (wallHPSlider != null)
        {
            wallHPSlider.minValue = 0;
            wallHPSlider.maxValue = maxHP;
            wallHPSlider.value = maxHP;
        }
    }

    // Метод, который вызывают свинки, чтобы нанести стене урон
    public void TakeDamage(int damageAmount)
    {
        if (_currentHP <= 0) return;

        _currentHP -= damageAmount;
        Debug.Log($"[Wall] Стене нанесен урон! Осталось HP: {_currentHP}/{maxHP}");

        if (wallHPSlider != null)
        {
            wallHPSlider.value = _currentHP;
        }

        if (_currentHP <= 0)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        Debug.Log("[Wall] СТЕНА РАЗРУШЕНА! Игра окончена.");
        // Сюда позже можно будет вставить вызов экрана Game Over
        Destroy(gameObject); 
    }
}
