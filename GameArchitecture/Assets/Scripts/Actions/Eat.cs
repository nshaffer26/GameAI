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
        // Food nearby
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
