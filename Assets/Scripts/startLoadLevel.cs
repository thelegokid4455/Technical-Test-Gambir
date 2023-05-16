using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class startLoadLevel : MonoBehaviour {

	public float waitTime;
	public string levelToLoad;
	float curTimer;
	
	void Start ()
	{	
		curTimer = waitTime;

    }
	
	void Update ()
	{
		curTimer -= Time.deltaTime *1;
		if(curTimer <= 0)
		{
			doObj();
		}
	}

	void doObj ()
	{
		SceneManager.LoadScene(levelToLoad);
    }
}