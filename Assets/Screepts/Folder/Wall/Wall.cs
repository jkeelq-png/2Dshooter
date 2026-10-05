using UnityEngine;
using UnityEngine.UI;

namespace Tanks2D
{
    public class Wall : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHP = 100;
        private int _currentHP;

        [Header("UI Elements & Colors")]
        [SerializeField] private Slider wallHPSlider;

        // НОВОЕ: Поле для настройки цветов в Инспекторе Unity
        [Tooltip("Настрой цвета здесь: справа налево (от 100% HP до 0% HP)")]
        [SerializeField] private Gradient hpGradient;

        // Внутренняя ссылка на картинку заполнения слайдера, которую мы будем красить
        private Image _sliderFillImage;

        private void Start()
        {
            _currentHP = maxHP;

            if (wallHPSlider != null)
            {
                wallHPSlider.minValue = 0;
                wallHPSlider.maxValue = maxHP;
                wallHPSlider.value = maxHP;

                // Находим компонент Image на объекте Fill внутри Слайдера
                // Обычно он лежит по пути: Slider -> Fill Area -> Fill
                _sliderFillImage = wallHPSlider.fillRect.GetComponent<Image>();
                
                // Сразу красим в максимальный цвет (зелёный) при старте
                UpdateSliderColor();
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (_currentHP <= 0) return;

            _currentHP -= damageAmount;
            Debug.Log($"[Wall] Стене нанесен урон! Осталось HP: {_currentHP}/{maxHP}");

            if (wallHPSlider != null)
            {
                wallHPSlider.value = _currentHP;
                
                // НОВОЕ: Перекрашиваем полоску при каждом получении урона
                UpdateSliderColor();
            }

            if (_currentHP <= 0)
            {
                DestroyWall();
            }
        }

        // Вспомогательный метод для динамического изменения цвета
        private void UpdateSliderColor()
        {
            if (_sliderFillImage != null && hpGradient != null)
            {
                // Считаем процент здоровья от 0.0 до 1.0
                float hpNormalized = (float)_currentHP / maxHP;
                
                // Берем соответствующий цвет из градиента и красим полоску
                _sliderFillImage.color = hpGradient.Evaluate(hpNormalized);
            }
        }

        private void DestroyWall()
        {
            Debug.Log("[Wall] СТЕНА РАЗРУШЕНА! Игра окончена.");
            Destroy(gameObject); 
        }
    }
}
