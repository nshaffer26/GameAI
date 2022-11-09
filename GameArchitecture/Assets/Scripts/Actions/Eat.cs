using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : Actions
{
    public Eat()
    {
        m_requiresProximity = false;

        m_preconditions.Add(new KeyValuePair<string, object>("isHungry", true));
        m_effects.Add(new KeyValuePair<string, object>("isHungry", false));

        m_actionType = "Eat";
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Is there food at the table?
        if(agent.m_seat != null && agent.m_seat.m_table.m_foodAvailable > 0)
        {
            return true;
        }
        return false;
    }

    public override void Perform(Agents agent)
    {
        // This agent is no longer waiting for food
        agent.gameObject.tag = "Untagged";

        SpawnFood table = agent.m_seat.m_table;

        // If there is no food to eat, replan
        if(table.m_foodAvailable <= 0)
        {
            agent.gameObject.tag = "Waiting";
            m_done = false;
            return;
        }

        // This agent's table has 1 less food
        table.m_foodAvailable--;
        table.UpdateFoodDisplay();

        // Update relevant worldState values for this agent
        agent.m_isHungry = false;

        // This action is finished
        m_done = true;
    }
}
