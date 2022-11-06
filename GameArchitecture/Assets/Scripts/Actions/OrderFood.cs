using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderFood : Actions
{
    public OrderFood()
    {
        requiresProximity = false;

        m_preconditions.Add(new KeyValuePair<string, object>("sittingDown", true));
        m_preconditions.Add(new KeyValuePair<string, object>("isHungry", true));
        m_preconditions.Add(new KeyValuePair<string, object>("hasOrdered", false));
        m_effects.Add(new KeyValuePair<string, object>("hasOrdered", true));
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // No procedural pre-conditions for this action
        return true;
    }

    public override void Perform(Agents agent)
    {

    }

    public override void ResetAction()
    {
        throw new System.NotImplementedException();
    }
}
