using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : Agents
{
    public Customer()
    {
        goal = new KeyValuePair<string, object>("eatFood", true);
    }
}
