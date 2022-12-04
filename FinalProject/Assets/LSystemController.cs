using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class LSystemController : MonoBehaviour
{
    // for defining the language and rules
    Hashtable ruleHash = new Hashtable(100);

    float initial_length = 6.0f;
    public float initial_radius = 1.0f;
    List<byte> start;
    List<byte> lang;
    GameObject contents;
    float angleToUse = 90f;
    int iterations = 10;

    // current location and angle
    Vector3 position = new Vector3(0, 0, 0);
    float angle = 0f;

    // to push and pop location and angles
    Stack<float> positions = new Stack<float>(100);
    Stack<float> angles = new Stack<float>(100);

    // for drawing lines
    public float lineWidth = 1.0f;
    Mesh lineMesh;
    struct vertexInfo
    {
        public Vector3 pos;
        public Color32 color;
        public vertexInfo(Vector3 p, Color32 c)
        {
            pos = p;
            color = c;
        }
    }
    List<vertexInfo> vertices;
    List<int> indices;
    public Material lineMaterial;
    MeshFilter filter;

    RoomManager roomManager;

    // A dictionary of all individual vertices and their connections
    Dictionary<Vector3, List<Vector3>> connections = new Dictionary<Vector3, List<Vector3>>();
    Vector3 end;

    void Start()
    {
        // DEBUG
        //Random.InitState(42);
        //

        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();

        // for timing start/finish of the rule generation and display
        // can be commented out
        Stopwatch watch = new Stopwatch();

        // create the object to draw with some default values for the mesh and rendering
        contents = GameObject.CreatePrimitive(PrimitiveType.Cube);
        contents.transform.position = new Vector3(0, 0f, 0);
        filter = (MeshFilter)contents.GetComponent("MeshFilter");
        MeshRenderer renderer = (MeshRenderer)filter.GetComponent<MeshRenderer>();
        renderer.material = lineMaterial;
        lineMesh = new Mesh();
        filter.mesh = lineMesh;

        watch.Start();
        // we set the start with the expected max size of the language iteration
        start = new List<byte>(100);

        // Axiom  : 809
        start.Add(8);
        start.Add(0);
        start.Add(9);
        byte[] rule;

        // All characters are translated into numbers for the byte array:
        // X=0 , F=1 , +=2 , -=3 , [=4 , ]=5, S=8, E=9

        List<byte[]> rules0 = new List<byte[]>();
        // X -> F
        rule = new byte[] { 1 };
        rules0.Add(rule);
        // X -> FF
        rule = new byte[] { 1, 1 };
        rules0.Add(rule);
        // X -> F-F
        rule = new byte[] { 1, 3, 1 };
        rules0.Add(rule);
        // X -> F+F
        rule = new byte[] { 1, 2, 1 };
        rules0.Add(rule);

        ruleHash.Add((byte)0, rules0);

        List<byte[]> rules1 = new List<byte[]>();
        // F -> +F
        rule = new byte[] { 2, 1 };
        rules1.Add(rule);
        // F -> -F
        rule = new byte[] { 3, 1 };
        rules1.Add(rule);
        // F -> FX
        rule = new byte[] { 1, 0 };
        rules1.Add(rule);
        // F -> [+F]X
        rule = new byte[] { 4, 2, 1, 5, 0 };
        rules1.Add(rule);
        // F -> [-F]X
        rule = new byte[] { 4, 3, 1, 5, 0 };
        rules1.Add(rule);
        // F -> [+F]F
        rule = new byte[] { 4, 2, 1, 5, 1 };
        rules1.Add(rule);
        // F -> [-F]F
        rule = new byte[] { 4, 3, 1, 5, 1 };
        rules1.Add(rule);

        ruleHash.Add((byte)1, rules1);

        List<byte[]> rules8 = new List<byte[]>();
        // S -> +S
        rule = new byte[] { 2, 8 };
        rules8.Add(rule);
        // S -> ++S
        rule = new byte[] { 2, 2, 8 };
        rules8.Add(rule);
        // S -> +++S
        rule = new byte[] { 2, 2, 2, 8 };
        rules8.Add(rule);
        // S -> S
        // This rule should happen most of the time
        rule = new byte[] { 8 };
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);
        rules8.Add(rule);

        ruleHash.Add((byte)8, rules8);

        List<byte[]> rules9 = new List<byte[]>();
        // E -> +E
        rule = new byte[] { 2, 9 };
        rules9.Add(rule);
        // E -> ++E
        rule = new byte[] { 2, 2, 9 };
        rules9.Add(rule);
        // E -> +++E
        rule = new byte[] { 2, 2, 2, 9 };
        rules9.Add(rule);
        // E -> E
        // This rule should happen most of the time
        rule = new byte[] { 9 };
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);
        rules9.Add(rule);

        ruleHash.Add((byte)9, rules9);

        vertices = new List<vertexInfo>();
        indices = new List<int>();
        run(iterations);

        // now print out the time for gen
        watch.Stop();

        // DEBUG
        //UnityEngine.Debug.Log("Time for generation took: " + watch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Size of lang is: " + lang.Count);
        //

        watch.Reset();

        // print out the time for display
        watch.Start();

        display4();

        watch.Stop();

        // DEBUG
        //UnityEngine.Debug.Log("Time for display took: " + watch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Count of vertices in list: " + vertices.Count);
        //

        watch.Start();

        BuildConnections();
        roomManager.BuildDungeon(connections, end);

        watch.Stop();

        // DEBUG
        //UnityEngine.Debug.Log("Time for dungeon took: " + watch.ElapsedMilliseconds);
        //
    }

    // Get a rule from a given letter that's in our array
    byte[] getRule(byte[] input)
    {
        byte[] chosenRule = new byte[input.Length];
        input.CopyTo(chosenRule, 0);

        if (ruleHash.ContainsKey(input[0]))
        {
            List<byte[]> rules = ruleHash[input[0]] as List<byte[]>;

            // Randomize rules
            Stack<int> ruleIndices = new Stack<int>();
            while (ruleIndices.Count < rules.Count)
            {
                int i = UnityEngine.Random.Range(0, rules.Count);
                if (ruleIndices.Contains(i))
                {
                    continue;
                }

                ruleIndices.Push(i);
            }

            // Keep trying rules until there are none left or a valid one is found
            bool verticesAdded;
            do
            {
                chosenRule = rules[ruleIndices.Pop()];
                List<byte> potentialRule = new List<byte>(chosenRule);
                verticesAdded = addVerticies(potentialRule);
            }
            while (ruleIndices.Count > 0 && !verticesAdded);


            if (!verticesAdded)
            {
                if (input[0] == 9)
                {
                    // This is an exit and it cannot be skipped
                    // Choose a random vertex that isn't the start and try to add the exit there
                    position = vertices[UnityEngine.Random.Range(2, vertices.Count - 1)].pos;
                    chosenRule = new byte[1] { 9 };
                    getRule(chosenRule);
                }
                else
                {
                    // A rule could not be found, replace with a terminating character
                    chosenRule = new byte[1] { 0 };
                    addVerticies(new List<byte>(chosenRule));
                }
            }
        }
        else
        {
            // There isn't a rule for this byte, see if vertices can be added
            addVerticies(new List<byte>(chosenRule));
        }

        return chosenRule;
    }

    // Run the lsystem iterations number of times on the start axiom.
    // note that this is double buffering
    void run(int iterations)
    {
        List<byte> buffer1 = start;
        List<byte> buffer2 = new List<byte>(100);
        List<byte> currentList = buffer1;
        List<byte> newList = buffer2;
        byte[] singleByte = new byte[] { 0 };
        int currentCount = 0;

        for (int i = 0; i < iterations; i++)
        {
            // Reset positions and angles
            vertices.Clear();
            indices.Clear();
            position = Vector3.zero;
            angle = 0f;
            positions.Clear();
            angles.Clear();

            currentCount = currentList.Count;
            for (int j = 0; j < currentCount; j++)
            {
                singleByte[0] = currentList[j];
                byte[] buff = getRule(singleByte);
                newList.AddRange(buff);
            }
            List<byte> tmp = currentList;
            currentList = newList;
            tmp.Clear();
            newList = tmp;
        }

        lang = currentList;

        // DEBUG
        string s = "Vertices (" + vertices.Count + "): ";
        foreach (vertexInfo v in vertices)
        {
            s += v.pos + ", ";
        }
        print(s);

        s = "Lang (" + lang.Count + "): ";
        foreach (byte b in lang)
        {
            s += b;
        }
        print(s);
        //
    }

    bool addVerticies(List<byte> str)
    {
        List<vertexInfo> potentialVertices = new List<vertexInfo>();

        Vector3 positionTemp = position;
        float angleTemp = angle;
        Stack<float> positionsTemp = new Stack<float>(positions);
        Stack<float> anglesTemp = new Stack<float>(angles);

        float posx = positionTemp.x;
        float posz = positionTemp.z;

        // location and rotation to draw towards
        Vector3 newPosition;
        Vector2 rotated;

        Color color = Color.green;

        // start at 0,0,0
        // Apply all the drawing rules to the lsystem string
        for (int i = 0; i < str.Count; i++)
        {
            byte buff = str[i];
            switch (buff)
            {
                case 0:
                    break;
                case 8:
                case 9:
                case 1:
                    if (buff == 8) color = Color.blue;
                    if (buff == 9) color = Color.red;
                    if (buff == 1) color = Color.green;

                    // draw a line 
                    posz += initial_length;
                    rotated = rotate(positionTemp, new Vector3(positionTemp.x, 0, posz), angleTemp);
                    newPosition = new Vector3(rotated.x, 0, rotated.y);

                    potentialVertices.AddRange(addLineToMesh(lineMesh, positionTemp, newPosition, color));

                    // set up for the next draw
                    positionTemp = newPosition;
                    posx = newPosition.x;
                    posz = newPosition.z;

                    break;
                case 2:
                    // Turn left 90
                    angleTemp += angleToUse;

                    break;
                case 3:
                    // Turn right 90
                    angleTemp -= angleToUse;

                    break;
                case 4:
                    //[: push position and angle
                    positionsTemp.Push(posz);
                    positionsTemp.Push(posx);
                    float currentAngle = angleTemp;
                    anglesTemp.Push(currentAngle);

                    break;
                case 5:
                    //]: pop position and angle
                    posx = positionsTemp.Pop();
                    posz = positionsTemp.Pop();
                    positionTemp = new Vector3(posx, 0, posz);
                    angleTemp = anglesTemp.Pop();

                    break;
                default: break;
            }
        }

        if (checkVertices(potentialVertices))
        {
            // Apply everything
            position = positionTemp;
            angle = angleTemp;
            positions = new Stack<float>(positionsTemp);
            angles = new Stack<float>(anglesTemp);

            for (int i = 0; i < potentialVertices.Count; i += 2)
            {
                // The vertices are valid, add them
                int numberOfPoints = vertices.Count;
                int[] indicesForLines = new int[] { 0 + numberOfPoints, 1 + numberOfPoints };

                vertices.Add(potentialVertices[i]);
                vertices.Add(potentialVertices[i + 1]);
                indices.AddRange(indicesForLines);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    bool checkVertices(List<vertexInfo> verts)
    {
        // Don't check the first vertex since it will always be the last in vertices
        for (int i = 1; i < verts.Count; i++)
        {
            for (int j = 0; j < vertices.Count; j++)
            {
                if (vertices[j].pos == verts[i].pos)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void display4()
    {
        //vertices.TrimExcess();
        //indices.TrimExcess();

        // after we recreate the mesh we need to assign it to the original object
        MeshUpdateFlags flags = MeshUpdateFlags.DontNotifyMeshUsers & MeshUpdateFlags.DontRecalculateBounds
            & MeshUpdateFlags.DontResetBoneBounds & MeshUpdateFlags.DontValidateIndices;

        // set vertices
        int totalCount = vertices.Count;
        var layout = new[]
        {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4)
        };
        lineMesh.SetVertexBufferParams(totalCount, layout);
        lineMesh.SetVertexBufferData(vertices, 0, 0, totalCount, 0, flags);

        // set indices
        totalCount = indices.Count;
        UnityEngine.Rendering.IndexFormat format = IndexFormat.UInt32;
        lineMesh.SetIndexBufferParams(totalCount, format);
        lineMesh.SetIndexBufferData(indices, 0, 0, totalCount, flags);

        // set submesh
        SubMeshDescriptor desc = new SubMeshDescriptor(0, totalCount, MeshTopology.Lines);
        desc.bounds = new Bounds();
        desc.baseVertex = 0;
        desc.firstVertex = 0;
        desc.vertexCount = totalCount;
        lineMesh.SetSubMesh(0, desc, flags);
    }

    vertexInfo[] addLineToMesh(Mesh mesh, Vector3 from, Vector3 to, Color color)
    {
        vertexInfo[] lineVerts = new vertexInfo[] { new vertexInfo(from, color), new vertexInfo(to, color) };
        //int numberOfPoints = vertices.Count;
        //int[] indicesForLines = new int[] { 0 + numberOfPoints, 1 + numberOfPoints };

        //vertices.AddRange(lineVerts);
        //indices.AddRange(indicesForLines);

        return lineVerts;
    }

    // rotate a line and return the position after rotation
    // Assumes rotation around the Z axis
    Vector2 rotate(Vector3 pivotPoint, Vector3 pointToRotate, float angle)
    {
        Vector2 result;
        float Nx = (pointToRotate.x - pivotPoint.x);
        float Nz = (pointToRotate.z - pivotPoint.z);
        angle = -angle * Mathf.PI / 180f;
        result = new Vector2(Mathf.Cos(angle) * Nx - Mathf.Sin(angle) * Nz + pivotPoint.x, Mathf.Sin(angle) * Nx + Mathf.Cos(angle) * Nz + pivotPoint.z);
        
        result.x = Mathf.Round(result.x);
        result.y = Mathf.Round(result.y);
        return result;
    }

    void BuildConnections()
    {
        connections.Add(vertices[0].pos, new List<Vector3>());

        for (int i = 1; i < vertices.Count; i += 2)
        {
            connections.Add(vertices[i].pos, new List<Vector3>());
            connections[vertices[i].pos].Add(vertices[i - 1].pos);
            connections[vertices[i - 1].pos].Add(vertices[i].pos);
        }

        end = vertices[vertices.Count - 1].pos;

        // DEBUG
        string s = "";
        foreach (KeyValuePair<Vector3, List<Vector3>> k in connections)
        {
            s += k.Key + ": ";
            foreach (Vector3 v in k.Value)
            {
                s += v + ", ";
            }
            s += "\n";
        }
        print(s);
        //
    }
}