using UnityEditor;
using UnityEngine;

public class Raycast : EditorWindow
{
    static bool active;

    private static GameObject plane = null;

    private static GameObject node = null;

    private static Transform nodeParent = null;

    private static Node currentNode = null;


    // Open this from Window menu
    [MenuItem("Window/Raycast in editor test")]
    static void Init()
    {
        var window = (Raycast)EditorWindow.GetWindow(typeof(Raycast));
        window.Show();

        plane = GameObject.Find("DEBUGPLANE");
        plane.SetActive(false);
    }

    // Listen to scene event
    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    // Receives scene events
    // Use event mouse click for raycasting
    void OnSceneGUI(SceneView view)
    {
        if(plane == null)
        {
            plane = FindInActiveObjectByName("DEBUGPLANE");
            return;
        }

        if (!active)
        {
            if(plane.activeSelf)
                plane.SetActive(false);
            return;
        }

        if (!plane.activeSelf)
            plane.SetActive(true);

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            // Spawn cube on hit location
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);

                GameObject obj = Instantiate(node);
                Vector3 pos = hit.point;
                pos.z = 0;
                obj.transform.position = pos;

                if(nodeParent == null)
                {
                    nodeParent = GameObject.Find("Points").transform;
                }

                obj.transform.parent = nodeParent;
                obj.name = "Point(" + (nodeParent.childCount - 1) + ")";

                Node newNode = obj.GetComponent<Node>();
                if(currentNode != null)
                {
                    newNode.links.Add(currentNode);
                    currentNode.links.Add(newNode);
                }
                currentNode = newNode;
            }
        }

        //Event.current.Use();
    }

    // Creates a editor window with button 
    // to toggle raycasting on/off
    void OnGUI()
    {
        if (GUILayout.Button("Enable Raycasting"))
        {
            active = !active;
        }

        GUILayout.Label("Active:" + active);

        GUILayout.Label("To Add Node", EditorStyles.boldLabel);
        node = (GameObject)EditorGUILayout.ObjectField(node, typeof(GameObject), true);

        GUILayout.Label("Parent Node", EditorStyles.boldLabel);
        nodeParent = (Transform)EditorGUILayout.ObjectField(nodeParent, typeof(Transform), true);

        GUILayout.Label("Selected Node", EditorStyles.boldLabel);
        currentNode = (Node)EditorGUILayout.ObjectField(currentNode, typeof(Node), true);

        if (GUILayout.Button("Show Links"))
        {
            GameObject goDebug = FindInActiveObjectByName("DEBUG_LINKS");
            if(goDebug != null)
            {
                DestroyImmediate(goDebug);
            }

            GameObject go = new GameObject("DEBUG_LINKS");
            Node[] points = FindObjectsOfType<Node>();
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = 0; j < points[i].links.Count; j++)
                {
                    GameObject link = new GameObject("link");
                    LineRenderer lr = link.AddComponent<LineRenderer>();
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                    lr.positionCount = 2;
                    lr.SetPosition(0, points[i].transform.position);
                    lr.SetPosition(1, points[i].links[j].transform.position);
                    link.transform.parent = go.transform;
                }
            }
        }

        if (GUILayout.Button("Remove Links"))
        {
            GameObject goDebug = FindInActiveObjectByName("DEBUG_LINKS");
            if (goDebug != null)
            {
                DestroyImmediate(goDebug);
            }
        }

        if (GUILayout.Button("Refresh"))
        {
            plane = FindInActiveObjectByName("DEBUGPLANE");
            plane.SetActive(false);

            node = Resources.Load<GameObject>("Point");

            if (nodeParent == null)
            {
                nodeParent = GameObject.Find("Points").transform;
            }
        }
    }

    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

}