using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // НОВОЕ: Библиотека для управления сценами

namespace Tanks2D
{
    public class Wall : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHP = 100;
        private int _currentHP;

        [Header("UI Elements & Colors")]
        [SerializeField] private Slider wallHPSlider;
        [SerializeField] private Gradient hpGradient;

        [Header("Game Over Settings (Экран конца игры)")]
        [Tooltip("Перетащи сюда объект GameOverPanel из Canvas")]
        [SerializeField] private GameObject gameOverPanel;
        
        [Tooltip("Перетащи сюда кнопку RestartButton из панели конца игры")]
        [SerializeField] private Button restartButton;

        private Image _sliderFillImage;

        private void Start()
        {
            _currentHP = maxHP;

            if (wallHPSlider != null)
            {
                wallHPSlider.minValue = 0;
                wallHPSlider.maxValue = maxHP;
                wallHPSlider.value = maxHP;

                _sliderFillImage = wallHPSlider.fillRect.GetComponent<Image>();
                UpdateSliderColor();
            }

            // Настраиваем кнопку перезапуска, если она привязана
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            // На всякий случай проверяем, что экран Game Over скрыт при старте
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
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
                UpdateSliderColor();
            }

            if (_currentHP <= 0)
            {
                TriggerGameOver();
            }
        }

        private void UpdateSliderColor()
        {
            if (_sliderFillImage != null && hpGradient != null)
            {
                float hpNormalized = (float)_currentHP / maxHP;
                _sliderFillImage.color = hpGradient.Evaluate(hpNormalized);
            }
        }

        // НОВОЕ: Логика остановки игры и включения экрана поражения
        private void TriggerGameOver()
        {
            Debug.Log("[Game Over] СТЕНА РАЗРУШЕНА! Показываем экран проигрыша.");
            
            // Включаем панель на экране
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Ставим игру на полную паузу, замораживая танк, пули и свинок
            Time.timeScale = 0f; 
        }

        // НОВОЕ: Метод перезапуска текущего уровня
        public void RestartGame()
        {
            Debug.Log("[Game] Перезапуск уровня...");
            
            // ОБЯЗАТЕЛЬНО возвращаем время в нормальный режим, иначе новая игра начнется на паузе!
            Time.timeScale = 1f; 

            // Сбрасываем кошелек до нуля, чтобы игрок начинал честно сначала
            Wallet.ResetWallet();

            // Перезагружаем текущую активную сцену
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
