using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    // 사각형 꼭짓점 좌표
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }

    //자식노드 리스트
    List<RoomNode> childrenNodeList;
    public List<RoomNode> ChildrenNodeList { get => childrenNodeList; set => childrenNodeList = value; }

    //부모 노드
    public RoomNode Parent { get; set; }

    //트리 인덱스
    public int TreeIndex { get; set; }

    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, RoomNode parentNode, int index)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        this.TreeIndex = index;

        childrenNodeList = new List<RoomNode>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }
    
    
    public int Width { get => (int)(TopRightAreaCorner.x-BottomLeftAreaCorner.x); }
    public int Height { get => (int)(TopRightAreaCorner.y-BottomLeftAreaCorner.y);}

    Wall leftWall;
    Wall rightWall;
    Wall bottomWall;
    Wall topWall;

    public void SetWall(Vector2Int leftBottomV, Vector2Int rightBottomV, Vector2Int rightTopV, Vector2Int leftTopV)
    {
        leftWall=new Wall(leftTopV, leftBottomV);
        rightWall = new Wall(rightTopV, rightBottomV);
        bottomWall = new Wall(leftBottomV, rightBottomV);
        topWall = new Wall(leftTopV, rightTopV);
    }
    private void AddChild(RoomNode node)
    {
        childrenNodeList.Add(node);
    }

}