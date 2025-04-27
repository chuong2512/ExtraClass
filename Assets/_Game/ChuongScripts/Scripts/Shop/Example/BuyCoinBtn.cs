using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChuongCustom
{
    public class BuyCoinBtn : BaseIAPButton
    {
        [SerializeField] private int _amount;
        [FormerlySerializedAs("_button")] [SerializeField] private Button _Abutton;
        [SerializeField] private TextMeshProUGUI _amountText;

        private PlayerData _player;

        protected override void OnStart()
        {
            _player = GameDataManager.Instance.playerData;

            _Abutton.onClick.AddListener(OnClickButton);

            _amountText.text = $"x{_amount}";
        }

        protected override void OnBuySuccess()
        {
            _player.Coin += _amount;
        }
    }
}