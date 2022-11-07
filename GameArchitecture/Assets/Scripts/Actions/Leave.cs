using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : Actions
{
    public Leave()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("isHungry", false));
        m_effects.Add(new KeyValuePair<string, object>("sittingDown", false));
        m_effects.Add(new KeyValuePair<string, object>("eatFood", true));

        actionType = "Leave";
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Find the exit
        agent.target = agent.exit;

        return true;
    }

    public override void Perform(Agents agent)
    {
        // This seat is empty again
        agent.seat.gameObject.tag = "EmptyChair";

        // Update relevant worldState values for this agent
        agent.sittingDown = false;

        // This action is finished
        done = true;
    }
}
