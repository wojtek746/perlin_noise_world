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

    public void Start()
    {
        ProcessStartInfo start = new ProcessStartInfo();

        start.FileName = "python.exe";
        start.Arguments = "\"" + Application.dataPath + "/python/perlin_noise.py\"" + $" \"{octaves} {seed}\"";

        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.CreateNoWindow = true;

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
                    try
                    {
                        points[i, j] = new Vector3(i, array[i, j], j);
                        newPoint.transform.position = points[i, j];
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        points[i, j] = new Vector3(i + 0.5f, array[i, j], j);
                        newPoint.transform.position = points[i, j];
                    }
                    catch
                    {

                    }
                }
            }
        }
        UnityEngine.Debug.Log(points.GetLength(0));

        for (int i = 0; i < points.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < points.GetLength(1) - 1; j++)
            {
                GameObject newTriangle = Instantiate(triangle, world.transform);
                MeshFilter meshFilter = newTriangle.GetComponent<MeshFilter>();

                Mesh mesh = meshFilter.mesh;

                Vector3[] verticles = mesh.vertices;
                verticles[0] = points[i + 1, j];
                verticles[1] = points[i, j];
                verticles[2] = points[i, j + 1];
                mesh.vertices = verticles;

                mesh.RecalculateNormals(); 
            }
        }
    }
}
