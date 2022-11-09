using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for spawning food over time at the counter and updating the food being displayed at tables.
/// </summary>
public class SpawnFood : MonoBehaviour
{
    public int m_foodAvailable;

    // The food prefab
    public GameObject m_food;

    // A list of the food objects in the scene
    List<GameObject> m_foodObjects;

    // The time since the last customer was spawned
    float m_time;
    // How long to wait until spawning the next customer
    int m_delay;

    // A flag to determine if this is the counter (i.e., it should continually spawn food)
    public bool m_counter = false;

    // Start is called before the first frame update
    void Start()
    {
        m_foodObjects = new List<GameObject>();

        m_time = 0;
        m_delay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Continually spawn food on the counter after a delay
        if (m_counter && m_foodAvailable < 5)
        {
            if (m_time >= m_delay)
            {
                Spawn();

                m_time = 0;
                m_delay = Random.Range(2, 11);
            }
        }
        
        m_time += Time.deltaTime;
    }

    /// <summary>
    /// Spawn a food object at this object's position + an offset assigns its parent as this object.
    /// </summary>
    void Spawn()
    {
        // Apply the offset, add additional height to bring the object above the table/counter
        Vector3 pos = transform.position;
        pos.y += GetComponent<MeshRenderer>().bounds.size.y / 2;

        Vector3 offsetPos = new Vector3(pos.x, pos.y + m_foodAvailable * 1.5f, pos.z);
        m_foodObjects.Add(Instantiate(m_food, offsetPos, Quaternion.identity, transform));

        m_foodAvailable++;
    }

    /// <summary>
    /// Updates the spawned food objects to reflect the amount of food at this object.
    /// </summary>
    public void UpdateFoodDisplay()
    {
        int newFoodCount = m_foodAvailable;

        foreach (GameObject food in m_foodObjects)
        {
            Destroy(food);
        }
        m_foodAvailable = 0;
        m_foodObjects.Clear();

        for (int i = 0; i < newFoodCount; i++)
        {
            Spawn();
        }
    }
}
