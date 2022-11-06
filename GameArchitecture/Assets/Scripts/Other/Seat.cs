using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public Table m_table;

    void Start()
    {
        m_table = this.GetComponentInParent<Table>();
    }
}
