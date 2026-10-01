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

        public static bool IsPaused { get; private set; } = false;

        private void Start()
        {
            if (shopPanel != null) shopPanel.SetActive(false);
            
            // Убедимся, что при старте игры кнопка открытия точно видна
            if (openButton != null) openButton.gameObject.SetActive(true); 
            
            Time.timeScale = 1f;
            IsPaused = false;

            if (openButton != null) openButton.onClick.AddListener(OpenShop);
            if (closeButton != null) closeButton.onClick.AddListener(CloseShop);
        }

        public void OpenShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                
                // НОВОЕ: Прячем саму кнопку открытия магазина
                if (openButton != null) openButton.gameObject.SetActive(false); 
                
                Time.timeScale = 0f;
                IsPaused = true;
                Debug.Log("[Shop] Магазин открыт. Кнопка «Магазин» скрыта.");
            }
        }

        public void CloseShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
                
                // НОВОЕ: Возвращаем кнопку открытия магазина обратно на экран
                if (openButton != null) openButton.gameObject.SetActive(true); 
                
                Time.timeScale = 1f;
                IsPaused = false;
                Debug.Log("[Shop] Магазин закрыт. Кнопка «Магазин» снова видна.");
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }
}
