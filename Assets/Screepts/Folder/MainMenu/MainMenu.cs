using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Подключаем библиотеку для работы со сценами

namespace Tanks2D
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Перетащи сюда кнопку NewGameButton")]
        [SerializeField] private Button newGameButton;

        [Tooltip("Перетащи сюда кнопку LoadGameButton")]
        [SerializeField] private Button loadGameButton;

        [Tooltip("Перетащи сюда кнопку SettingsButton")]
        [SerializeField] private Button settingsButton;

        [Header("Scene Settings")]
        [Tooltip("Имя боевой сцены, которая загрузится при нажатии Новая Игра")]
        [SerializeField] private string gameplaySceneName = "SampleScene"; // Подставьте имя вашей боевой сцены

        private void Start()
        {
            // На всякий случай возвращаем время в норму, если до этого был Game Over
            Time.timeScale = 1f;

            // Привязываем функции к кнопкам
            if (newGameButton != null)
                newGameButton.onClick.AddListener(StartNewGame);

            if (loadGameButton != null)
                loadGameButton.onClick.AddListener(LoadSavedGame);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);
        }

        // 1. НАЖАЛИ "НОВАЯ ИГРА"
        private void StartNewGame()
        {
            Debug.Log("[Menu] Запуск новой игры. Обнуляем кошелек...");
            
            // Сбрасываем кошелек до нуля для честного старта
            global::Wallet.ResetWallet();

            // Загружаем боевую локацию
            SceneManager.LoadScene(gameplaySceneName);
        }

        // 2. НАЖАЛИ "ЗАГРУЗИТЬ ИГРУ"
        private void LoadSavedGame()
        {
            Debug.Log("[Menu] Кнопка 'Загрузить игру' нажата! (Систему сохранений мы прикрутим чуть позже)");
            // Сюда в будущем встанет код загрузки PlayerPrefs или файлов
        }

        // 3. НАЖАЛИ "НАСТРОЙКИ"
        private void OpenSettings()
        {
            Debug.Log("[Menu] Кнопка 'Настройки' нажата! (Окно настроек мы сделаем на следующем этапе)");
            // Сюда в будущем встанет вызов всплывающей панели звука или графики
        }
    }
}
