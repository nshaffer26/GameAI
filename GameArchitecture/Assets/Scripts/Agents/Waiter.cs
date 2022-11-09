using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : Agents
{
    public Waiter()
    {
        // Add valid actions for this agent type
        m_validActions.Add(new CollectFood());
        m_validActions.Add(new DeliverFood());

        // Add goals for this agent type
        m_goals = new HashSet<KeyValuePair<string, object>>();
        m_goals.Add(new KeyValuePair<string, object>("collectFood", true));
        m_goals.Add(new KeyValuePair<string, object>("serveCustomer", true));

        m_speed = 2.0f;
    }
}
