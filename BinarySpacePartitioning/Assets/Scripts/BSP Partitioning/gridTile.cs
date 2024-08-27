using System.Numerics;
using UnityEngine;

public class gridTile
{

    //WorldPosition
    Vector2Int LeftBottomPoint, RightTopPoint;
    Vector2Int centerPoint;

    // gridPosition
    public int gridX, gridY;
   
    //해당 그리드 타일을 가진 방
    RoomNode masterRoom;

    //경로찾기에 필요한 변수
    public int gCost, hCost;
    public gridTile prev;

    public gridTile(Vector2Int leftBottomPoint, Vector2Int rightTopPoint, int gridX, int gridY)
    {
        this.LeftBottomPoint = leftBottomPoint;
        this.RightTopPoint = rightTopPoint;
        this.gridX = gridX;
        this.gridY = gridY;

        centerPoint = new Vector2Int(Mathf.CeilToInt((leftBottomPoint.x + rightTopPoint.x) / 2), Mathf.CeilToInt((leftBottomPoint.y + rightTopPoint.y) / 2));
    }

    public Vector2Int CenterPoint { get => centerPoint;}

    public void SetMasterRoom(RoomNode masterRoom)
    {
        this.masterRoom = masterRoom;   
    }
    public RoomNode GetMasterRoom()
    {
        return masterRoom;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

}