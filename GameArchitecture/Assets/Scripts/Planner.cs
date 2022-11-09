using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class Planner
{
    List<Node> m_planNodes;

    /// <summary>
    /// Create a plan for a specified agent given that agent's available actions, the world state, and the goal state.
    /// Only handles one goal at time.
    /// </summary>
    /// <param name="agent">The agent to plan for</param>
    /// <param name="validActions">All actions this agent can perform</param>
    /// <param name="worldState">The current world state</param>
    /// <param name="goals">This agent's goals.</param>
    /// <returns>The most optimal set of actions for this agent to accomplish their goal</returns>
    public Stack<Actions> CreatePlan(Agents agent, HashSet<Actions> validActions, HashSet<KeyValuePair<string, object>> worldState, HashSet<KeyValuePair<string, object>> goals)
    {
        m_planNodes = new List<Node>();

        // The available actions are no longer completed
        foreach (Actions action in validActions)
        {
            action.done = false;
        }

        // Check pre-conditions to get all actions that can currently be completed
        HashSet<Actions> planActions = new HashSet<Actions>();
        foreach (Actions action in validActions)
        {
            if(action.CheckProceduralPreconditions(agent))
            {
                planActions.Add(action);
            }
        }

        Node root = new Node(null, new List<Node>(), null, 0, worldState);
        m_planNodes.Add(root);

        bool planFound = BuildGraph(root, planActions, goals);
        if(!planFound)
        {
            // This agent's goal is currently not attainable
            return null;
        }

        // Debug
        //agent.DisplayTree(root, null);
        //agent.DisplayTree(root, "Customer");
        //agent.DisplayTree(root, "Waiter");

        // Find the cheapest path to the goal and create a stack of actions (the plan)
        Node cheapest = null;
        float minRunningCost = -1;

        foreach (Node node in m_planNodes)
        {
            // Is this is a child node?
            if(node.children.Count == 0)
            {
                if (cheapest == null)
                {
                    cheapest = node;
                    minRunningCost = node.runningCost;
                }
                else
                {
                    if (node.runningCost < minRunningCost)
                    {
                        cheapest = node;
                        minRunningCost = node.runningCost;
                    }
                }
            }
        }

        Stack<Actions> plan = new Stack<Actions>();
        while(cheapest != null)
        {
            if (cheapest.action != null)
            {
                plan.Push(cheapest.action);
            }
            cheapest = cheapest.parent;
        }

        return plan;
    }

    private bool BuildGraph(Node parent, HashSet<Actions> planActions, HashSet<KeyValuePair<string, object>> goals)
    {
        bool foundGoal = false;

        foreach(Actions action in planActions)
        {
            if(action.m_preconditions.IsSubsetOf(parent.currentState))
            {
                // Apply this action's effects to the current state and apply it to a new node
                HashSet<KeyValuePair<string, object>> currentState = UpdateState(parent.currentState, action.m_effects);
                Node child = new Node(parent, new List<Node>(), action, parent.runningCost + action.cost, currentState);
                
                // Add this child to the current node's children
                parent.children.Add(child);
                m_planNodes.Add(child);

                foreach(KeyValuePair<string, object> goal in goals)
                {
                    if(currentState.Contains(goal))
                    {
                        foundGoal = true;
                    }
                }

                // If goal not found, keep building
                if (!foundGoal)
                {
                    HashSet<Actions> actionSubset = new HashSet<Actions>();
                    actionSubset.UnionWith(planActions);
                    actionSubset.Remove(action);

                    foundGoal = BuildGraph(child, actionSubset, goals);
                }
            }
        }

        return foundGoal;
    }
    
    private HashSet<KeyValuePair<string, object>> UpdateState(HashSet<KeyValuePair<string, object>> current, HashSet<KeyValuePair<string, object>> changes)
    {
        HashSet<KeyValuePair<string, object>> newState = new HashSet<KeyValuePair<string, object>>();
        newState.UnionWith(changes);

        foreach (KeyValuePair<string, object> state in current)
        {
            bool alreadyInCurrentState = false;

            foreach (KeyValuePair<string, object> change in newState)
            {
                // If this state already exists in newState, don't add it
                if (state.Key == change.Key)
                {
                    alreadyInCurrentState = true;
                    break;
                }
            }

            if(!alreadyInCurrentState)
            {
                newState.Add(state);
            }
        }

        return newState;
    }

    //private void Start()
    //{
    //    HashSet<KeyValuePair<string, object>> current = new HashSet<KeyValuePair<string, object>>();
    //    HashSet<KeyValuePair<string, object>> changes = new HashSet<KeyValuePair<string, object>>();

    //    current.Add(new KeyValuePair<string, object>("test1", true));
    //    current.Add(new KeyValuePair<string, object>("test2", false));
    //    current.Add(new KeyValuePair<string, object>("test3", true));
    //    current.Add(new KeyValuePair<string, object>("test4", true));
    //    current.Add(new KeyValuePair<string, object>("test5", false));

    //    changes.Add(new KeyValuePair<string, object>("test2", true));
    //    changes.Add(new KeyValuePair<string, object>("test3", false));
    //    changes.Add(new KeyValuePair<string, object>("test4", true));
    //    changes.Add(new KeyValuePair<string, object>("test6", false));

    //    DisplaySet(UpdateState(current, changes));
    //}
    //void DisplaySet(HashSet<KeyValuePair<string, object>> collection)
    //{
    //    string str = "{";
    //    foreach (KeyValuePair<string, object> i in collection)
    //    {
    //        str += " (" + i.Key + ", " + i.Value + ")";
    //    }
    //    print(str + "}");
    //}
}

public class Node
{
    public Node parent;
    public List<Node> children;
    // The action this node represents
    public Actions action;
    // How much it has costed to get to this node from the start node
    public float runningCost;

    // The state of the world as of this node
    public HashSet<KeyValuePair<string, object>> currentState;

    public Node(Node parent, List<Node> children, Actions action, float runningCost, HashSet<KeyValuePair<string, object>> currentState)
    {
        this.parent = parent;
        this.children = children;
        this.action = action;
        this.runningCost = runningCost;
        this.currentState = currentState;
    }
}