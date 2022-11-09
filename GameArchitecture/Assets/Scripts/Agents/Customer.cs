using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Agents
{
    public Customer()
    {
        // Add valid actions for this agent type
        m_validActions.Add(new SitDown());
        m_validActions.Add(new Eat());
        m_validActions.Add(new Leave());

        // Add goals for this agent type
        m_goals = new HashSet<KeyValuePair<string, object>>();
        m_goals.Add(new KeyValuePair<string, object>("sitDown", true));
        m_goals.Add(new KeyValuePair<string, object>("eatFood", true));

        m_speed = 2.0f;
    }
}
