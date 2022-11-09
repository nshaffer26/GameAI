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

        actionType = "DeliverFood";

        cost = 3.0f;
    }

    public override bool CheckProceduralPreconditions(Agents agent)
    {
        GameObject[] customers = GameObject.FindGameObjectsWithTag("Waiting");

        // If there are no waiting customers, this action cannot be performed
        if(customers.Length == 0)
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
            seat = closest.GetComponent<Agents>().seat;
            if (seat != null)
            {
                agent.target = seat.m_table.gameObject;
            }
        }

        return agent.target != null;
    }

    public override void Perform(Agents agent)
    {
        // Give some food to the table
        SpawnFood table = agent.target.GetComponent<SpawnFood>();
        table.m_foodAvailable++;
        table.UpdateFoodDisplay();

        // Update relevant worldState values for this agent
        agent.foodHeld--;

        // This action is finished
        done = true;
    }
}
