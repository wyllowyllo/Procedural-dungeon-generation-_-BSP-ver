using System.Numerics;
using UnityEngine;

public class gridTile
{
    Vector2Int LeftBottomPoint, RightTopPoint;
    Vector2Int centerPoint;
    RoomNode masterRoom;

    public gridTile(Vector2Int leftBottomPoint, Vector2Int rightTopPoint)
    {
        this.LeftBottomPoint = leftBottomPoint;
        this.RightTopPoint = rightTopPoint;

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
   
}