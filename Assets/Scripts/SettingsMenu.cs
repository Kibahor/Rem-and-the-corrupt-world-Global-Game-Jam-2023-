using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
	public AudioMixer audioMixer;
	public Resolution[] resolutions;
	public Dropdown resolutionDropdown;

	public void Start()
	{
		resolutions = Screen.resolutions.Select(Resolution => new Resolution { width = Resolution.width, height = Resolution.height }).Distinct().ToArray(); //Get available resolutions and uniq list
		resolutionDropdown.ClearOptions();
		List<string> options = new List<string>();
		int currentResolutionIndex = 0;
		for (int i = 0; i < resolutions.Length; i++)
		{
			options.Add(resolutions[i].width + "x" + resolutions[i].height);
			if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
			{
				currentResolutionIndex = i;
			}
		}
			
		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();

	}

	public void SetVolume(float volume)
	{
		audioMixer.SetFloat("volume", volume);
	}

	public void SetFullScreen(bool isFullScreen)
	{
		Screen.fullScreen = isFullScreen;
	}

	public void SetResolution(int index)
	{
		Resolution resolution = resolutions[index];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}
}
