using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDetection : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<CharacterController>())
		{
			SceneManager.LoadScene("Win");
		}
	}
}
