using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitDown : Actions
{
    public SitDown()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("sittingDown", false));
        m_effects.Add(new KeyValuePair<string, object>("sittingDown", true));
        m_effects.Add(new KeyValuePair<string, object>("sitDown", true));

        m_actionType = "SitDown";
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
                minDist = Vector3.Distance(seat.transform.position, agent.transform.position);
            }
            else
            {
                float dist = Vector3.Distance(seat.transform.position, agent.transform.position);
                if (dist < minDist)
                {
                    closest = seat;
                    minDist = dist;
                }
            }
        }
        agent.m_target = closest;

        return agent.m_target != null;
    }

    public override void Perform(Agents agent)
    {
        // Record where this agent is sitting
        agent.m_seat = agent.m_target.GetComponent<Seat>();
        // This seat should no longer have the "EmptyChair" tag
        agent.m_seat.gameObject.tag = "Untagged";
        // This agent is now waiting for food
        agent.gameObject.tag = "Waiting";

        // Update relevant worldState values for this agent
        agent.m_sittingDown = true;

        // This action is finished
        m_done = true;
    }
}
