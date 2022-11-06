using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CollectFood : Actions
{
    public CollectFood()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("hasFood", false));
        m_effects.Add(new KeyValuePair<string, object>("hasFood", true));
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Is there food at the counter?
        if(counter.GetComponent<Counter>().m_foodAvailable > 0)
        {
            // Yes, the counter is this agent's new target
            agent.target = counter;
        }

        return agent.target != null;
    }

    public override void Perform(Agents agent)
    {
        // Give all held food to the table
        agent.target.GetComponent<Seat>().m_table.m_foodAvailable += agent.foodHeld;

        // Update relevant worldState values for this agent
        agent.foodHeld = 0;

        // This action is finished
        done = true;
    }
}
