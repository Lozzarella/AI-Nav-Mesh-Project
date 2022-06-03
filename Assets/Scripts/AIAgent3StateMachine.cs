using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent3StateMachine : MonoBehaviour
{
    #region Variables
    NavMeshAgent m_Agent;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _splashSpeed = 1.5f;

    //Animation variables
    [SerializeField] private float _animationStop = 0.01f;
    [SerializeField] private Animator _anim;

    //Collectable variables
    [SerializeField] List <GameObject> _collectables;
    [SerializeField] float _distanceToCollectable = 5f;
    GameObject _collectableGoal;
    public State currentState;
    #endregion

    public enum State
    {
        Searching,
        Collecting,
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        NextState();
    }
    // Update is called once per frame
    void Update()
    {
        ChangeAreaSpeed();//runs the change area speed function for different area modifers
        UpdateAnimator();
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
    void ChangeAreaSpeed()
    {
        NavMeshHit navHit; //checks if we are hitting the nav mesh
        m_Agent.SamplePathPosition(-1, 0.0f, out navHit);
        int SplashMask = 1 << NavMesh.GetAreaFromName("Splash");//variable for splash

        if (navHit.mask == SplashMask) //if we are on splash mask
        {
            m_Agent.speed = _splashSpeed;
            //Debug.Log("You're in the splash zone!");
        }
        else
        {
            m_Agent.speed = _speed;
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Barrel") //if the tag of the collider object is Barrel
        {
            Debug.Log("Barrel Collected");
            Destroy(gameObject);//destroys the game object collected by AI
        }
    }*/
    private void NextState() //function for AI to move between three states
    {
        switch (currentState)
        {
            case State.Searching:
                StartCoroutine(SearchingState());
                break;
            case State.Collecting:
                StartCoroutine(CollectingState());
                break;
        }
    }

    private IEnumerator SearchingState()//Function for searching state
    {
        Debug.Log("Currently Searching"); //logs when AI enters searching state
        while (currentState == State.Searching)//while in searching state
        {
            if (_collectables[0] != null)// if collectables does not equal null
            {
                currentState = State.Collecting;//sets current state to collecting
            }
            
            m_Agent.SetDestination(_collectables[0].transform.position);//set destination to collectable goal
            yield return null;
        }
        Debug.Log("Stopped Searching");
        NextState();
    }

    private IEnumerator CollectingState() //Function for collecting state
    {
        Debug.Log("Currently Collecting"); //logs when AI enters collecting state
        while (currentState == State.Collecting)//while AI is in collecting state
        {
            if (_collectables == null) //if collectables is null and the Ai has not collected collectables
            {
                currentState = State.Searching; //set current state to searching
            }
            if (Vector3.Distance(m_Agent.transform.position, _collectables[0].transform.position) < _distanceToCollectable)// if the position of the AI agent and target goal is less than distance to collectable
            {
                currentState = State.Searching;
                GameObject collectDelete = _collectables[0];//store reference to collectables
                _collectables.RemoveAt(0);//remove from collection
                Destroy(collectDelete);//destroys the game object collected by AI
                
            }

            //foreach (GameObject collect in _collectables)
            //{
            //    if (Vector3.Distance(m_Agent.transform.position, collect.transform.position) < _distanceToCollectable)// if the position of the AI agent and target goal is less than distance to collectable
            //    {
            //        _collectableGoal = collect;
            //        currentState = State.Searching;
            //        _collectables.RemoveAt()
            //        Destroy(_collectables[0]);//destroys the game object collected by AI
            //    }
            //}
            yield return null;
        }
        Debug.Log("Stopped Collecting");
        NextState();
    }
}
