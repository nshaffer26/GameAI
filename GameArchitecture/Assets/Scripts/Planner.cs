using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    /// <summary>
    /// Create a plan for a specified agent given that agent's available actions, the world state, and the goal state
    /// </summary>
    /// <param name="agent">The agent to plan for</param>
    /// <param name="validActions">All actions this agent can perform</param>
    /// <param name="worldState">The current world state</param>
    /// <param name="goal">The goal to be satisfied by this plan</param>
    /// <returns>The most optimal set of actions for this agent to accomplish their goal</returns>
    public Stack<Actions> CreatePlan(Agents agent, HashSet<Actions> validActions, HashSet<KeyValuePair<string, object>> worldState, HashSet<KeyValuePair<string, object>> goal)
    {
        return null;
    }
}

class Node
{
    public Node parent;
    // The action this node represents
    public Actions action;
    // How much it has costed to get to this node from the start node
    public float runningCost;

    // The state of the world as of this node
    public HashSet<KeyValuePair<string, object>> currentState;

    public Node(Node parent, Actions action, float runningCost, HashSet<KeyValuePair<string, object>> currentState)
    {
        this.parent = parent;
        this.action = action;
        this.runningCost = runningCost;
        this.currentState = currentState;
    }
}