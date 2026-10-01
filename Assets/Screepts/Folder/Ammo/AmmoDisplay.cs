using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tanks2D
{
    public class AmmoDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private Slider reloadSlider;

        [Header("References")]
        [SerializeField] private PlayerController2D playerController;

        void Start()
        {
            if (reloadSlider != null) reloadSlider.gameObject.SetActive(false);
        }

        void Update()
        {
            if (playerController == null) return;

            if (playerController.IsReloading)
            {
                if (ammoText != null) ammoText.text = "<color=red>ПЕРЕЗАРЯДКА...</color>";

                if (reloadSlider != null)
                {
                    if (!reloadSlider.gameObject.activeSelf) 
                        reloadSlider.gameObject.SetActive(true);

                    float totalTime = PlayerController2D.CurrentReloadTime;
                    float timeLeft = playerController.ReloadEndTime - Time.time;
                    float progress = Mathf.Clamp01((totalTime - timeLeft) / totalTime);
                    reloadSlider.value = progress;
                }
            }
            else
            {
                if (ammoText != null) ammoText.text = $"Патроны: {playerController.CurrentAmmo} / {PlayerController2D.MaxAmmo}";

                if (reloadSlider != null && reloadSlider.gameObject.activeSelf)
                {
                    reloadSlider.gameObject.SetActive(false); 
                }
            }
        }
    }
}
