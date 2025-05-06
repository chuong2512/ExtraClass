using UnityEngine;
using System;
using System.Collections.Generic;

public enum DailyRewardType
{
	Gold,
	Diamond,
	Booster
}

[Serializable]
public class DailyRewardItem
{
	public int             intType=0;
	public DailyRewardType Type => (DailyRewardType) intType;
	public int             amount;
	public float           cooldownHours=24f;
}

public class DailyRewardSystem : Singleton<DailyRewardSystem>
{
	[SerializeField] private List<DailyRewardItem> dailyRewards;
	[SerializeField] private GameManager           gameManager;
	[SerializeField] private CoinItemPool          coinItemPool;
	[SerializeField] private Transform             coinTargetLabel;

	// Key prefix để lưu thời gian nhận thưởng cuối cho mỗi loại phần thưởng
	private const string LAST_CLAIM_PREFIX="LastClaim_";

	private void Start()
	{
		// Khởi tạo thời gian cho phần thưởng chưa được nhận bao giờ
		for(int i=0;i<dailyRewards.Count;i++)
		{
			string key=LAST_CLAIM_PREFIX+i;
			if(!PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.SetString(key,DateTime.MinValue.ToString());
			}
		}
	}

	public bool CanClaimReward(int rewardIndex)
	{
		if(rewardIndex>=dailyRewards.Count) return false;

		string   lastClaimStr=PlayerPrefs.GetString(LAST_CLAIM_PREFIX+rewardIndex);
		DateTime lastClaim   =DateTime.Parse(lastClaimStr);
		DateTime now         =DateTime.Now;

		float cooldownHours=dailyRewards[rewardIndex].cooldownHours;
		return (now-lastClaim).TotalHours>=cooldownHours;
	}

	public void ClaimReward(int rewardIndex)
	{
		if(!CanClaimReward(rewardIndex))
		{
			ShowCooldownMessage(rewardIndex);
			return;
		}

		DailyRewardItem reward=dailyRewards[rewardIndex];

		switch (reward.Type)
		{
			case DailyRewardType.Gold:
				GiveCoins(reward.amount);
				break;

			case DailyRewardType.Diamond:
				GiveDiamonds(reward.amount);
				break;

			case DailyRewardType.Booster:
				GiveBooster(reward.amount);
				break;
		}

		// Lưu thời gian nhận thưởng
		SaveClaimTime(rewardIndex);

		// Hiệu ứng và âm thanh
		PlayRewardEffects();
	}

	private void GiveCoins(int amount)
	{
		double instantCash=GetInstantCash()*amount;
		coinItemPool.Pool(coinTargetLabel,instantCash);
		Notification.instance.Warning($"Received <color=#FFD700FF>{GameUtilities.Currencies.Convert(instantCash)}</color> coins!");
	}

	private void GiveDiamonds(int amount)
	{
		gameManager.SetDiamond(amount);
		Notification.instance.Warning($"Received <color=#00FFDFFF>{amount}</color> diamonds!");
	}

	private void GiveBooster(int duration)
	{/*
		Item booster=new Item
		{
			duration =duration,
			effective=2, // Có thể điều chỉnh theo nhu cầu
			itemCount=1
		};
		Singleton<Inventory>.Instance.Add(booster);
		Notification.instance.Warning($"Received {GameUtilities.DateTime.Convert(duration)} Booster!");
		*/
		
		BoostManager.Instance.WatchAdBoost();
	}

	private void SaveClaimTime(int rewardIndex)
	{
		PlayerPrefs.SetString(LAST_CLAIM_PREFIX+rewardIndex,DateTime.Now.ToString());
		PlayerPrefs.Save();
	}

	public TimeSpan GetRemainingTime(int rewardIndex)
	{
		if(rewardIndex>=dailyRewards.Count) return TimeSpan.Zero;

		string   lastClaimStr=PlayerPrefs.GetString(LAST_CLAIM_PREFIX+rewardIndex);
		DateTime lastClaim   =DateTime.Parse(lastClaimStr);
		DateTime now         =DateTime.Now;

		float    cooldownHours=dailyRewards[rewardIndex].cooldownHours;
		DateTime nextAvailable=lastClaim.AddHours(cooldownHours);

		return nextAvailable>now ? nextAvailable-now : TimeSpan.Zero;
	}

	private void ShowCooldownMessage(int rewardIndex)
	{
		TimeSpan remaining=GetRemainingTime(rewardIndex);
		string   timeStr  =$"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
		Notification.instance.Warning($"Available in: {timeStr}");
		Singleton<SoundManager>.Instance.Play("Notification");
	}

	private void PlayRewardEffects()
	{
		Singleton<SoundManager>.Instance.Play("Purchased");
		// Thêm các hiệu ứng khác nếu cần
	}

	private double GetInstantCash()
	{
		double idleCash=Singleton<DataManager>.Instance.database
			.restaurant[Singleton<DataManager>.Instance.database.targetRestaurant].idleCash;
		return Singleton<GameProcess>.Instance.GetInstantCash(idleCash);
	}

	// UI Helper methods
	public string GetRewardDescription(int rewardIndex)
	{
		if(rewardIndex>=dailyRewards.Count) return string.Empty;

		DailyRewardItem reward=dailyRewards[rewardIndex];
		switch (reward.Type)
		{
			case DailyRewardType.Gold:
				return $"{GameUtilities.Currencies.Convert(GetInstantCash()*reward.amount)}";
			case DailyRewardType.Diamond:
				return $"{reward.amount}";
			case DailyRewardType.Booster:
				return $"{GameUtilities.DateTime.Convert(reward.amount)}";
			default:
				return string.Empty;
		}
	}

	public string GetCooldownText(int rewardIndex)
	{
		if(CanClaimReward(rewardIndex))
			return "CLAIM";

		TimeSpan remaining=GetRemainingTime(rewardIndex);
		return $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
	}
}
