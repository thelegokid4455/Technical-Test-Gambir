using UnityEngine;
using System.Collections;

public class lightFlicker : MonoBehaviour {

	[SerializeField] bool turnOff;
	public GameObject lightObj;
	public GameObject meshObj;
	
	public float minIntensity;
	public float maxIntensity;
	private float curIntensity;
	
	public float maxInterval;
	public float interval;
	
	private float count;
	
	void  Start ()
	{
		if(!lightObj)
			lightObj = gameObject;
	}
	
	void  Update ()
	{
		lightObj.GetComponent<Light>().intensity = curIntensity;
		
		if(count <= 0)
		{
			if(turnOff)
			{
				curIntensity = curIntensity == maxIntensity ? minIntensity : maxIntensity;
			}
			else
            {
				curIntensity = Random.Range(minIntensity, maxIntensity);
			}

			if(meshObj)
				meshObj.SetActive(!meshObj.activeSelf);

			count = Random.Range(interval, maxInterval);
		}
		else
		{
			count -= Time.deltaTime * 1;
		}
	}
	
}