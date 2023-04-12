using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour
{
	private void Start()
	{
		if (PlayerPrefs.GetInt("result_gdpr", 0) != 0)
		{
			SceneManager.LoadScene("AppodealDemo");
			return;
		}
		SceneManager.LoadScene("GDPR");
	}
}
