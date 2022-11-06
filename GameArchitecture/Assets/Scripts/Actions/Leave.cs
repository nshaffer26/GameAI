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
