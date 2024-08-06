using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    Orientation orientation;
    Vector2Int leftVertex;
    Vector2Int rightVertex;
    int length;
    List<Vector2Int> doorPos= new List<Vector2Int>();

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

    public void AddDoor(Vector2Int doorPosition)
    {
        doorPos.Add(doorPosition);
    }
    public int DoorNum()
    {
        return doorPos.Count;
    }

    public bool sameWall(Wall wall)
    {
        if(this.orientation==wall.orientation)
            if(this.leftVertex==wall.leftVertex && this.rightVertex==wall.rightVertex) 
                return true;

        return false;
    }
}