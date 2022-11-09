using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CollectFood : Actions
{
    public CollectFood()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("canHoldMoreFood", true));
        m_effects.Add(new KeyValuePair<string, object>("hasFood", true));
        m_effects.Add(new KeyValuePair<string, object>("collectFood", true));

        m_actionType = "CollectFood";

        m_cost = 1.0f;
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Is there food at the counter?
        if (agent.m_counter.GetComponent<SpawnFood>().m_foodAvailable > 0)
        {
            // Yes, the counter is this agent's new target
            agent.m_target = agent.m_counter.gameObject;
        }

        return agent.m_target != null;
    }

    public override void Perform(Agents agent)
    {
        // Take some food from the counter
        agent.m_counter.m_foodAvailable--;
        agent.m_counter.UpdateFoodDisplay();

        // Update relevant worldState values for this agent
        agent.m_foodHeld++;

        // This action is finished
        m_done = true;
    }
}
