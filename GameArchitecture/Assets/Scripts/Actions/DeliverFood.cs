using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeliverFood : Actions
{
    public DeliverFood()
    {
        m_preconditions.Add(new KeyValuePair<string, object>("hasFood", true));
        m_effects.Add(new KeyValuePair<string, object>("canHoldMoreFood", true));
        m_effects.Add(new KeyValuePair<string, object>("serveCustomer", true));

        m_cost = 1.0f;

        m_actionType = "DeliverFood";
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        GameObject[] customers = GameObject.FindGameObjectsWithTag("Waiting");

        // If there are no waiting customers, this action cannot be performed
        if (customers.Length == 0)
        {
            return false;
        }

        // Find the nearest waiting customer and set their table as the target
        GameObject closest = null;
        float minDist = -1;

        foreach (GameObject customer in customers)
        {
            if (closest == null)
            {
                closest = customer;
                minDist = Vector3.Distance(customer.transform.position, agent.transform.position);
            }
            else
            {
                float dist = Vector3.Distance(customer.transform.position, agent.transform.position);
                if (dist < minDist)
                {
                    closest = customer;
                    minDist = dist;
                }
            }
        }

        Seat seat;
        if (closest != null)
        {
            seat = closest.GetComponent<Agents>().m_seat;
            if (seat != null)
            {
                agent.m_target = seat.m_table.gameObject;

                // Adjust cost (DeliverFood takes priority if the closest table is closer than the counter)
                float distToCounter = Vector3.Distance(agent.transform.position, agent.m_counter.transform.position);
                float distToTable = Vector3.Distance(agent.transform.position, agent.m_target.transform.position);

                m_cost = distToTable <= distToCounter ? -1.0f : 1.0f;
            }
        }


        return agent.m_target != null;
    }

    public override void Perform(Agents agent)
    {
        // Give some food to the table
        SpawnFood table = agent.m_target.GetComponent<SpawnFood>();
        table.m_foodAvailable++;
        table.UpdateFoodDisplay();

        // Update relevant worldState values for this agent
        agent.m_foodHeld--;

        // This action is finished
        m_done = true;
    }
}
