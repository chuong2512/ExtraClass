using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardButton : MonoBehaviour
{
	[SerializeField] private int    rewardIndex;
	DailyRewardSystem           rewardSystemManager;
	[SerializeField] private Text   descriptionText;
	[SerializeField] private Text   cooldownText;
	[SerializeField] private Button claimButton;

	private void Start()
	{
		rewardSystemManager=DailyRewardSystem.Instance;

		UpdateUI();
		StartCoroutine(UpdateTimer());
	}

	private void UpdateUI()
	{
		descriptionText.text    =rewardSystemManager.GetRewardDescription(rewardIndex);
		cooldownText.text       =rewardSystemManager.GetCooldownText(rewardIndex);
		claimButton.interactable=rewardSystemManager.CanClaimReward(rewardIndex);
	}

	private IEnumerator UpdateTimer()
	{
		while (true)
		{
			UpdateUI();
			yield return new WaitForSeconds(1f);
		}
	}

	public void OnClaimButtonClick()
	{
		rewardSystemManager.ClaimReward(rewardIndex);
		UpdateUI();
	}
}
