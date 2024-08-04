using UnityEngine;

public class RoomNode:Node
{
    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        this.TreeIndex = index;
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

}