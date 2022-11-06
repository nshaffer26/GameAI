using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool m_empty = true;

    void Update()
    {
        if(m_empty)
        {
            gameObject.tag = "EmptyChair";
        }
        else
        {
            gameObject.tag = null;
        }
    }
}
