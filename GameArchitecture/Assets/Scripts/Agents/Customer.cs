using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Agents
{
    public Customer()
    {
        m_validActions.Add(new SitDown());
        m_validActions.Add(new Eat());
        m_validActions.Add(new Leave());

        goals = new HashSet<KeyValuePair<string, object>>();
        goals.Add(new KeyValuePair<string, object>("sitDown", true));
        goals.Add(new KeyValuePair<string, object>("eatFood", true));

        speed = 2.0f;
    }
}
