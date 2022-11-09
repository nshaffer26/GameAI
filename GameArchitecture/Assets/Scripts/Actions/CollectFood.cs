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

        actionType = "CollectFood";

        cost = 1.0f;
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Is there food at the counter?
        if (agent.counter.GetComponent<SpawnFood>().m_foodAvailable > 0)
        {
            // Yes, the counter is this agent's new target
            agent.target = agent.counter.gameObject;
        }

        return agent.target != null;
    }

    public override void Perform(Agents agent)
    {
        // Take some food from the counter
        agent.counter.m_foodAvailable--;
        agent.counter.UpdateFoodDisplay();

        // Update relevant worldState values for this agent
        agent.foodHeld++;

        // This action is finished
        done = true;
    }
}
