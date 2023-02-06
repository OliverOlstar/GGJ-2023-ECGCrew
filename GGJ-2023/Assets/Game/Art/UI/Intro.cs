using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	[SerializeField]
	private GameObject item1 = null;
	[SerializeField]
	private GameObject item2 = null;

	int t = 0;

    public void Close()
	{
		t++;
		if (t == 1)
		{
			Destroy(item1);
		}
		else if (t == 2)
		{
			SceneManager.LoadScene(1);
		}
	}
}
