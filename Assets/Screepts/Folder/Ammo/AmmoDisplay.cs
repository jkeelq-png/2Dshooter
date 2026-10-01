using UnityEngine;
using TMPro;

namespace Tanks2D
{
    public class AmmoDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Перетащи сюда созданный текстовый объект AmmoText")]
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("References")]
        [Tooltip("Перетащи сюда объект Танка (игрока), на котором висит PlayerController2D")]
        [SerializeField] private PlayerController2D playerController;

        [Header("Settings")]
        [SerializeField] private int maxAmmo = 15; // Должно совпадать с макс. патронами танка

        void Update()
        {
            if (ammoText == null || playerController == null) return;

            // Если танк в процессе перезарядки
            if (playerController.IsReloading)
            {
                ammoText.text = "<color=red>ПЕРЕЗАРЯДКА...</color>";
            }
            else
            {
                // Показываем текущее количество патронов
                ammoText.text = $"Патроны: {playerController.CurrentAmmo} / {maxAmmo}";
            }
        }
    }
}
