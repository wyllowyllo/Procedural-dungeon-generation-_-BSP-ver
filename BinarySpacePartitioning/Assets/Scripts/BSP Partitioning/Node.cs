using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Node
{
    List<Node> childrenNodeList;

    public int Visited { get; set; }
    public List<Node> ChildrenNodeList { get => childrenNodeList; set => childrenNodeList = value; }
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }

    public Node Parent { get; set; }

    public int TreeLayerIndex {  get; set; }
    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    private void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }
}