using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FSMState { IDLE, GOTO, ACTION }

public class Agents : MonoBehaviour
{
    // The current state of this agent
    [SerializeField] FSMState m_state;

    // The starting state of the world
    HashSet<KeyValuePair<string, object>> m_worldState;
    // Customer relevant states
    public bool sittingDown = false;
    public bool isHungry = true;
    // Waiter relevant states
    public int foodHeld = 0;

    // The goal state, will only ever be one for now
    public HashSet<KeyValuePair<string, object>> goals;

    public Seat seat;

    // This agent's speed
    protected float speed = 1.0f;

    // All valid actions
    public HashSet<Actions> m_validActions = new HashSet<Actions>();
    // The current plan, i.e., a set of valid actions to follow
    public Stack<Actions> m_planActions;

    Planner m_planner;

    // This agent's target (either what they should be acting on or moving towards)
    public GameObject target = null;

    public SpawnFood counter;
    public GameObject exit;

    // Start is called before the first frame update
    void Start()
    {
        m_planActions = new Stack<Actions>();

        m_planner = new Planner();

        counter = GameObject.Find("Counter").GetComponent<SpawnFood>();
        exit = GameObject.Find("Exit");
    }

    // Update is called once per frame
    void Update()
    {
        FSMState oldState = m_state;
        switch (m_state)
        {
            case FSMState.IDLE:
                m_state = IdleState();
                break;

            case FSMState.GOTO:
                m_state = GoToState();
                break;

            case FSMState.ACTION:
                m_state = ActionState();
                break;
        }

        // Debug
        //DisplayStateChange(m_state, oldState);
    }

    FSMState IdleState()
    {
        Stack<Actions> plan = m_planner.CreatePlan(this, m_validActions, GetWorldState(), goals);

        // Debug
        //DisplayActions(plan, null);
        //DisplayActions(plan, "Customer");
        //DisplayActions(plan, "Waiter");

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
        if (target == null)
        {
            return FSMState.IDLE;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (transform.position == target.transform.position)
        {
            return FSMState.ACTION;
        }

        return FSMState.GOTO;
    }
    FSMState ActionState()
    {
        if (m_planActions.Count == 0)
        {
            // No actions left to perform
            return FSMState.IDLE;
        }

        Actions currentAction = m_planActions.Peek();
        currentAction.CheckProceduralPreconditions(this);
        if (target == null)
        {
            return FSMState.IDLE;
        }

        // Does the current action require proximity?
        if (currentAction.requiresProximity && transform.position != target.transform.position)
        {
            return FSMState.GOTO;
        }

        currentAction.Perform(this);
        if (!currentAction.done)
        {
            // The action could not be performed, rerturn to idle state and plan again
            return FSMState.IDLE;
        }
        m_planActions.Pop();

        // Debug
        //print(name + " performs " + currentAction.actionType);

        return FSMState.ACTION;
    }

    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        m_worldState = new HashSet<KeyValuePair<string, object>>();

        m_worldState.Add(new KeyValuePair<string, object>("sittingDown", sittingDown));
        m_worldState.Add(new KeyValuePair<string, object>("isHungry", isHungry));
        m_worldState.Add(new KeyValuePair<string, object>("hasFood", (foodHeld > 0)));
        m_worldState.Add(new KeyValuePair<string, object>("canHoldMoreFood", (foodHeld < 2)));

        // Debug
        //DisplaySet(m_worldState, null);
        //DisplaySet(m_worldState, "Customer");
        //DisplaySet(m_worldState, "Waiter");

        return m_worldState;
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }


    // Debug functions
    public void PrintTest(string toPrint)
    {
        print(toPrint);
    }
    void DisplayStateChange(FSMState newState, FSMState oldState)
    {
        if (newState != oldState)
        {
            print(name + ": " + oldState + " -> " + m_state);
        }
    }
    void DisplaySet(HashSet<KeyValuePair<string, object>> collection, string agentType)
    {
        if (agentType != null && !name.Contains(agentType)) return;

        string str = "{";
        foreach (KeyValuePair<string, object> i in collection)
        {
            str += " (" + i.Key + ", " + i.Value + ")";
        }
        print(name + ": " + str + " }");
    }
    // The following method is adapted from: https://stackoverflow.com/questions/55361628/how-do-i-print-out-a-tree-structure-in-c
    public void DisplayTree(Node tree, string agentType)
    {
        if (agentType != null && !name.Contains(agentType)) return;

        string str = "";

        Stack<Node> stack = new Stack<Node>();
        Stack<int> nodeLevel = new Stack<int>();
        stack.Push(tree);
        nodeLevel.Push(0);

        while (stack.Count > 0)
        {
            Node next = stack.Pop();
            int curLevel = nodeLevel.Pop();

            for (int i = 0; i < curLevel; i++) { str += "-"; }
            str += next.action != null ? next.action.actionType : "Root";
            str += "\n";

            foreach (Node c in next.children)
            {
                nodeLevel.Push(curLevel + 1);
                stack.Push(c);
            }
        }

        print(name + ":\n" + str);
    }
    // The following method is adapted from: https://www.geeksforgeeks.org/print-stack-elements-from-bottom-to-top/
    void DisplayActions(Stack<Actions> collection, string agentType)
    {
        if (agentType != null && !name.Contains(agentType)) return;

        string str = name + ": {";
        if (collection != null)
        {
            Stack<Actions> temp = new Stack<Actions>();
            while (collection.Count != 0)
            {
                temp.Push(collection.Peek());
                collection.Pop();
            }

            while (temp.Count != 0)
            {
                Actions t = temp.Peek();
                str += " " + t;
                temp.Pop();

                // To restore contents of
                // the original stack.
                collection.Push(t);
            }
        }

        print(str + " }");
    }
}
