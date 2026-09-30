using UnityEngine;
using UnityEngine.UI;

namespace Tanks2D
{
    public class ShopController : MonoBehaviour
    {
        [Header("Shop UI Panel")]
        [SerializeField] private GameObject shopPanel;

        [Header("Control Buttons")]
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        // ГЛОБАЛЬНЫЙ ФЛАГ ПАУЗЫ
        public static bool IsPaused { get; private set; } = false;

        private void Start()
        {
            if (shopPanel != null) shopPanel.SetActive(false);
            Time.timeScale = 1f;
            IsPaused = false; // При старте паузы нет

            if (openButton != null) openButton.onClick.AddListener(OpenShop);
            if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        }

        public void OpenShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                Time.timeScale = 0f;
                IsPaused = true; // ВКЛЮЧАЕМ ПАУЗУ
                Debug.Log("[Shop] Магазин открыт. Игра на паузе.");
            }
        }

        public void CloseShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
                Time.timeScale = 1f;
                IsPaused = false; // ВЫКЛЮЧАЕМ ПАУЗУ
                Debug.Log("[Shop] Магазин закрыт. Игра возобновлена.");
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }
}
