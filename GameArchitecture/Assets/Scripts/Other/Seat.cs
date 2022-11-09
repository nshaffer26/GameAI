using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public SpawnFood m_table;

    void Start()
    {
        m_table = this.GetComponentInParent<SpawnFood>();
    }
}
