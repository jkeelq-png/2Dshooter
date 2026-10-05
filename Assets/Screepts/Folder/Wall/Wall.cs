using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // НОВОЕ: Подключаем пространство имен для работы с текстом

namespace Tanks2D
{
    public class Wall : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHP = 100;
        private int _currentHP;
        private float _visualHP; // НОВОЕ: Переменная, которая будет плавно «догонять» реальное HP

        [Header("UI Elements & Colors")]
        [SerializeField] private Slider wallHPSlider;
        [SerializeField] private Gradient hpGradient;
        
        [Tooltip("Перетащи сюда созданный текст WallHPText из слайдера")]
        [SerializeField] private TextMeshProUGUI wallHPText; // НОВОЕ: Ссылка на текст с цифрами

        [Header("Animation Settings")]
        [Tooltip("Скорость плавного уменьшения полоски (чем выше, тем быстрее)")]
        [SerializeField] private float lerpSpeed = 5f; // НОВОЕ: Скорость анимации

        [Header("Game Over Settings (Экран конца игры)")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        private Image _sliderFillImage;

        private void Start()
        {
            _currentHP = maxHP;
            _visualHP = maxHP; // В начале игры визуальное здоровье равно максимальному

            if (wallHPSlider != null)
            {
                wallHPSlider.minValue = 0;
                wallHPSlider.maxValue = maxHP;
                wallHPSlider.value = maxHP;

                _sliderFillImage = wallHPSlider.fillRect.GetComponent<Image>();
                UpdateSliderVisuals();
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // НОВОЕ: Плавная анимация ползунка
            // Математическая функция Mathf.MoveTowards плавно двигает _visualHP к реальному _currentHP каждый кадр
            if (!Mathf.Approximately(_visualHP, _currentHP))
            {
                _visualHP = Mathf.MoveTowards(_visualHP, _currentHP, lerpSpeed * maxHP * Time.deltaTime);
                
                if (wallHPSlider != null)
                {
                    wallHPSlider.value = _visualHP; // Присваиваем слайдеру промежуточное анимированное значение
                    UpdateSliderVisuals(); // Перекрашиваем полоску в зависимости от анимированного значения
                }
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (_currentHP <= 0) return;

            _currentHP -= damageAmount;
            
            // Если здоровье упало ниже нуля, принудительно округляем до 0
            if (_currentHP < 0) _currentHP = 0;

            Debug.Log($"[Wall] Стене нанесен урон! Осталось HP: {_currentHP}/{maxHP}");

            // Сразу же обновляем цифры на экране (текст меняется мгновенно, а полоска поедет плавно)
            UpdateHPText();

            if (_currentHP <= 0)
            {
                TriggerGameOver();
            }
        }

        // Изменено: Теперь цвет и текст обновляются на основе текущего состояния ползунка
        private void UpdateSliderVisuals()
        {
            if (_sliderFillImage != null && hpGradient != null)
            {
                float hpNormalized = _visualHP / maxHP;
                _sliderFillImage.color = hpGradient.Evaluate(hpNormalized);
            }

            UpdateHPText();
        }

        // НОВОЕ: Метод отображения текущих цифр здоровья
        private void UpdateHPText()
        {
            if (wallHPText != null)
            {
                // Выводим в формате "Текущее / Максимальное" (например: 75 / 100)
                wallHPText.text = $"{_currentHP} / {maxHP}";
            }
        }

        private void TriggerGameOver()
        {
            Debug.Log("[Game Over] СТЕНА РАЗРУШЕНА! Показываем экран проигрыша.");
            
            // Принудительно сбрасываем слайдер и текст в 0, чтобы сгладить остатки анимации
            if (wallHPSlider != null) wallHPSlider.value = 0;
            if (wallHPText != null) wallHPText.text = $"0 / {maxHP}";

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Time.timeScale = 0f; 
        }

        public void RestartGame()
        {
            Debug.Log("[Game] Перезапуск уровня...");
            Time.timeScale = 1f; 
            Wallet.ResetWallet();

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
