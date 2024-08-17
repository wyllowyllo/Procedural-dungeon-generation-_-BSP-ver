using System.Collections.Generic;
using UnityEngine;

public class Door
{
    Vector2Int DoorLeftPoint; //문의 왼쪽 끝 또는 위 끝 좌표 
    Vector2Int DoorRightPoint;//문의 오른쪽 끝 또는 아래 끝 좌표 
    Orientation doorOrientation;
    List<RoomNode> parentRooms; //0번. 1번 인덱스의 방들이 해당 문이 연결하고 있는 방들임.
    //Dictionary<string, RoomNode> rooms;

    public Door(Vector2Int entranceCoordinate, int entranceSize, Orientation doorOrientation)
    {
        if (doorOrientation == Orientation.Horizontal)
        {
            DoorLeftPoint = entranceCoordinate;
            DoorRightPoint = new Vector2Int(entranceCoordinate.x + entranceSize, entranceCoordinate.y);
            this.doorOrientation = doorOrientation;
        }
        else
        {
            DoorRightPoint = entranceCoordinate;
            DoorLeftPoint = new Vector2Int(entranceCoordinate.x, entranceCoordinate.y + entranceSize);
            this.doorOrientation = doorOrientation;

        }
      
        parentRooms = new List<RoomNode>();
    }

    public List<RoomNode> ParentRooms { get => parentRooms; set => parentRooms = value; }
}