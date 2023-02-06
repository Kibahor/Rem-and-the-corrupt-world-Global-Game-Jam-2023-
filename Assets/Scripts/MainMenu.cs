using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject SettingsWindow;
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void SettingsButton(bool state)
	{
		SettingsWindow.SetActive(state);
	}

	public void QuitButton()
	{
		Application.Quit();
	}
}
