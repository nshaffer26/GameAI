using System;
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
    public HashSet<Actions> m_validActions = new HashSet<Actions>();
    // The current plan, i.e., a set of valid actions to follow
    public Stack<Actions> m_planActions;

    Planner m_planner;

    // This agent's target (either what they should be acting on or moving towards)
    public GameObject target;

    public GameObject counter;
    public GameObject exit;

    public string agentType;

    // Start is called before the first frame update
    void Start()
    {
        m_planActions = new Stack<Actions>();

        m_planner = new Planner();

        counter = GameObject.Find("Counter");
        exit = GameObject.Find("Exit");
    }

    // Update is called once per frame
    void Update()
    {
        FSMState oldState = state;
        switch(state)
        {
            case FSMState.IDLE:
                state = IdleState();
                break;

            case FSMState.GOTO:
                state = GoToState();
                break;

            case FSMState.ACTION:
                state = ActionState();
                break;
        }
        if(state != oldState)
        {
            print(agentType + ": " + oldState + " -> " + state);
        }
    }

    FSMState IdleState()
    {
        Stack<Actions> plan = m_planner.CreatePlan(this, m_validActions, GetWorldState(), goal);
        DisplayActions(plan);

        if (plan != null)
        {
            // A plan was found
            m_planActions = plan;
            return FSMState.ACTION;
        }
        else
        {
            // A plan could not be found given the current worldState
            return FSMState.IDLE;
        }
    }
    FSMState GoToState()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        print("Target: " + target.name);
        if(transform.position == target.transform.position)
        {
            return FSMState.ACTION;
        }

        return FSMState.GOTO;
    }
    FSMState ActionState()
    {
        if(m_planActions.Count == 0)
        {
            // No actions left to perform
            return FSMState.IDLE;
        }

        Actions currentAction = m_planActions.Peek();
        currentAction.CheckProceduralPreconditions(this);
        print(currentAction.actionType);

        // Does the current action require proximity?
        if (currentAction.requiresProximity && transform.position != target.transform.position)
        {
            return FSMState.GOTO;
        }

        currentAction.Perform(this);
        m_planActions.Pop();

        return FSMState.ACTION;
    }

    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        m_worldState = new HashSet<KeyValuePair<string, object>>();

        m_worldState.Add(new KeyValuePair<string, object>("sittingDown", sittingDown));
        m_worldState.Add(new KeyValuePair<string, object>("isHungry", isHungry));
        m_worldState.Add(new KeyValuePair<string, object>("hasFood", (foodHeld > 0)));
        DisplaySet(m_worldState);
        return m_worldState;
    }

    public void TEST(int toPrint)
    {
        print(toPrint);
    }
    void DisplaySet(HashSet<KeyValuePair<string, object>> collection)
    {
        string str = "{";
        foreach (KeyValuePair<string, object> i in collection)
        {
            str += " (" + i.Key + ", " + i.Value + ")";
        }
        print(str + " }");
    }
    void DisplayActions(Stack<Actions> collection)
    {
        if (collection != null)
        {
            print(agentType + ": {" + PrintStack(collection, "") + " }");
        }
    }
    static string PrintStack(Stack<Actions> s, string str)
    {
        // If stack is empty then return
        if (s.Count == 0)
        {
            return str;
        }

        Actions x = s.Peek();

        // Pop the top element of the stack
        s.Pop();

        // Recursively call the function PrintStack
        PrintStack(s, str);

        // Print the stack element starting
        // from the bottom
        str += " " + x.actionType;

        // Push the same element onto the stack
        // to preserve the order
        s.Push(x);

        return str;
    }
}
