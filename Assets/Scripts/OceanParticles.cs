using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanParticles : MonoBehaviour
{
    // Ocean Settings
    [SerializeField]
    Transform PointPrefab;

    [SerializeField, Range(10, 500)]
    int resolution = 10;

    Transform[] points;

    // Quad Settings
    [SerializeField]
    private Material material;

    private Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    float[] graph;

    void Awake() {
        float step = 24f / resolution;
        var scale = Vector3.one * step;
        var pos = Vector3.zero;

        points = new Transform[resolution];

        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i] = Instantiate(PointPrefab);
            point.SetParent(transform);
            pos.x = (i + 0.5f) * step - 12f;
            pos.y = RunOceanFunction(pos.x);
            point.localScale = scale;
            point.localPosition = pos;
        }
    }

    void Update() {
        float step = 24f / resolution;
        graph = new float[points.Length];
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i];
            Vector3 pos = point.localPosition;
            point.localPosition = pos;
            graph[i] = RunOceanFunction(pos.x);
            pos.y = graph[i];
            pos.z = -5;
            point.localPosition = pos;
        }

        mesh.Clear();
        CreateOceanMesh(graph);
        DisplayOceanMesh();
    }

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void FixedUpdate() {
        float step = 24f / resolution;
        float centerpointHeight = points[points.Length / 2].localPosition.y;
        GetComponent<BuoyancyEffector2D>().surfaceLevel = (centerpointHeight / 1.2f) - 0.2f;
        //GetComponent<BoxCollider2D>().offset = new Vector2(GetComponent<BoxCollider2D>().offset.x, (centerpointHeight / 1.2f) - 4f);

        // Update Ocean to Collider
        Vector2[] NewPoints = new Vector2[points.Length + 2];
        NewPoints[0] = new Vector2(-12.0f, -5.5f);
        int i;
        for (i = 0; i < points.Length; i++) {
            NewPoints[i + 1] = new Vector2((i + 0.5f) * step - 12f, graph[i]);
        }
        NewPoints[i + 1] = new Vector2(12.0f, -5.5f);
        GetComponent<PolygonCollider2D>().points = NewPoints;
    }

    float RunOceanFunction(float x) {
        float sine1 = Mathf.Sin(x + Time.time) / 2f;
        float sine2 = Mathf.Sin((x + Time.time)*1.5f);
        float sine3 = Mathf.Sin((x + Time.time)*0.2f) / 2f;
        float y = (sine1+sine2+sine3) / 2.5f;
        return y;
    }

    void CreateOceanMesh(float[] graph) {
        vertices = new Vector3[graph.Length * 2];

        int graph_index = 0;
        int vertex_index = 0;
        Vector3 pointpos;
        while (graph_index < graph.Length) {
            pointpos = points[graph_index].localPosition;
            vertices[vertex_index] = new Vector3(pointpos.x, -5, 0);
            vertex_index++;
            vertices[vertex_index] = new Vector3(pointpos.x, graph[graph_index], 0);
            vertex_index++;

            graph_index++;
            graph_index++;
        }

        // Get Triangle Numbers
        triangles = new int[(points.Length-2)*3];

        int triangle_index = 0;
        for (int i = 0; i < (points.Length-3); i++) {
            triangles[triangle_index] = i;
            triangle_index++;
            triangles[triangle_index] = i+1;
            triangle_index++;
            triangles[triangle_index] = i+2;
            triangle_index++;
        }
    }

    void DisplayOceanMesh() {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
