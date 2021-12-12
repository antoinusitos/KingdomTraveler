using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float heuristique;
    public float cost;
    public Node from;
    public List<Node> links = new List<Node>();

    public int dangerosity = 0;
}
