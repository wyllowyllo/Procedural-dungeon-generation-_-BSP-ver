using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    Orientation orientation;
    Vector2Int leftVertex;
    Vector2Int rightVertex;
    int length;
    List<Door> doors= new List<Door>();

    public Wall(Vector2Int leftVertex, Vector2Int rightVertex)
    {
        this.leftVertex = leftVertex;
        this.rightVertex = rightVertex;

        orientation= (leftVertex.y==rightVertex.y)?Orientation.Horizontal : Orientation.Vertical;
        length = (orientation == Orientation.Horizontal) ? Mathf.Abs(rightVertex.x - LeftVertex.x) : Mathf.Abs(rightVertex.y - LeftVertex.y);
    }

    public Vector2Int LeftVertex { get => leftVertex; }
    public Vector2Int RightVertex { get => rightVertex; }
    public Orientation Orientation { get => orientation; }
    public int Length { get => length; }

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