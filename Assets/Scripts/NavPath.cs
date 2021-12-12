using System.Collections.Generic;
using UnityEngine;

public class NavPath : MonoBehaviour
{
    public List<Node> path = new List<Node>();
    public List<Node> openList = new List<Node>();
    public List<Node> closedList = new List<Node>();
    public List<Node> allNavPoints = new List<Node>();
    private Node startPoint = null;
    private Node endPoint = null;
    public Node currentNode = null;

    private void Start()
    {
        Node[] points = FindObjectsOfType<Node>();
        for (int i = 0; i < points.Length; i++)
        {
            allNavPoints.Add(points[i]);
        }
    }

    public bool FindPathTo(Node destination)
    {

        if (destination == currentNode)
            return false;

        openList.Clear();
        closedList.Clear();
        path.Clear();

        endPoint = destination;
        startPoint = currentNode;
        return ShortestPath();
    }

    public bool ShortestPath()
    {
        openList.Add(startPoint);
        startPoint.cost = 0;
        while (currentNode != endPoint && closedList.Count < allNavPoints.Count)
        {
            currentNode = openList[0];
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            for (int i = 0; i < currentNode.links.Count; i++)
            {
                currentNode.links[i].heuristique = Vector3.Distance(currentNode.links[i].transform.position, endPoint.transform.position);
                AddInOpenList(currentNode.links[i], currentNode);
            }
        }
        
        if(currentNode == endPoint)
        {
            currentNode = endPoint;
            RetreivePath();
            GameManager.instance.ShowRenderer(path);
            return true;
        }
        else
        {
            Debug.Log("CANNOT FIND ENDPOINT");
            currentNode = startPoint;
            return false;
        }
    }

    private void RetreivePath()
    {
        Node aNode = currentNode;
        while(aNode != startPoint && aNode.from != null && aNode != null)
        {
            path.Add(aNode);
            aNode = aNode.from;
        }
        path.Add(startPoint);
        path.Reverse();
    }

    public void AddInOpenList(Node element, Node from)
    {
        if (closedList.Contains(element))
            return;

        if (openList.Contains(element))
        {
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i] == element)
                {
                    if (openList[i].cost > from.cost + 1)
                    {
                        openList[i].from = from;
                        openList[i].cost = from.cost + 1;
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            return;
        }

        for(int i = 0; i < openList.Count; i++)
        {
            if(openList[i].heuristique > element.heuristique)
            {
                openList.Insert(i, element);
                openList[i].cost = from.cost + 1;
                openList[i].from = from;
                return;
            }
        }

        element.cost = from.cost + 1;
        element.from = from;
        openList.Add(element);
    }
}
