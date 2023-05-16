using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public List<Transform> targets;

    public Transform aimTarget;

    public float smooth = 0.3f;
    private Vector3 velocity = Vector3.zero;

    public Camera mainCam;

    public float minZoom = 60f;
    public float maxZoom = 40f;
    public float zoomLimiter = 50f;

    public static PlayerCamera instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        //create new aim transform for aim position
        aimTarget = new GameObject("AimPoint").transform;
        targets.Add(aimTarget);
    }

    private void FixedUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();

    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetGreatestDistance() / zoomLimiter);
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView,newZoom,Time.deltaTime);
    }

    void Move ()
    {
        Vector3 centerPoint = GetCenterPoint();
        transform.position = Vector3.SmoothDamp(transform.position, centerPoint, ref velocity, smooth);
    }

    float GetGreatestDistance ()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint ()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }
        else if (targets.Count < 1)
        {

        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
