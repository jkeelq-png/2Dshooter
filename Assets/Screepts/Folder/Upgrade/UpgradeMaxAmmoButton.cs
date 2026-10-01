using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanks2D
{
    public class UpgradeMaxAmmoButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI upgradeInfoText; 
        [SerializeField] private Button buyButton; 

        [Header("Upgrade Settings")]
        [SerializeField] private string upgradeName = "Размер обоймы";
        [SerializeField] private int basePrice = 30; 
        [SerializeField] private float priceMultiplier = 1.7f;
        [SerializeField] private int ammoIncreasePerLevel = 5; 
        [SerializeField] private int maxAmmoLimit = 50; 

        private int _currentLevel = 1;

        private void Start()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(BuyUpgrade);
            }
            UpdateUI();
        }

        public void BuyUpgrade()
        {
            int cost = CalculatePrice();

            if (PlayerController2D.MaxAmmo >= maxAmmoLimit) return;

            if (Wallet.TrySpendGold(cost))
            {
                _currentLevel++;
                PlayerController2D.MaxAmmo = Mathf.Min(maxAmmoLimit, PlayerController2D.MaxAmmo + ammoIncreasePerLevel); 
                UpdateUI();
            }
        }

        private int CalculatePrice()
        {
            return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, _currentLevel - 1));
        }

        private void UpdateUI()
        {
            if (upgradeInfoText != null)
            {
                if (PlayerController2D.MaxAmmo >= maxAmmoLimit)
                {
                    upgradeInfoText.text = $"{upgradeName} (LVL {_currentLevel})\nМАКС.";
                }
                else
                {
                    upgradeInfoText.text = $"{upgradeName} (LVL {_currentLevel})\nЦена: {CalculatePrice()} зл.";
                }
            }
        }
    }
}
