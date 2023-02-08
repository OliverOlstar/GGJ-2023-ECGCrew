using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource = null;
	[SerializeField]
	private float delay = 1.0f;
	[SerializeField]
	private int sceneToLoad = 0;
	private bool inputEnabled = false;
	
	private IEnumerator Start()
	{
		inputEnabled = false;
		yield return new WaitForSeconds(delay);
		inputEnabled = true;
	}

	public void InputExit()
	{
		if (inputEnabled == false)
		{
			return;
		}
		SceneManager.LoadScene(sceneToLoad);
	}
}
