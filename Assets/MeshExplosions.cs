using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshExplosions : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public GameObject explosion;
    public float nextExplosionTime;
    public float minimumTimeBetweenExplosions;
    public float maximumTimeBetweenExplosions;

    void Update()
    {
        if (Time.time < nextExplosionTime)
            return;

        Explode();
    }

    void Explode()
    {
        nextExplosionTime = Time.time + Random.Range(MinTime(), MaxTime());

        Vector3 randomPoint = Vector3.zero;

        randomPoint = GetRandomWorldPointOnMeshes(meshRenderers);

        int randomCount = Random.Range(0, 3) + 1;

        for (int i = 0; i < randomCount; i++)
        {
            Vector3 randomV3 = Random.insideUnitSphere * 2;
            Instantiate(explosion, randomPoint + randomV3, Quaternion.identity);
        }
    }

    float MinTime() => Mathf.Min(minimumTimeBetweenExplosions, maximumTimeBetweenExplosions);

    float MaxTime() => Mathf.Max(minimumTimeBetweenExplosions, maximumTimeBetweenExplosions);

    public static Vector3 GetRandomWorldPointOnMeshes(MeshRenderer[] meshRenderers)
    {
        // Step 1: Calculate the total surface area of all meshes
        float[] cumulativeAreas = new float[meshRenderers.Length];
        float totalArea = 0f;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshFilter meshFilter = meshRenderers[i].GetComponent<MeshFilter>();
            if (meshFilter == null)
                continue;

            float meshArea = CalculateMeshSurfaceArea(meshFilter.sharedMesh);
            totalArea += meshArea;
            cumulativeAreas[i] = totalArea;
        }

        // Step 2: Randomly select a mesh based on its proportional area
        float randomArea = Random.Range(0, totalArea);
        int selectedMeshIndex = System.Array.FindIndex(cumulativeAreas, area => randomArea <= area);
        MeshRenderer selectedMeshRenderer = meshRenderers[selectedMeshIndex];

        // Step 3: Get a random point on the selected mesh
        MeshFilter selectedMeshFilter = selectedMeshRenderer.GetComponent<MeshFilter>();
        Mesh selectedMesh = selectedMeshFilter.sharedMesh;
        Vector3 localPoint = GetRandomPointOnMesh(selectedMesh);

        // Step 4: Transform the local point to world space
        return selectedMeshRenderer.transform.TransformPoint(localPoint);
    }

    private static float CalculateMeshSurfaceArea(Mesh mesh)
    {
        float area = 0f;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            area += Vector3.Cross(v2 - v1, v3 - v1).magnitude * 0.5f;
        }

        return area;
    }

    private static Vector3 GetRandomPointOnMesh(Mesh mesh)
    {
        // Step 1: Calculate cumulative triangle areas
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float[] cumulativeAreas = new float[triangles.Length / 3];
        float totalArea = 0f;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            float triangleArea = Vector3.Cross(v2 - v1, v3 - v1).magnitude * 0.5f;
            totalArea += triangleArea;
            cumulativeAreas[i / 3] = totalArea;
        }

        // Step 2: Randomly select a triangle
        float randomArea = Random.Range(0, totalArea);
        int selectedTriangleIndex = System.Array.FindIndex(
            cumulativeAreas,
            area => randomArea <= area
        );

        // Step 3: Sample a random point within the selected triangle
        int triIndex = selectedTriangleIndex * 3;
        Vector3 a = vertices[triangles[triIndex]];
        Vector3 b = vertices[triangles[triIndex + 1]];
        Vector3 c = vertices[triangles[triIndex + 2]];

        return SamplePointInTriangle(a, b, c);
    }

    private static Vector3 SamplePointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        float u = Random.value;
        float v = Random.value;

        if (u + v > 1f)
        {
            u = 1f - u;
            v = 1f - v;
        }

        return (1 - u - v) * a + u * b + v * c;
    }
}
