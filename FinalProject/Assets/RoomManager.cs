using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> rooms;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildDungeon(Dictionary<Vector3, List<Vector3>> connections, Vector3 end)
    {
        foreach (KeyValuePair<Vector3, List<Vector3>> k in connections)
        {
            // Determine which connections this room has
            byte[] roomConnections = new byte[4] { 0, 0, 0, 0 };
            foreach (Vector3 v in k.Value)
            {
                if (v.z > k.Key.z)
                {
                    // Connects up
                    roomConnections[0] = 1;
                }
                if (v.x > k.Key.x)
                {
                    // Connects right
                    roomConnections[1] = 1;
                }
                if (v.z < k.Key.z)
                {
                    // Connects down
                    roomConnections[2] = 1;
                }
                if (v.x < k.Key.x)
                {
                    // Connects left
                    roomConnections[3] = 1;
                }
            }

            // Build the room string
            string roomString = "";
            foreach (byte b in roomConnections)
            {
                roomString += b;
            }

            // Choose a room type
            if (k.Key == Vector3.zero)
            {
                // This room is the start
                roomString += " Start";
            }
            else if (k.Key == end)
            {
                // This room is the end
                roomString += " End";
            }
            else
            {
                // Choose room type 1 or 2
                float random = Random.value;
                if (random <= 0.8f)
                {
                    roomString += " (1)";
                }
                else
                {
                    roomString += " (2)";
                }
            }

            // Spawn the room
            foreach (GameObject o in rooms)
            {
                if (o.name == roomString)
                {
                    Instantiate(o, k.Key, o.transform.rotation, transform);
                    break;
                }
            }
        }
    }
}
