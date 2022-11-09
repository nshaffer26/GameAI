using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a place in which a customer can sit.
/// </summary>
public class Seat : MonoBehaviour
{
    public SpawnFood m_table;

    void Start()
    {
        m_table = this.GetComponentInParent<SpawnFood>();
    }
}
