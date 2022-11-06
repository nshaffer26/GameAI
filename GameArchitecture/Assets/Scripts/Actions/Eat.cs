using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : Actions
{
    public Eat()
    {
        requiresProximity = false;

        m_preconditions.Add(new KeyValuePair<string, object>("isHungry", true));
        m_effects.Add(new KeyValuePair<string, object>("isHungry", false));
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Is there food at the table?
        if(agent.target.GetComponent<Seat>().m_table.m_foodAvailable > 0)
        {
            return true;
        }
        return false;
    }

    public override void Perform(Agents agent)
    {
        // This agent's table has 1 less food
        agent.seat.m_table.m_foodAvailable--;

        // Update relevant worldState values for this agent
        agent.isHungry = false;

        // This action is finished
        done = true;
    }
}
