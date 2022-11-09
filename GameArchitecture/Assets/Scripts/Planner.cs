using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// This class is responsible for creating a plan.
/// </summary>
public class Planner
{
    // All nodes in the plan
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
            action.m_done = false;
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

        // Build the root node and add it to the list of nodes in the plan
        Node root = new Node(null, new List<Node>(), null, 0, worldState);
        m_planNodes.Add(root);

        // Build out the plan possibilities
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

    /// <summary>
    /// Builds the graph of all possible plans given a root node, a set of possible actions, and a set of goals.
    /// </summary>
    /// <param name="parent">The root node of this branch.</param>
    /// <param name="planActions">The actions that are valid for this plan.</param>
    /// <param name="goals">This agent's goals.</param>
    /// <returns><c>true</c> if a goal was found, <c>false</c> otherwise.</returns>
    private bool BuildGraph(Node parent, HashSet<Actions> planActions, HashSet<KeyValuePair<string, object>> goals)
    {
        bool foundGoal = false;

        foreach(Actions action in planActions)
        {
            if(action.m_preconditions.IsSubsetOf(parent.currentState))
            {
                // Apply this action's effects to the current state and apply it to a new node
                HashSet<KeyValuePair<string, object>> currentState = UpdateState(parent.currentState, action.m_effects);
                Node child = new Node(parent, new List<Node>(), action, parent.runningCost + action.m_cost, currentState);
                
                // Add this child to the current node's children
                parent.children.Add(child);
                m_planNodes.Add(child);

                // Check if the applied action satisfies a goal
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
    
    /// <summary>
    /// Update the current state with a set of changes.
    /// </summary>
    /// <param name="current">The current world state.</param>
    /// <param name="changes">The new state values to add to/modify the current state.</param>
    /// <returns>The new world state after the changes are applied.</returns>
    private HashSet<KeyValuePair<string, object>> UpdateState(HashSet<KeyValuePair<string, object>> current, HashSet<KeyValuePair<string, object>> changes)
    {
        // Create a new world state and add the changes
        HashSet<KeyValuePair<string, object>> newState = new HashSet<KeyValuePair<string, object>>();
        newState.UnionWith(changes);

        // Add only those states that haven't already been added to the new world state
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
}

/// <summary>
/// A node in a tree of actions
/// </summary>
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