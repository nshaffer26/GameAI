using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeliverFood : Actions
{
    public DeliverFood()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("hasFood", true));
        m_effects.Add(new KeyValuePair<string, object>("hasFood", false));
        m_effects.Add(new KeyValuePair<string, object>("serveCustomer", true));
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        // Find the nearest waiting customer and set them as the target
        GameObject[] customers = GameObject.FindGameObjectsWithTag("Waiting");
        GameObject closest = null;
        float minDist = -1;

        foreach (GameObject customer in customers)
        {
            if (closest == null)
            {
                closest = customer;
                minDist = (customer.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                float dist = (customer.transform.position - agent.transform.position).magnitude;
                if (dist < minDist)
                {
                    closest = customer;
                    minDist = dist;
                }
            }
        }
        agent.target = closest;

        return agent.target != null;
    }

    public override void Perform(Agents agent)
    {
        // Give all held food to the table
        agent.target.GetComponent<Seat>().m_table.m_foodAvailable += agent.foodHeld;

        // Update relevant worldState values for this agent
        agent.foodHeld = 0;

        // This action is finished
        done = true;
    }
}
