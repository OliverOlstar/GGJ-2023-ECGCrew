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

	private void OnEnable()
	{
		StartCoroutine(EnableInputRoutine());
	}

	private void Update()
	{
		if (inputEnabled == false)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene(sceneToLoad);
		}
	}

	private IEnumerator EnableInputRoutine()
	{
		yield return new WaitForSeconds(delay);
		inputEnabled = true;
	}
}
