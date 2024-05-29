using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class generateWorld : MonoBehaviour
{
    public float octaves;
    public int seed;

    public GameObject point;
    public GameObject triangle;

    private ProcessStartInfo start; 

    public void Start()
    {
        start = new ProcessStartInfo();

        start.FileName = "python.exe";

        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;

        for (int i = -3; i <= 3; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                PrintChunk(i, j); 
            }
        }
    }

    public void PrintChunk(int x, int y)
    {
        start.Arguments = "\"" + Application.dataPath + "/python/perlin_noise.py\"" + $" \"{octaves} {seed} {x * 16} {y * 16} {x * 16 + 16} {y * 16 + 16}\"";

        float[,] array = null;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string output = reader.ReadToEnd().ToString();
                output = output.Substring(0, output.Length - 1);

                string[] rows = output.Split("\n");

                array = new float[rows.Length, rows[0].Split(",").Length];

                for (int i = 0; i < rows.Length; i++)
                {
                    string row = rows[i].Replace("[", "").Replace("]", "").Replace(" ", "");

                    string[] values = row.Split(",");
                    for (int j = 0; j < values.Length; j++)
                    {
                        array[i, j] = float.Parse(values[j].Split(".")[0]) + float.Parse(values[j].Split(".")[1]) / (values[j].Split(".")[1].Length * 10);
                    }
                }
            }
        }

        GameObject world = GameObject.Find("world");

        Vector3[,] points = new Vector3[array.GetLength(0), array.GetLength(1)];

        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                GameObject newPoint = Instantiate(point, world.transform.position, world.transform.rotation);
                newPoint.transform.parent = world.transform;
                if (j % 2 == 0)
                {
                    points[i, j] = new Vector3(i + x * 16, array[i, j], j + y * 16);
                    newPoint.transform.position = points[i, j];
                    newPoint.name = $"point {i + x * 16} {j + y * 16}";
                    Point script = newPoint.GetComponent<Point>();
                    if (script != null)
                    {
                        script.height = points[i, j].y;
                    }
                }
                else
                {
                    points[i, j] = new Vector3(i + 0.5f + x * 16, array[i, j], j + y * 16);
                    newPoint.transform.position = points[i, j];
                    newPoint.name = $"point {i + x * 16} {j + y * 16}";
                    Point script = newPoint.GetComponent<Point>();
                    if (script != null)
                    {
                        script.height = points[i, j].y;
                    }
                }
            }
        }
        UnityEngine.Debug.Log(points.GetLength(0));

        for (int i = 0; i < points.GetLength(0); i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                if (i < points.GetLength(0) - 1 && j < points.GetLength(1) - 1)
                {
                    Vector3 point1 = points[i + 1, j];
                    Vector3 point2 = points[i, j];
                    Vector3 point3 = points[i, j + 1];
                    PrintTriangle(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z); 
                }
                if (i > 0 && j > 0)
                {
                    Vector3 point1 = points[i - 1, j];
                    Vector3 point2 = points[i, j];
                    Vector3 point3 = points[i, j - 1];
                    PrintTriangle(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z);
                }
            }
        }

        GameObject startPoint = GameObject.Find($"point {x * 16} {y * 16 - 1}"); 
        if (startPoint != null)
        {
            for (int i = x * 16; i < x * 16 + 16; i++)
            {
                GameObject point = GameObject.Find($"point {i} {y * 16 - 1}");
                if (point != null)
                {
                    Point script = point.GetComponent<Point>();
                    if (script != null)
                    {
                        float height = script.height;
                        Vector3 point2 = points[i - x * 16, 0];
                        Vector3 point3 = points[i - x * 16 + 1, 0];
                        PrintTriangle(i + 0.5f, height, y * 16 - 1, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z);

                        GameObject nexPoint = GameObject.Find($"point {i + 1} {y * 16 - 1}");
                        if (nexPoint != null)
                        {
                            Point nexScript = nexPoint.GetComponent<Point>();
                            if (nexScript != null)
                            {
                                float nexHeight = nexScript.height;
                                Vector3 point1 = points[i - x * 16 + 1, 0];
                                PrintTriangle(i + 0.5f, height, y * 16 - 1, point1.x, point1.y, point1.z, i + 1.5f, nexHeight, y * 16 - 1);
                            }
                        }
                    }
                }
            }
        }

        startPoint = GameObject.Find($"point {x * 16} {(y + 1) * 16}");
        if (startPoint != null)
        {
            for (int i = x * 16 + 16; i > x * 16; i--)
            {
                GameObject point = GameObject.Find($"point {i} {(y + 1) * 16}");
                if (point != null)
                {
                    Point script = point.GetComponent<Point>();
                    if (script != null)
                    {
                        float height = script.height;
                        Vector3 point2 = points[i - x * 16, 15];
                        Vector3 point3 = points[i - x * 16 - 1, 15];
                        PrintTriangle(i, height, (y + 1) * 16, point2.x, point2.y, point2.z, point3.x, point3.y, point3.z);

                        GameObject nexPoint = GameObject.Find($"point {i - 1} {(y + 1) * 16}");
                        if (nexPoint != null)
                        {
                            Point nexScript = nexPoint.GetComponent<Point>();
                            if (nexScript != null)
                            {
                                float nexHeight = nexScript.height;
                                Vector3 point1 = points[i - x * 16 - 1, 15];
                                PrintTriangle(i, height, (y + 1) * 16, point1.x, point1.y, point1.z, i - 1, nexHeight, (y + 1) * 16);
                            }
                        }
                    }
                }
            }
        }
    }

    public void PrintTriangle(float x1, float y1, float z1, float x2, float y2, float z2, float x3, float y3, float z3)
    {
        GameObject world = GameObject.Find("world");

        GameObject newTriangle = Instantiate(triangle, world.transform);

        MeshFilter meshFilter = newTriangle.GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] verticles = new Vector3[3];
        verticles[0] = new Vector3(x1, y1 + 1, z1);
        verticles[1] = new Vector3(x2, y2 + 1, z2);
        verticles[2] = new Vector3(x3, y3 + 1, z3);

        mesh.vertices = verticles;
        mesh.triangles = new int[] { 0, 1, 2 };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
}
