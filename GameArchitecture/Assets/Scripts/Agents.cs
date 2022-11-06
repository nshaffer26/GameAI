using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSMState { IDLE, GOTO, ACTION }

public class Agents : MonoBehaviour
{
    // The current state of this agent
    FSMState state;

    // The starting state of the world
    HashSet<KeyValuePair<string, object>> m_worldState;
    // Customer relevant states
    public bool sittingDown = false;
    public bool isHungry = true;
    // Waiter relevant states
    public int foodHeld = 0;

    // The goal state, will only ever be one for now
    public KeyValuePair<string, object> goal;

    public Seat seat;

    // This agent's speed
    public float speed = 1.0f;

    // All valid actions
    public Stack<Actions> validActions;
    // The current plan, i.e., a set of valid actions to follow
    public Stack<Actions> planActions;

    // This agent's target (either what they should be acting on or moving towards)
    public GameObject target;

    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        m_worldState = new HashSet<KeyValuePair<string, object>>();

        m_worldState.Add(new KeyValuePair<string, object>("sittingDown", sittingDown));
        m_worldState.Add(new KeyValuePair<string, object>("isHungry", isHungry));
        m_worldState.Add(new KeyValuePair<string, object>("hasFood", (foodHeld > 0)));

        return m_worldState;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case FSMState.IDLE:
                // Plan found?
                if (IdleState())
                {
                    //true, state = action
                }
                break;

            case FSMState.GOTO:
                // Target reached?
                if(GoToState())
                {
                    state = FSMState.ACTION;
                }
                break;

            case FSMState.ACTION:
                // More actions left?
                if (ActionState())
                {
                    //true, reqires goto?
                    //  true, state = goto
                    //  false, perform action
                }
                else
                {
                    //false, state = idle
                }
                break;
        }
    }

    bool IdleState()
    {
        //plan, if found return true
        return true;
    }
    bool GoToState()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if(transform.position == target.transform.position)
        {
            return true;
        }

        return false;
    }
    bool ActionState()
    {
        // perform action, if successful, return true
        return true;
    }
}
