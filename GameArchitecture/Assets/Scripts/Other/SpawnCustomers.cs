using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for spawning customers over time.
/// </summary>
public class SpawnCustomers : MonoBehaviour
{
    // The customer prefab
    public GameObject m_customer;

    // The time since the last customer was spawned
    float m_time;
    // How long to wait until spawning the next customer
    int m_delay;

    // Start is called before the first frame update
    void Start()
    {
        m_time = 0;
        m_delay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn a new customer after a delay
        if (m_time >= m_delay)
        {
            SpawnCustomer();

            m_time = 0;
            m_delay = Random.Range(2, 9);
        }

        m_time += Time.deltaTime;
    }

    /// <summary>
    /// Spawns a customer at this object's position assigns its parent as this object.
    /// </summary>
    void SpawnCustomer()
    {
        Instantiate(m_customer, transform);
    }
}
