using UnityEngine;
using System.Collections;


public class PlanetVertexFinder : MonoBehaviour
{
    [SerializeField] int m_framesToSpreadOver = 6;
    [SerializeField] MeshFilter[] m_meshFilters;

    private Transform m_playerTransform;
    private Mesh[] m_meshes;
    private Transform[] m_meshTransforms;
    private Vector3[] m_meshPositions;
    private Vector3[][] m_verticesSet;
    private Vector3[] m_vertices;
    private Vector3 m_closestPoint;

    private Transform m_closestMeshTransform;

    private int m_length;
    private int m_vertexSetIndex;
    private int m_closestVertexIndex;

    private bool m_iterating;
    

    void Awake()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag(Tags.Player).transform;

        if (m_meshFilters.Length == 0)
            m_meshFilters = GetComponentsInChildren<MeshFilter>();

        m_length = m_meshFilters.Length;
        m_meshes = new Mesh[m_length];
        m_meshTransforms = new Transform[m_length];
        m_meshPositions = new Vector3[m_length];
        m_verticesSet = new Vector3[m_length][];

        for (int i = 0; i < m_length; i++)
        {
            var meshFilter = m_meshFilters[i];
            m_meshes[i] = meshFilter.mesh;
            m_meshTransforms[i] = meshFilter.transform;
            m_meshPositions[i] = meshFilter.GetComponent<MeshRenderer>().bounds.center;
            m_verticesSet[i] = m_meshes[i].vertices;
        }

        //print(name + " mesh has " + m_vertices.Length + " vertices");
    }


    void Update()
    {
        if (!m_iterating)
        {
            FindClosestMeshTransform();
            StartCoroutine(IterateToClosestVertex());
        }
    }


    private void FindClosestMeshTransform()
    {
        float minDistanceSqr = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < m_length; i++)
        {
            float distSq = (m_playerTransform.position - m_meshPositions[i]).sqrMagnitude;

            if (distSq < minDistanceSqr)
            {
                closestIndex = i;
                minDistanceSqr = distSq;
            }
        }

        m_vertexSetIndex = closestIndex;
        m_closestMeshTransform = m_meshTransforms[m_vertexSetIndex];
        m_vertices = m_verticesSet[m_vertexSetIndex];
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
            var point = m_closestMeshTransform.InverseTransformPoint(m_playerTransform.position);

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
        closestVertex = m_closestMeshTransform.TransformPoint(closestVertex);

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
