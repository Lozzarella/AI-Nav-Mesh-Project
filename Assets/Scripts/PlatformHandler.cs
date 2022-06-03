using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    public Vector3[] points;
    public int pointNumber = 0;
    private Vector3 currentTarget;

    public float tolerance;
    public float speed;
    public float delayTime;
    public float delayStart;

    public bool automatic;

    // Start is called before the first frame update
    void Start()
    {
        if (points.Length > 0)
        {
            currentTarget = points[0];
        }
        tolerance = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition != currentTarget)//if platform is not at current target
        {
            MovePlatform();

        }
        else
        {
            UpdateTarget();
        }
    }

    void MovePlatform()//how to move platform
    {
        //what direction to travel
        Vector3 heading = currentTarget - transform.localPosition;
        transform.localPosition += (heading / heading.magnitude) * speed * Time.deltaTime;
        if (heading.magnitude < tolerance)
        {
            transform.localPosition = currentTarget; //snap target back into position
            delayStart = Time.time;
        }
    }

    void UpdateTarget()
    {
        //if platform moves automatic
        if (automatic)
        {
            if (Time.time - delayStart > delayTime)
            {
                NextPlatform();
            }
        }
    }
    public void NextPlatform()
    {
        pointNumber++;
        if (pointNumber >= points.Length)
        {
            pointNumber = 0;
        }
        currentTarget = points[pointNumber];
    }



}
