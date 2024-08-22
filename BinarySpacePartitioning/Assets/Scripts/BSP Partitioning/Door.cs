using System.Collections.Generic;
using UnityEngine;

public class Door
{
    Vector2Int doorPosition; //문의 중점
    int doorWidth;
   
    Orientation doorOrientation;
    List<RoomNode> parentRooms; //0번. 1번 인덱스의 방들이 해당 문이 연결하고 있는 방들임.
   

    public Door(Vector2Int entranceCoordinate, int entranceSize, Orientation doorOrientation)
    {
        doorPosition = entranceCoordinate;
        doorWidth = entranceSize;
        this.doorOrientation = doorOrientation;
        parentRooms = new List<RoomNode>();
    }

    public List<RoomNode> ParentRooms { get => parentRooms; set => parentRooms = value; }
    public Vector2Int DoorPosition { get => doorPosition;}
    public Orientation DoorOrientation { get => doorOrientation; }
}