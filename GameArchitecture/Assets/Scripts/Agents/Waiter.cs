using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : Agents
{
    public Waiter()
    {
        m_validActions.Add(new CollectFood());
        m_validActions.Add(new DeliverFood());

        goals = new HashSet<KeyValuePair<string, object>>();
        goals.Add(new KeyValuePair<string, object>("serveCustomer", true));

        speed = 3.0f;
    }
}
