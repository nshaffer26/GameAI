using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : Agents
{
    public Waiter()
    {
        goal = new KeyValuePair<string, object>("serveCustomer", true);

        // TODO: Keep different speed?
        speed = 2.0f;

        m_validActions.Add(new CollectFood());
        m_validActions.Add(new DeliverFood());

        agentType = "Waiter";
    }
}
