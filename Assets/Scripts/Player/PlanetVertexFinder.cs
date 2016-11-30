using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshFilter))]
public class PlanetVertexFinder : MonoBehaviour
{
    [SerializeField] int m_framesToSpreadOver = 6;

    private Transform m_playerTransform;
    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Vector3 m_closestPoint;
    private int m_closestVertexIndex;

    private bool m_iterating;
    

    void Awake()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag(Tags.Player).transform;

        var meshFilter = GetComponentInChildren<MeshFilter>();
        m_mesh = meshFilter.mesh;
        m_vertices = m_mesh.vertices;

        //print(name + " mesh has " + m_vertices.Length + " vertices");
    }


    void Update()
    {
        if (!m_iterating)
            StartCoroutine(IterateToClosestVertex());
    }


    private IEnumerator IterateToClosestVertex()
    {
        m_iterating = true;
        float minDistanceSqr = Mathf.Infinity;
        var closestVertex = Vector3.zero;
        int closestVertexIndex = 0;
        int verticesPerFrame = (int) Mathf.Ceil((float) m_vertices.Length / m_framesToSpreadOver);

        int frame = 1;
        int min = 0;
        int max = verticesPerFrame;

        while (min < m_vertices.Length)
        {
            //print("Frame: " + frame + " min: " + min + ", max: " + max);

            // convert point to local space
            var point = transform.InverseTransformPoint(m_playerTransform.position);

            // scan all vertices to find nearest
            for (int i = min; i < max; i += 1)
            {
                var vertex = m_vertices[i];
                var diff = point - vertex;
                float distSqr = diff.sqrMagnitude;

                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    closestVertex = vertex;
                    closestVertexIndex = i;
                }
            }

            min = max;
            max += verticesPerFrame;
            max = Mathf.Min(max, m_vertices.Length);
            frame++;

            yield return null;
        }

        // convert nearest vertex back to world space
        closestVertex = transform.TransformPoint(closestVertex);

        m_closestPoint = closestVertex;
        m_closestVertexIndex = closestVertexIndex;  

        m_iterating = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(m_closestPoint, 1f);
    }
}
