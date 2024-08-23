using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RoomNode
{
    public string roomName = null;

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
   

    //리프노드(실제 방) 의 벽
    Wall leftWall;
    Wall rightWall;
    Wall bottomWall;
    Wall topWall;
    List<Wall> wallList= new List<Wall>();
    public List<Wall> WallList { get => wallList; }
    

    

    public void SetWall(int unitSize=5)
    {
        leftWall=new Wall(TopLeftAreaCorner, BottomLeftAreaCorner, unitSize);
        rightWall = new Wall(TopRightAreaCorner, BottomRightAreaCorner,unitSize);
        bottomWall = new Wall(BottomLeftAreaCorner, BottomRightAreaCorner, unitSize);
        topWall = new Wall(TopLeftAreaCorner, TopRightAreaCorner, unitSize);

        wallList.Add(leftWall);
        wallList.Add(rightWall);
        wallList.Add(bottomWall);
        wallList.Add(topWall);
    }
   
   

   /* public void PlaceObjectInRoom(Vector2Int gridPosition, Vector2Int objectSize,int unitSize, int objectType)
    {
        if (IsSpaceAvailable(gridPosition, objectSize))
        {
           
            for (int x = 0; x < objectSize.x; x++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    roomGrid[gridPosition.x + x, gridPosition.y + y] = objectType;
                }
            }
        }
        else
        {
            Debug.LogWarning("Not enough space to place the object!");
        }
    }

    bool IsSpaceAvailable(Vector2Int gridPosition, Vector2Int objectSize)
    {
     
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                int checkX = gridPosition.x + x;
                int checkY = gridPosition.y + y;

                if (checkX >= roomGrid.GetLength(0) || checkY >= roomGrid.GetLength(1) || roomGrid[checkX, checkY] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }*/



    private void AddChild(RoomNode node)
    {
        childrenNodeList.Add(node);
    }

}