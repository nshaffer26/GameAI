using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitDown : Actions
{
    public SitDown()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("sittingDown", false));
        m_effects.Add(new KeyValuePair<string, object>("sittingDown", true));
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Find the nearest seat and take it
        GameObject[] seats = GameObject.FindGameObjectsWithTag("EmptyChair");
        GameObject closest = null;
        float minDist = -1;

        foreach (GameObject seat in seats)
        {
            if (closest == null)
            {
                closest = seat;
                minDist = (seat.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                float dist = (seat.transform.position - agent.transform.position).magnitude;
                if (dist < minDist)
                {
                    closest = seat;
                    minDist = dist;
                }
            }
        }
        agent.target = closest;

        return agent.target != null;
    }

    public override void Perform(Agents agent)
    {
        
    }

    public override void ResetAction()
    {
        throw new System.NotImplementedException();
    }
}
