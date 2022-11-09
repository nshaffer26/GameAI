using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FSMState { IDLE, GOTO, ACTION }

/// <summary>
/// The parent class for all agents.
/// </summary>
public class Agents : MonoBehaviour
{
    // The current state of this agent
    [SerializeField] FSMState m_state;

    // The starting state of the world
    HashSet<KeyValuePair<string, object>> m_worldState;

    // Customer relevant states
    public bool m_sittingDown = false;
    public bool m_isHungry = true;
    // Waiter relevant states
    public int m_foodHeld = 0;

    // This agent's goal states
    public HashSet<KeyValuePair<string, object>> m_goals;

    // This agent's speed
    protected float m_speed = 1.0f;

    // All valid actions for this agent
    public HashSet<Actions> m_validActions = new HashSet<Actions>();
    // The current plan, i.e., a set of valid actions to follow that satisfy all pre-conditions
    public Stack<Actions> m_planActions;

    Planner m_planner;

    // This agent's target (either what they should be acting on or moving towards)
    public GameObject m_target = null;

    public Seat m_seat;
    public SpawnFood m_counter;
    public GameObject m_exit;

    // Start is called before the first frame update
    void Start()
    {
        m_planActions = new Stack<Actions>();

        m_planner = new Planner();

        m_counter = GameObject.Find("Counter").GetComponent<SpawnFood>();
        m_exit = GameObject.Find("Exit");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        //FSMState oldState = m_state;

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

    /// <summary>
    /// Implement the idle state. This is where the plan is formulated.
    /// </summary>
    /// <returns>The <c>FSMState</c> that should be transitioned to next.</returns>
    FSMState IdleState()
    {
        // Create a plan
        Stack<Actions> plan = m_planner.CreatePlan(this, m_validActions, GetWorldState(), m_goals);

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
    /// <summary>
    /// Implement the goto state. This is where an agent moves to their target.
    /// </summary>
    /// <returns>The <c>FSMState</c> that should be transitioned to next.</returns>
    FSMState GoToState()
    {
        // If this agent does not have a target, replan
        if (m_target == null)
        {
            return FSMState.IDLE;
        }

        // Step towards the target
        transform.position = Vector3.MoveTowards(transform.position, m_target.transform.position, m_speed * Time.deltaTime);

        // If this agent has reached their target, move to the action state
        if (transform.position == m_target.transform.position)
        {
            return FSMState.ACTION;
        }

        // Continue to move towards the target
        return FSMState.GOTO;
    }
    /// <summary>
    /// Implement the action state. This is where the agent performs the actions of their plan.
    /// </summary>
    /// <returns>The <c>FSMState</c> that should be transitioned to next.</returns>
    FSMState ActionState()
    {
        // If there are no actions left to perform, replan
        if (m_planActions.Count == 0)
        {
            return FSMState.IDLE;
        }

        // If the current action doesn't have a target, replan
        Actions currentAction = m_planActions.Peek();
        currentAction.CheckProceduralPreconditions(this);
        if (m_target == null)
        {
            return FSMState.IDLE;
        }

        // If the current action require proximity, switch to the goto state
        if (currentAction.m_requiresProximity && transform.position != m_target.transform.position)
        {
            return FSMState.GOTO;
        }

        currentAction.Perform(this);
        if (!currentAction.m_done)
        {
            // The action could not be performed, rerturn to idle state and plan again
            return FSMState.IDLE;
        }
        // Move to the next action
        m_planActions.Pop();

        // Debug
        //print(name + " performs " + currentAction.actionType);

        return FSMState.ACTION;
    }

    /// <summary>
    /// Returns the current world state for this agent.
    /// </summary>
    /// <returns>The current world state for this agent.</returns>
    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        m_worldState = new HashSet<KeyValuePair<string, object>>();

        m_worldState.Add(new KeyValuePair<string, object>("sittingDown", m_sittingDown));
        m_worldState.Add(new KeyValuePair<string, object>("isHungry", m_isHungry));
        m_worldState.Add(new KeyValuePair<string, object>("hasFood", (m_foodHeld > 0)));
        m_worldState.Add(new KeyValuePair<string, object>("canHoldMoreFood", (m_foodHeld < 2)));

        // Debug
        //DisplaySet(m_worldState, null);
        //DisplaySet(m_worldState, "Customer");
        //DisplaySet(m_worldState, "Waiter");

        return m_worldState;
    }

    /// <summary>
    /// Destroy this agent.
    /// </summary>
    public void Despawn()
    {
        Destroy(gameObject);
    }


    // Debug functions for displaying data
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
            str += next.action != null ? next.action.m_actionType : "Root";
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
