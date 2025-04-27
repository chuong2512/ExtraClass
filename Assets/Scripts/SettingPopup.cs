using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
	[SerializeField] private GameObject popup;

	[SerializeField] private Image musicOn,mucsicOff;

	[SerializeField] private Image soundOn,soundOff;

	[SerializeField] private Sprite onImage;

	public void MusicChange(bool value)
	{
		this.musicOn.gameObject.SetActive(value);
		this.mucsicOff.gameObject.SetActive(!value);
	}

	public void SoundChange(bool value)
	{
		this.soundOn.gameObject.SetActive(value);
		this.soundOff.gameObject.SetActive(!value);
	}

	public void Show(bool value)
	{
		if(value)
		{
			Singleton<SoundManager>.Instance.Play("Popup");
		}

		this.popup.SetActive(value);
	}
}
