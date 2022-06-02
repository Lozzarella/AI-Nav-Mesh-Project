using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent2StateMachine : MonoBehaviour
{
    NavMeshAgent m_Agent;

    [SerializeField] List<GameObject> collectables;
    float distanceToCollectable = 5f;
    float _randomDistanceRadius = 2f;
    GameObject collectableGoal;

    public enum State//two states
    {
        Collecting,
        RandomMove,
    }

    public State currentState;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        NextState();

    }

    private void NextState()//moves into states
    {
        switch(currentState)
        {
            case State.Collecting:
                StartCoroutine(CollectState());
                break;
            case State.RandomMove:
                StartCoroutine(RandomMoveState());
                break;
        }
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
                m_Agent.SetDestination(collectableGoal.transform.position);//set destination to collectable goal
            }
            yield return null;
        }
        Debug.Log("Stopped Collecting");//logs when AI exits collectin state
        NextState();//AI moves to next state
    }

    private IEnumerator RandomMoveState()//AI randomly moves 
    {
        m_Agent.destination = m_Agent.transform.position;

        while (currentState == State.RandomMove)
        {
            //&& m_Agent.hasPath == true
            if (m_Agent.remainingDistance < 0.1f
                && m_Agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Set Random Position");
                Vector3 randomOffset = new Vector3(Random.Range(-_randomDistanceRadius, _randomDistanceRadius),
                                                   0,
                                                   Random.Range(-_randomDistanceRadius, _randomDistanceRadius));
                Vector3 newPosition = m_Agent.transform.position + randomOffset;
                m_Agent.SetDestination(newPosition);
            }

            foreach (GameObject collect in collectables)
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
    
}
