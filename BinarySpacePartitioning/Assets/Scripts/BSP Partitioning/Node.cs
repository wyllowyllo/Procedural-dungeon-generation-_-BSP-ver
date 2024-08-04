using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Node
{

   


    // 사각형 꼭짓점 좌표
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }

    //자식노드 리스트
    List<Node> childrenNodeList;
    public List<Node> ChildrenNodeList { get => childrenNodeList; set => childrenNodeList = value; }

    //부모 노드
    public Node Parent { get; set; }

    //트리 인덱스
    public int TreeIndex {  get; set; }
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