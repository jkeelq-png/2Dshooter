using UnityEngine;
using TMPro;

namespace Tanks2D
{
    public class StatsDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI statsText;

        [Header("References")]
        [SerializeField] private PlayerController2D playerController;

        private void Update()
        {
            if (statsText == null) return;

            // 1. Урон из первой кнопки апгрейда
            int damage = UpgradeButton.BulletDamage;

            // 2. Скорострельность (текущая задержка между выстрелами из магазина)
            float fireRate = PlayerController2D.CurrentFireRate;

            // 3. Время перезарядки обоймы из магазина
            float reloadTime = PlayerController2D.CurrentReloadTime;

            // 4. Максимальные патроны в обойме из магазина
            int maxAmmo = PlayerController2D.MaxAmmo;

            // Выводим все 4 параметра (для дробных чисел используем :F2, чтобы показывать 2 знака после запятой)
            statsText.text = 
                $"<color=#FF4500>⚔️ УРОН:</color> {damage}\n" +
                $"<color=#00BFFF>🏹 ЗАДЕРЖКА ВЫСТРЕЛA:</color> {fireRate:F2} сек.\n" +
                $"<color=#FFD700>⏳ ПЕРЕЗАРЯДКА:</color> {reloadTime:F1} сек.\n" +
                $"<color=#32CD32>🔋 ОБОЙМА:</color> {maxAmmo} патр.";
        }
    }
}
