using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class SlidingDoors : MonoBehaviour
{
    //Field
    private Vector3 startingPoint;//the starting point of the door
    private Vector3 openPosition;//the end position of the door
    [SerializeField] private Vector3 direction = Vector3.down;//the direction the door moves
    [SerializeField] private float distance;//the distance we want to move
    [SerializeField] private float speed = 2f;//how fast the door moves
    [SerializeField] private float waitTime = 3f;//wait time before next move
    private bool isOpen = false;//check to see if the door is open
    private float nextTimeDoorMoves;
    Coroutine running;

    [SerializeField] private bool _isLocked = false;//check to see if the door is able to move

    NavMeshObstacle obstacle;


    public bool IsLocked//property
    {
        set
        {
            _isLocked = value;

            if (value) //value == true
            {
                if (running != null) StopCoroutine(running);
                if (obstacle != null) obstacle.carving = true;
            }
            else
            {
                if (obstacle != null) obstacle.carving = false;
            }
        }
        get
        {
            return _isLocked;
        }
    }

    private void OnValidate()
    {
        if (IsLocked) //IsLocked == true //if the door is locked
        {
            if (running != null) StopCoroutine(running);//and open is trying to happen...stop it ya locked ya bastard
            if (obstacle != null) obstacle.carving = true;//it is an obstacle according to the nav mesh
        }
        else
        {
            if (obstacle != null) obstacle.carving = false;// it is not an obstacle according to the nav mesh
        }
    }

    private void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();//first thing assign the reference for the nave mesh obstacle to this object
    }

    void Start()
    {
        startingPoint = transform.position; //the start position is the position this object starts the game at
        openPosition = transform.position + transform.rotation * (direction.normalized * distance);//from the start point add the distance we want to move and make that our end point

        nextTimeDoorMoves = waitTime;//assign next time door moves to the wait time variable we have stored
    }
    void Update()
    {
        OpenCloseDoor();
    }

    void OpenCloseDoor()
    {
        if (IsLocked)//if we are locked
        {
            return;//get out of here we are done bish
        }

        if (nextTimeDoorMoves >= Time.time)//we reach the time where we should move the door
        {
            nextTimeDoorMoves = Time.time + waitTime;//increase time for next door move

            if (isOpen)//if the door is open we should close yea
            { //Closing

                if (running != null)//when we are closing and we dont know what our coroutine is 
                {
                    StopCoroutine(running);//set our coroutine to running
                }
                running = StartCoroutine(MoveDoor(startingPoint));// start the coroutine
                isOpen = false;//set the check to false so we know we are closing
            }
            else//if the door is closed we should open yea
            {//Opening
              
                if (running != null)//when we are closing and we dont know what our coroutine is 
                {
                    StopCoroutine(running);//set our coroutine to running
                }
                running = StartCoroutine(MoveDoor(openPosition));// start the coroutine
                isOpen = true;//set the check to false so we know we are open

            }
            Debug.Log("OPEN DOOR");
        }
    }

    IEnumerator MoveDoor(Vector3 position)//take in a position
    {
        while (Vector3.Distance(transform.position, position) > 0.01f)//while we are not at goal
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);//move the door
            yield return null; // wait one frame
        }
    }
}
