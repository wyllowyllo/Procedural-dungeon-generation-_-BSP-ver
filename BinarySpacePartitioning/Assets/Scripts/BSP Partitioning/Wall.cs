using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    Orientation orientation;
    Vector2Int leftVertex;
    Vector2Int rightVertex;
   
    int length;
    int unitSize;
    List<Vector3> wallObjPoint=new List<Vector3>();
    List<Door> doors= new List<Door>();

    public Wall(Vector2Int leftVertex, Vector2Int rightVertex, int unitSize)
    {
        this.leftVertex = leftVertex;
        this.rightVertex = rightVertex;
        this.unitSize = unitSize;

        orientation= (leftVertex.y==rightVertex.y)?Orientation.Horizontal : Orientation.Vertical;
        length = (orientation == Orientation.Horizontal) ? Mathf.Abs(rightVertex.x - LeftVertex.x) : Mathf.Abs(rightVertex.y - LeftVertex.y);
        SetWallObjList();
    }

    public Vector2Int LeftVertex { get => leftVertex; }
    public Vector2Int RightVertex { get => rightVertex; }
    public Orientation Orientation { get => orientation; }
    public int Length { get => length; }

    void SetWallObjList()
    {
        Vector2Int LeftV;
        Vector2Int RightV;
        Vector2Int centerV;

        for (int i = 0; i < length / unitSize; i++)
        {
            if(Orientation == Orientation.Horizontal)
            {
                LeftV = new Vector2Int(LeftVertex.x + (i * unitSize),leftVertex.y);
                RightV = new Vector2Int(LeftV.x + unitSize, LeftV.y); 
            }
            else
            {
                LeftV = new Vector2Int(LeftVertex.x, leftVertex.y - (i * unitSize));
                RightV = new Vector2Int(LeftV.x, LeftV.y+unitSize);
            }

            centerV = SetCenterPoint(LeftV, RightV);
            wallObjPoint.Add(new Vector3(centerV.x, 0, centerV.y));
        }

       
       
    }
    Vector2Int SetCenterPoint(Vector2Int leftV, Vector2Int RightV)
    {
        Vector2Int centerPoint = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
            centerPoint = new Vector2Int(Mathf.CeilToInt((leftV.x + RightV.x) / 2), leftV.y);
        else
            centerPoint = new Vector2Int(leftV.x, Mathf.CeilToInt((leftV.y + RightV.y) / 2));

        return centerPoint;
    }
   
    public void AddDoor(Door door)
    {
        doors.Add(door);
    }
    public List<Door> GetDoorList()
    {
        return doors;
    }
    public int DoorNum()
    {
        return doors.Count;
    }

   
}