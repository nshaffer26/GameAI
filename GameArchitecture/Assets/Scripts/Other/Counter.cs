using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int m_foodAvailable;

    // Start is called before the first frame update
    public Counter()
    {
        int m_foodAvailable = 0;
    }

    void Start()
    {
        InvokeRepeating("AddFood", 0, 5);
    }

    void AddFood()
    {
        m_foodAvailable++;
    }
}
