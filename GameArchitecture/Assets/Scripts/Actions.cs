using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actions
{
    public HashSet<KeyValuePair<string, object>> m_preconditions;
    public HashSet<KeyValuePair<string, object>> m_effects;

    public float cost;
    public bool requiresProximity = true;

    // Has the action been performed
    public bool done = false;

    public string actionType;

    public Actions()
    {
        m_preconditions = new HashSet<KeyValuePair<string, object>>();
        m_effects = new HashSet<KeyValuePair<string, object>>();

        cost = 1f;
    }

    /// <summary>
    /// This method should check this action to see if it can be performed.
    /// </summary>
    /// <param name="agent">The agent performing this action.</param>
    /// <returns><c>true</c> if this precondition is satisfied, <c>false</c> otherwise.</returns>
    public abstract bool CheckProceduralPreconditions(Agents agent);

    /// <summary>
    /// This method should implement the effects of an action and update the world state.
    /// </summary>
    /// <param name="agent">The agent performing this action.</param>
    public abstract void Perform(Agents agent);
}
