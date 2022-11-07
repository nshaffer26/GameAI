using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Agents
{
    public Customer()
    {
        goal = new KeyValuePair<string, object>("sitDown", true);

        m_validActions.Add(new SitDown());
        m_validActions.Add(new Eat());
        m_validActions.Add(new Leave());

        agentType = "Customer";
    }
}
