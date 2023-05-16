using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITargetFinder : MonoBehaviour
{
    public string[] targetList;
    public AICharacter mainBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
        foreach (string t in targetList)
        {
            if (!mainBody.curTarget)
            {
                if (other.CompareTag(t))
                {
                    mainBody.SetTarget(other.transform);//GameObject.FindGameObjectWithTag("CameraSystem").transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (mainBody.curTarget)
            mainBody.SetTarget(null);
    }
}
