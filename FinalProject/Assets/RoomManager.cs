using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // A list of all possible rooms, assigned in the editor
    public List<GameObject> rooms;

    /// <summary>
    /// Assigns a room to each vertex in the dungeon based on it's connections to other rooms
    /// </summary>
    /// <param name="connections">A dictionary of vertices where each key is a unique vertex in the dungeon
    ///     and each value a list of vertices that connect to the key</param>
    /// <param name="end">The last vertex in the dungeon</param>
    public void BuildDungeon(Dictionary<Vector3, List<Vector3>> connections, Vector3 end)
    {
        Vector3 prev = Vector3.zero;
        foreach (KeyValuePair<Vector3, List<Vector3>> k in connections)
        {
            // Determine which connections this room has and assign the previous vertex
            byte[] roomConnections = new byte[4] { 0, 0, 0, 0 };
            bool first = true;
            foreach (Vector3 v in k.Value)
            {
                if (first)
                {
                    // The first connection is always the previous vertex
                    first = false;
                    prev = v;
                }

                if (v.z > k.Key.z)
                {
                    // Connects forward
                    roomConnections[0] = 1;
                }
                if (v.x > k.Key.x)
                {
                    // Connects right
                    roomConnections[1] = 1;
                }
                if (v.z < k.Key.z)
                {
                    // Connects back
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
            else if (prev.y < k.Key.y && prev != k.Key)
            {
                // Moving up
                roomString += " Up";
                if (prev.z < k.Key.z)
                {
                    // Moving forward
                    roomString += "Forward";
                }
                else if (prev.z > k.Key.z)
                {
                    // Moving back
                    roomString += "Back";
                }
                else if (prev.x < k.Key.x)
                {
                    // Moving to right
                    roomString += "Right";
                }
                else if (prev.x > k.Key.x)
                {
                    // Moving to left
                    roomString += "Left";
                }
            }
            else if (prev.y > k.Key.y && prev != k.Key)
            {
                // Moving down
                roomString += " Down";
                if (prev.z < k.Key.z)
                {
                    // Moving forward
                    roomString += "Forward";
                }
                else if (prev.z > k.Key.z)
                {
                    // Moving back
                    roomString += "Back";
                }
                else if (prev.x < k.Key.x)
                {
                    // Moving to right
                    roomString += "Right";
                }
                else if (prev.x > k.Key.x)
                {
                    // Moving to left
                    roomString += "Left";
                }
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
