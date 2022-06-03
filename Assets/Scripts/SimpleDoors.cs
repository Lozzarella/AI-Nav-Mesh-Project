using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoors : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed = 2;
    public float delay = 2;
    public bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = transform.position+new Vector3(0,-5,0);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (open)
        {
           
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, endPos.y, 2*Time.deltaTime), transform.position.y);

            if (transform.position.y >= endPos.y)
            {
                open = false;
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, startPos.y, 2 * Time.deltaTime), transform.position.y);
            if (transform.position.y >= startPos.y)
            {
                open = true;
            }
        }
    }

}
