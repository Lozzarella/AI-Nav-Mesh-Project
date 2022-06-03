using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent2StateMachine : MonoBehaviour
{
    #region Variables

    NavMeshAgent m_Agent;
    public State currentState;

    //Animation variables
    [SerializeField] private float _animationStop = 0.01f;
    [SerializeField] private Animator _anim;

    //Variables for navmesh area modifier
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _runSpeed = 15f;

    //Variables for collectables
    [SerializeField] List<GameObject> _collectables;
    float distanceToCollectable = 5f;
    float _randomDistanceRadius = 2f;
    GameObject collectableGoal;

    //count how many times we randomly move
    int _randomMoveCount = 0;

    #endregion

    public enum State//three states the AI moves between
    {
        RandomMove, //AI agent randomly moves
        Searching, //AI agent searchs for collectable
        Collecting, //AI agent collects collectable & unlocks a door
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();//get nav mesh agent component
        _anim = GetComponentInChildren<Animator>();
        NextState(); //statemachine
       
    }

    // Update is called once per frame
    private void Update()
    {
        ChangeAreaSpeed();//runs the change area speed function for different area modifers
        UpdateAnimator();
    }

    void ChangeAreaSpeed()
    {
        NavMeshHit navHit; //checks if we are hitting the nav mesh
        m_Agent.SamplePathPosition(-1, 0.0f, out navHit);
        int RunMask = 1 << NavMesh.GetAreaFromName("Run");//variable for run

        if (navHit.mask == RunMask) //if we are on run mask
        {
            m_Agent.speed = _runSpeed;
            Debug.Log("Need for Speed!");
        }
        else
        {
            m_Agent.speed = _speed;
        }
    }

    private void UpdateAnimator()
    {
        if (m_Agent.velocity.magnitude < _animationStop)
        {
            _anim.SetBool("isWalking", false);
        }
        else
        {
            _anim.SetBool("isWalking", true);
        }
    }
    private void NextState()//moves into states
    {
        switch (currentState)
        {
            case State.RandomMove:
                StartCoroutine(RandomMoveState());
                break;
            case State.Searching:
                StartCoroutine(SearchingState());
                break;
            case State.Collecting:
                StartCoroutine(CollectState());
                break;

        }
    }

    private IEnumerator RandomMoveState()//AI randomly moves 
    {
        m_Agent.destination = m_Agent.transform.position;

        _randomMoveCount = 0;

        while (currentState == State.RandomMove)
        {
            if (_randomMoveCount >= 10)
            {//once we have found a random location 5 times
                currentState = State.Searching;
                continue;//go back to the "while"
            }
            //&& m_Agent.hasPath == true
            if (m_Agent.remainingDistance < 0.1f
                && m_Agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                _randomMoveCount++;

                Debug.Log("Set Random Position");
                Vector3 randomOffset = new Vector3(Random.Range(-_randomDistanceRadius, _randomDistanceRadius),
                                                   0,
                                                   Random.Range(-_randomDistanceRadius, _randomDistanceRadius));
                Vector3 newPosition = m_Agent.transform.position + randomOffset;
                m_Agent.SetDestination(newPosition);
            }

            foreach (GameObject collect in _collectables)
            {
                if (Vector3.Distance(m_Agent.transform.position, collect.transform.position) < distanceToCollectable)
                {
                    collectableGoal = collect;
                    currentState = State.Collecting;
                }
            }
            yield return null;
        }

        NextState();
    }

    private IEnumerator SearchingState()
    {
        while (currentState == State.Searching)//while in searching state
        {
            if (Vector3.Distance(transform.position, _collectables[0].transform.position) < distanceToCollectable)// if collectables does not equal null
            {
                currentState = State.Collecting;//sets current state to collecting
            }

            m_Agent.SetDestination(_collectables[0].transform.position);//set destination to collectable goal
            yield return null;
        }

        NextState();//statemachine coroutine
    }
    private IEnumerator CollectState()//AI goes into collecting state
    {
        Vector3 AIPosition = transform.position;
        Debug.Log("Currently Collecting"); //logs when AI enter collecting state
        while (currentState == State.Collecting)
        {
            Debug.Log("WALK TOWARDS COLLECTABLE HERE");
            if (collectableGoal == null)// if collectable goal is null
            {
                currentState = State.RandomMove;//randomly move
            }
            else
            {
                CollectObect(collectableGoal);

            }
            yield return null;
        }
        Debug.Log("Stopped Collecting");//logs when AI exits collectin state
        NextState();//AI moves to next state
    }


    void CollectObect(GameObject obect)
    {
        DoorKey key = obect.GetComponent<DoorKey>();

        if (key != null)
        {
            key.UnlockDoor();
        }


        m_Agent.SetDestination(obect.transform.position);//set destination to collectable goal
        _collectables.Remove(obect);//remove from collection
        Destroy(obect);//destroys the game object collected by AI

        /*     if (_doorToUnlock == null) return;
             if (other.transform.tag == "AIAgent")
             {
                 _doorToUnlock.IsLocked = false;
             }*/


    }

}



