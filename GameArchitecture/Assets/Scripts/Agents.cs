using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FSMState { IDLE, GOTO, ACTION }

public class Agents : MonoBehaviour
{
    // The current state of this agent
    FSMState state;

    // This agent's speed
    float speed = 1.0f;

    // All valid actions
    public Stack<Actions> validActions;
    // The current plan, i.e., a set of valid actions to follow
    public Stack<Actions> planActions;

    // This agent's target (either what they should be acting on or moving towards)
    public GameObject target;

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
                    //false, state = idle, plan
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
