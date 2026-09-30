using UnityEngine;
using TMPro;

public class WalletUI : MonoBehaviour
{
    [Header("References (Ссылки)")]
    [SerializeField] private TextMeshProUGUI _goldText;

    private void Start()
    {
        if (_goldText != null)
        {
            // Жестко отключаем перенос слов по горизонтали
            _goldText.textWrappingMode = TextWrappingModes.NoWrap;
            _goldText.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }

    private void Update()
    {
        if (_goldText != null)
        {
            // Выводим только число накопленного золота
            _goldText.text = Wallet.TotalGold.ToString();
        }
    }
}
