
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MeshEditor : MonoBehaviour
{
    public class MeshVertex
    {
        public Vector3 vertexPosition;
        public Transform vertexGrip;
        public Dictionary<Vector3, MeshVertex> edgeMap;    //Store a list of neihbors, vertex's connected to this one by edge
        List<int> sourceVertexIDs; //store a list of index values that this vertex references, may be like 6 different vertex's all combined into one here.

        public MeshVertex()
        {
            vertexPosition = Vector3.zero;
            edgeMap = new Dictionary<Vector3, MeshVertex>();
            sourceVertexIDs = new List<int>();
        }
        public MeshVertex(Vector3 _vertexPosition)
        {
            vertexPosition = _vertexPosition;
            edgeMap = new Dictionary<Vector3, MeshVertex>();
            sourceVertexIDs = new List<int>();
        }

        public void AddVertexEdge(MeshVertex nextNeighbor)
        {
            //add an edge if we don't already have it
            if (!edgeMap.ContainsKey(nextNeighbor.vertexPosition))
                edgeMap.Add(nextNeighbor.vertexPosition, nextNeighbor);
        }

        public void AddSourceIndex(int val)
        {
            //add a new source index to the source list

            //not sure why we would have duplicate id's here but, check anyway for duplicates
            if (!sourceVertexIDs.Contains(val))
            {
                sourceVertexIDs.Add(val);
            }
        }

        public void Update(ref Vector3[] updateVerts)
        {
            if (updateVerts != null)
            {
                for (int i = 0; i < sourceVertexIDs.Count; i++)
                {
                    updateVerts[sourceVertexIDs[i]] = vertexGrip.localPosition;
                }
            }
        }

    }
    public class MeshEdge
    {
        public MeshVertex v1;
        public MeshVertex v2;
        public MeshEdge()
        {
            v1 = null;
            v2 = null;
        }
        public MeshEdge(MeshVertex _v1, MeshVertex _v2)
        {
            v1 = _v1;
            v2 = _v2;
        }
    }

    private Dictionary<Vector3, MeshVertex> vertexDictionary;
    public bool _destroy;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector2[] uvs;
    private Vector3 vertPos;
    private GameObject[] handles;

    private Dictionary<int, MeshVertex> handleDictionary;

    private const string TAG_HANDLE = "VertHandle";

    void FillVertexMap(int[] triList)
    {
        //Debug.Log("Triangles: " + (triList.Length / 3).ToString());
        for (int i = 0; i + 2 < triList.Length; i += 3)
        {
            //Debug.Log("Filling verts count: " + i.ToString());
            //get the 3 vertices of the triangle
            Vector3 v1 = verts[triList[i]];
            Vector3 v2 = verts[triList[i + 1]];
            Vector3 v3 = verts[triList[i + 2]];
            MeshVertex mv1 = null;
            MeshVertex mv2 = null;
            MeshVertex mv3 = null;
            bool v1Set = false;
            bool v2Set = false;
            bool v3Set = false;

            if (vertexDictionary.ContainsKey(v1))
            {
                v1Set = true;
                mv1 = vertexDictionary[v1];
            }
            if (vertexDictionary.ContainsKey(v2))
            {
                v2Set = true;
                mv2 = vertexDictionary[v2];
            }
            if (vertexDictionary.ContainsKey(v3))
            {
                v3Set = true;
                mv3 = vertexDictionary[v3];
            }


            if (!v1Set)
            {
                //didn't find v1, create a new one
                mv1 = new MeshVertex(v1);
                vertexDictionary.Add(v1, mv1);
            }
            if (!v2Set)
            {
                //didn't find v2, create a new one
                mv2 = new MeshVertex(v2);
                vertexDictionary.Add(v2, mv2);
            }
            if (!v3Set)
            {
                //didn't find v3, create a new one
                mv3 = new MeshVertex(v3);
                vertexDictionary.Add(v3, mv3);
            }

            //Now that we have vertices created, add each other as neighbors
            mv1.AddVertexEdge(mv2);
            mv1.AddVertexEdge(mv3);
            mv2.AddVertexEdge(mv1);
            mv2.AddVertexEdge(mv3);
            mv3.AddVertexEdge(mv1);
            mv3.AddVertexEdge(mv2);


        }
    }

    void LinkVerticesToMap()
    {

        //add all vertex indexs to the source list of a specific vertex. So if there are multiple vertices on top of each other, we only will control 1
        for (int i = 0; i < verts.Length; i++)
        {
            if (!vertexDictionary.ContainsKey(verts[i]))
            {
                Debug.Log("Vertex index (" + i.ToString() + " - " + verts[i].ToString() + ") is not in map, could this be a rounding problem?");
            }
            else
            {
                vertexDictionary[verts[i]].AddSourceIndex(i);
            }
        }
    }

    void DisplayUVs()
    {

    }

    void OnEnable()
    {
        if (MeshManager.instance == null)
        {
            Debug.Log("MeshManager is null. This may be due to deleting a gameobject with both a manager and editor attached.");
            return;
        }
        if (MeshManager.instance.modifyMesh == MeshManager.CopyMesh.ORIGINAL)
            mesh = GetComponent<MeshFilter>().sharedMesh;
        else if (MeshManager.instance.modifyMesh == MeshManager.CopyMesh.COPY)
        {
            Mesh sourceMesh = GetComponent<MeshFilter>().sharedMesh;

            string filePath =
            EditorUtility.SaveFilePanelInProject("Create New Mesh", sourceMesh.name + MeshManager.instance.postfixName, "asset", "");
            if (filePath == "")
            {
                Debug.Log("unable to create file path for new mesh. Data will probably not be saved.");
                mesh = Instantiate<Mesh>(sourceMesh);
            }
            else
            {
                mesh = Instantiate<Mesh>(sourceMesh);
                AssetDatabase.CreateAsset(mesh, filePath);
                mesh = AssetDatabase.LoadAssetAtPath<Mesh>(filePath);
            }

            MeshFilter myFilter = GetComponent<MeshFilter>();
            MeshCollider myCollider = GetComponent<MeshCollider>();
            if (myFilter != null)
                myFilter.mesh = mesh;
            if (myCollider != null)
                myCollider.sharedMesh = mesh;
        }
        //Debug.Log("name of mesh: " + mesh.name.ToString());
        vertexDictionary = new Dictionary<Vector3, MeshVertex>();
        handleDictionary = new Dictionary<int, MeshVertex>();

        verts = mesh.vertices;
        uvs = mesh.uv;

        FillVertexMap(mesh.GetTriangles(0)); //Should we handle multiple sub meshes? 

        LinkVerticesToMap(); //assign vertex indexes to the map 

        DisplayUVs();
        mesh.uv = uvs;
        // Debug.Log("Total combined verts: " + vertexDictionary.Count.ToString());


        foreach (MeshVertex vert in vertexDictionary.Values)
        {
            vertPos = transform.TransformPoint(vert.vertexPosition);
            GameObject handle = new GameObject(TAG_HANDLE);
            vert.vertexGrip = handle.transform;
            handleDictionary.Add(handle.GetInstanceID(), vert);
            handle.transform.position = vertPos;
            handle.transform.parent = transform;
            handle.tag = TAG_HANDLE;
            handle.AddComponent<VertHandleGizmo>()._parent = this;
        }

    }

    void OnDisable()
    {
        GameObject[] handles = GameObject.FindGameObjectsWithTag(TAG_HANDLE);
        foreach (GameObject handle in handles)
        {
            DestroyImmediate(handle);
        }
    }

    void Update()
    {
        if (_destroy)
        {
            _destroy = false;
            DestroyImmediate(this);
            return;
        }

        foreach (MeshVertex mv in vertexDictionary.Values)
        {
            mv.Update(ref verts);
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }

    void IncreaseDepth()
    {

        List<int> mySelectedChildren = new List<int>();

        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            Transform selectedTransform = Selection.transforms[i];
            if (selectedTransform.tag == "VertHandle")
            {
                if (selectedTransform.parent == transform)
                {
                    //if this vertex is a child of ours, then store it for later
                    mySelectedChildren.Add(selectedTransform.gameObject.GetInstanceID());
                }
            }
        }
        int nextID = 0;
        List<int> addSelection = new List<int>();
        for (int i = 0; i < mySelectedChildren.Count; i++)
        {
            nextID = mySelectedChildren[i];
            if (handleDictionary.ContainsKey(nextID))
            {
                foreach (MeshVertex nextVertex in handleDictionary[nextID].edgeMap.Values)
                {
                    //add each edge neighbor to selected list
                    if (!addSelection.Contains(nextVertex.vertexGrip.gameObject.GetInstanceID()))
                        addSelection.Add(nextVertex.vertexGrip.gameObject.GetInstanceID());
                }
            }
            if (addSelection.Contains(nextID))
                addSelection.Remove(nextID);
        }

        //have all our additional ids to select in addSelection, now select them
        int[] combinedLists = new int[addSelection.Count + Selection.instanceIDs.Length];
        Selection.instanceIDs.CopyTo(combinedLists, 0);
        addSelection.CopyTo(combinedLists, Selection.instanceIDs.Length);

        Selection.instanceIDs = combinedLists;
    }

    void DecreaseDepth()
    {
        List<int> mySelectedChildren = new List<int>();

        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            Transform selectedTransform = Selection.transforms[i];
            if (selectedTransform.tag == "VertHandle")
            {
                if (selectedTransform.parent == transform)
                {
                    //if this vertex is a child of ours, then store it for later
                    mySelectedChildren.Add(selectedTransform.gameObject.GetInstanceID());
                }
            }
        }
        int nextID = 0;
        List<int> deleteSelection = new List<int>();
        for (int i = 0; i < mySelectedChildren.Count; i++)
        {
            nextID = mySelectedChildren[i];
            if (handleDictionary.ContainsKey(nextID))
            {
                bool isBoundary = false;
                foreach (MeshVertex nextVertex in handleDictionary[nextID].edgeMap.Values)
                {
                    //this is an edge vertex if only some of the neighbors are selected
                    if (!Selection.Contains(nextVertex.vertexGrip.gameObject.GetInstanceID()))
                    {
                        isBoundary = true;
                        break;
                    }

                }
                if (isBoundary)
                {
                    if (!deleteSelection.Contains(nextID))
                        deleteSelection.Add(nextID);
                }
            }
        }
        Transform[] currentSelection = Selection.transforms;
        List<int> modifiedSelection = new List<int>();
        for (int i = 0; i < currentSelection.Length; i++)
        {
            //Loop through the selection list, and grab only the one not in the deleteSelectionList
            if (!deleteSelection.Contains(currentSelection[i].gameObject.GetInstanceID()))
            {
                modifiedSelection.Add(currentSelection[i].gameObject.GetInstanceID());
            }
        }
        Selection.instanceIDs = modifiedSelection.ToArray();

    }

    [MenuItem("Custom/Increase Selection Depth _=")]
    public static void IncreaseMenuCommand()
    {
        List<Transform> parentTransforms = new List<Transform>();
        foreach (Transform selectedTransform in Selection.transforms)
        {
            if (selectedTransform.tag == "VertHandle")
            {
                if (!parentTransforms.Contains(selectedTransform.parent))
                    parentTransforms.Add(selectedTransform.parent);

            }
        }
        foreach (Transform nextTransform in parentTransforms)
        {
            MeshEditor parentMeshEditor = nextTransform.GetComponent<MeshEditor>();
            if (parentMeshEditor != null)
            {
                parentMeshEditor.IncreaseDepth();
            }
        }
    }

    [MenuItem("Custom/Decrease Selection Depth _-")]
    public static void DecreaseDepthMenuCommand()
    {
        List<Transform> parentTransforms = new List<Transform>();
        foreach (Transform selectedTransform in Selection.transforms)
        {
            if (selectedTransform.tag == "VertHandle")
            {
                if (!parentTransforms.Contains(selectedTransform.parent))
                    parentTransforms.Add(selectedTransform.parent);
            }
        }

        foreach (Transform nextTransform in parentTransforms)
        {
            MeshEditor parentMeshEditor = nextTransform.GetComponent<MeshEditor>();
            if (parentMeshEditor != null)
            {
                parentMeshEditor.DecreaseDepth();
            }
        }
    }
}

[ExecuteInEditMode]
public class VertHandleGizmo : MonoBehaviour
{
    public MeshEditor _parent;
    public bool _destroy;

    void Start()
    {

    }

    void Awake()
    {

    }

    void Update()
    {
        if (_destroy)
            DestroyImmediate(_parent);
    }

    void OnDrawGizmos()
    {
        if (MeshManager.instance == null)
            return;
        if (Selection.Contains(gameObject))
            Gizmos.color = MeshManager.instance.selectedColor;
        else
            Gizmos.color = MeshManager.instance.defaultColor;

        if (MeshManager.instance.vertexDisplay == MeshManager.GripMesh.CUBE)
            Gizmos.DrawCube(transform.position, Vector3.one * MeshManager.instance.currentSize);
        else if (MeshManager.instance.vertexDisplay == MeshManager.GripMesh.SPHERE)
        {
            Gizmos.DrawSphere(transform.position, MeshManager.instance.currentSize);
        }
    }

}


