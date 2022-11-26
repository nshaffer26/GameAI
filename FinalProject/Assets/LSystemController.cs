using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Rendering;

public class LSystemController : MonoBehaviour
{


    // for defining the language and rules
    Hashtable ruleHash = new Hashtable(100);

    public float initial_length = 2;
    public float initial_radius = 1.0f;
    List<byte> start;
    List<byte> lang;
    GameObject contents;
    float angleToUse = 90f;
    public int iterations = 4;

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

    void Start()
    {

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

        //variables : 0, 1
        //constants: [, ]
        //axiom  : 0
        //rules  : (1 → 11), (0 → 1[0]0)
        // Second example LSystem from 
        // http://en.wikipedia.org/wiki/L-system
        // uncomment the following for the lsystem above:
        //start.Add(0);
        //lang = start;
        //// 1 -> 11
        //byte[] firstRule = new byte[] { 1, 1 };
        //ruleHash.Add((byte)1, firstRule);
        //// 0 -> 1[0]0
        //// note: we/re using bytes rather than alpha text here, so we just need [ and ] to be
        //// an easy to remember number 6 and 9 look like opposites to me and I chose them.
        //byte[] secondRule = new byte[] { 1, 6, 0, 9, 0 };
        //ruleHash.Add((byte)0, secondRule);
        //angleToUse = 45f;
        //run(iterations);
        //// now print out the time for gen
        //watch.Stop();
        //UnityEngine.Debug.Log("Time for generation took: " + watch.ElapsedMilliseconds);
        //watch.Reset();
        //watch.Start();
        //// print out the display time
        //vertices = new List<vertexInfo>(lang.Count);
        //indices = new List<int>(lang.Count);
        //display2();



        // Weed type plant example from: 
        // http://en.wikipedia.org/wiki/L-system
        // rules: X = 0, F = 1, 
        // [ = 4, ] = 5
        // + = 2, - = 3
        // Uncomment the following for a weed type lsystem:

        //start.Add(0);
        ////X -> F-[[X]+X]+F[+FX]-X
        //// keep in mind that all the letters are translated into numbers for the byte array
        //byte[] firstRule = new byte[] { 1, 3, 4, 4, 0, 5, 2, 0, 5, 2, 1, 4, 2, 1, 0, 5, 3, 0 };
        //ruleHash.Add((byte)0, firstRule);
        ////F -> FF 
        //byte[] secondRule = new byte[] { 1, 1 };
        //ruleHash.Add((byte)1, secondRule);

        //angleToUse = 25f;
        //run(iterations);
        //// now print out the time for gen
        //watch.Stop();
        //UnityEngine.Debug.Log("Time for generation took: " + watch.ElapsedMilliseconds);
        //UnityEngine.Debug.Log("Size of lang is: " + lang.Count);
        //watch.Reset();
        //// print out the time for display
        //watch.Start();
        //vertices = new List<vertexInfo>(lang.Count);
        //indices = new List<int>(lang.Count);
        //display3();

        //start.Add(8);
        start.Add(0);
        byte[] rule;

        // All characters are translated into numbers for the byte array:
        // X=0 , F=1 , +=2 , -=3 , [=4 , ]=5

        List<byte[]> rules0 = new List<byte[]>();
        // X -> F
        rule = new byte[] { 1 };
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
        // F -> [F]F
        rule = new byte[] { 4, 1, 5, 1 };
        rules1.Add(rule);
        // F -> [F]X
        rule = new byte[] { 4, 1, 5, 0 };
        rules1.Add(rule);
        // F -> [X]X
        rule = new byte[] { 4, 0, 5, 0 };
        rules1.Add(rule);
        // F -> [X]F
        rule = new byte[] { 4, 0, 5, 1 };
        rules1.Add(rule);
        //// F -> [+F]X
        //rule = new byte[] { 4, 2, 1, 5, 0 };
        //rules1.Add(rule);
        //// F -> [+X]F
        //rule = new byte[] { 4, 2, 0, 5, 1 };
        //rules1.Add(rule);
        //// F -> [-F]X
        //rule = new byte[] { 4, 3, 1, 5, 0 };
        //rules1.Add(rule);
        //// F -> [-X]F
        //rule = new byte[] { 4, 3, 0, 5, 1 };
        //rules1.Add(rule);

        ruleHash.Add((byte)1, rules1);

        vertices = new List<vertexInfo>();
        indices = new List<int>();
        run(iterations);

        string s = "";
        foreach (byte b in lang)
        {
            s += b;
        }
        print(s);

        // now print out the time for gen
        watch.Stop();

        UnityEngine.Debug.Log("Time for generation took: " + watch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("Size of lang is: " + lang.Count);
        watch.Reset();

        // print out the time for display
        watch.Start();

        display4();

        watch.Stop();

        UnityEngine.Debug.Log("Time for display took: " + watch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("Count of vertices in list: " + vertices.Count);
    }

    // Get a rule from a given letter that's in our array
    byte[] getRule(byte[] input)
    {
        byte[] chosenRule = input;

        if (ruleHash.ContainsKey(input[0]))
        {
            List<byte[]> rules = ruleHash[input[0]] as List<byte[]>;
            chosenRule = rules[UnityEngine.Random.Range(0, rules.Count)];
            List<byte> potentialRule = new List<byte>(chosenRule);

            List<vertexInfo> verticesTemp = vertices;
            List<int> indicesTemp = indices;
            Vector3 positionTemp = position;
            float angleTemp = angle;
            Stack<float> positionsTemp = positions;
            Stack<float> anglesTemp = angles;

            int stop = 0;
            while (!checkVertices(getVerticies(potentialRule)) && stop < 10)
            {
                chosenRule = rules[UnityEngine.Random.Range(0, rules.Count)];
                potentialRule = new List<byte>(chosenRule);

                // Reset positions and angles
                vertices = verticesTemp;
                indices = indicesTemp;
                position = positionTemp;
                angle = angleTemp;
                positions = positionsTemp;
                angles = anglesTemp;

                stop++;
            }

            List<vertexInfo> potentialVertices = getVerticies(potentialRule);
            if (!checkVertices(potentialVertices))
            {
                chosenRule = input;

                // Reset positions and angles
                vertices = verticesTemp;
                indices = indicesTemp;
                position = positionTemp;
                angle = angleTemp;
                positions = positionsTemp;
                angles = anglesTemp;

                potentialVertices = getVerticies(new List<byte>(chosenRule));
            }
            //if (potentialVertices.Count == 2)
            //{
            //    int numberOfPoints = vertices.Count;
            //    int[] indicesForLines = new int[] { 0 + numberOfPoints, 1 + numberOfPoints };

            //    vertices.AddRange(potentialVertices);
            //    indices.AddRange(indicesForLines);
            //}
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
        //lang.Add(9);
    }

    List<vertexInfo> getVerticies(List<byte> str)
    {
        List<vertexInfo> potentialVertices = new List<vertexInfo>();

        float posx = position.x;
        float posy = position.y;

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
                    posy += initial_length;
                    rotated = rotate(position, new Vector3(position.x, posy, 0), angle);
                    newPosition = new Vector3(rotated.x, rotated.y, 0);

                    potentialVertices.AddRange(addLineToMesh(lineMesh, position, newPosition, color));

                    // set up for the next draw
                    position = newPosition;
                    posx = newPosition.x;
                    posy = newPosition.y;

                    break;
                case 2:
                    // Turn left 90
                    angle += angleToUse;

                    break;
                case 3:
                    // Turn right 90
                    angle -= angleToUse;

                    break;
                case 4:
                    //[: push position and angle
                    positions.Push(posy);
                    positions.Push(posx);
                    float currentAngle = angle;
                    angles.Push(currentAngle);

                    break;
                case 5:
                    //]: pop position and angle
                    posx = positions.Pop();
                    posy = positions.Pop();
                    position = new Vector3(posx, posy, 0);
                    angle = angles.Pop();

                    break;
                default: break;
            }
        }

        return potentialVertices;
    }

    bool checkVertices(List<vertexInfo> verts)
    {
        // Don't check the first vertex since it will always be the last in vertices
        for (int i = 1; i < verts.Count - 1; i++)
        {
            if (vertices.Contains(verts[i]))
            {
                return false;
            }
        }

        return true;
    }

    void display4()
    {
        vertices.TrimExcess();
        indices.TrimExcess();

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
        int numberOfPoints = vertices.Count;
        int[] indicesForLines = new int[] { 0 + numberOfPoints, 1 + numberOfPoints };

        vertices.AddRange(lineVerts);
        indices.AddRange(indicesForLines);

        return lineVerts;
    }

    // rotate a line and return the position after rotation
    // Assumes rotation around the Z axis
    Vector2 rotate(Vector3 pivotPoint, Vector3 pointToRotate, float angle)
    {
        Vector2 result;
        float Nx = (pointToRotate.x - pivotPoint.x);
        float Ny = (pointToRotate.y - pivotPoint.y);
        angle = -angle * Mathf.PI / 180f;
        result = new Vector2(Mathf.Cos(angle) * Nx - Mathf.Sin(angle) * Ny + pivotPoint.x, Mathf.Sin(angle) * Nx + Mathf.Cos(angle) * Ny + pivotPoint.y);
        return result;
    }


    // Draw a circle with the given parameters
    // Should probably use different stuff than the default
    void addCircleToMesh(Mesh mesh, float radiusX, float radiusY, Vector3 center, Color color)
    {
        int numberOfPoints = vertices.Count;
        float x;
        float y;
        float z = 0f;
        int segments = 15;
        float angle = (360f / segments);

        for (int i = 0; i < (segments + 1); i++)
        {

            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX + center.x;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY + center.y;

            vertices.Add(new vertexInfo(new Vector3(x, y, 0), color));
            indices.Add(numberOfPoints + i);
            angle += (360f / segments);
        }
    }
}
