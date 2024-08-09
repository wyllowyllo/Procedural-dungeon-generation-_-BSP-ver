using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class EntranceGenerator
{
    PUBLICSPACE type;
    List<RoomNode> listOfRooms;
   
    int publicSpaceDoorNumOnWall;
    int entranceSize;
   
    public EntranceGenerator(List<RoomNode> listOfRooms,PUBLICSPACE type, int publicSpaceDoorNumOnWall, int entranceSize)
    {
        this.listOfRooms = listOfRooms;
        this.publicSpaceDoorNumOnWall = publicSpaceDoorNumOnWall;
        this.type = type;
        this.entranceSize = entranceSize;

        GenerateEntrance();
    }
    void GenerateEntrance()
    {
        List<Vector2Int> roomVertexes = new List<Vector2Int>();

        //모든 방 꼭짓점 리스트 만들기
        foreach (var room in listOfRooms)
        {
            roomVertexes.Add(room.BottomLeftAreaCorner);
            roomVertexes.Add(room.BottomRightAreaCorner);
            roomVertexes.Add(room.TopLeftAreaCorner);
            roomVertexes.Add(room.TopRightAreaCorner);
        }

        RoomNode currentNode;

        foreach (var room in listOfRooms)
        {
            if (room.Parent == null)
                continue;

            currentNode = room.Parent;

            while (currentNode.Parent != null)
            {
                GenerateEntranceLogic(currentNode, roomVertexes);
                currentNode = currentNode.Parent;
            }


        }

        //공용공간이 있을 경우 공용공간에 통로 따로 생성
        if (type != PUBLICSPACE.none)
        {
            RoomNode publicSpaceNode = listOfRooms[0];

            switch (type)
            {
                //왼쪽 아래 코너에 공용공간이 있을 경우, 해당 공간의 오른쪽, 위쪽 벽면에 통로 생성
                case PUBLICSPACE.left_bottom:

                    while (publicSpaceNode.WallList[1].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[1], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //오른쪽 아래 코너에 공용공간이 있을 경우, 해당 공간의 왼쪽, 위쪽 벽면에 통로 생성
                case PUBLICSPACE.right_bottom:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //오른쪽 위 코너에 공용공간이 있을 경우, 해당 공간의 왼쪽, 아래쪽 벽면에 통로 생성
                case PUBLICSPACE.right_top:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[2], roomVertexes);
                    break;

                //왼쪽 위 코너에 공용공간이 있을 경우, 해당 공간의 아래쪽, 오른쪽 벽면에 통로 생성
                case PUBLICSPACE.left_top:
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[2], roomVertexes);
                    while (publicSpaceNode.WallList[1].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[1], roomVertexes);
                    break;

                //중앙에 공용공간이 있을 경우, 해당 공간의 4벽면에 통로 생성
                case PUBLICSPACE.center:
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[2], roomVertexes);
                    while (publicSpaceNode.WallList[1].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[1], roomVertexes);
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntranceLogic(publicSpaceNode.WallList[3], roomVertexes);
                    break;
            }
        }
    }
    void GenerateEntranceLogic(RoomNode room, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate = Vector2Int.zero;
        Wall line = room.GetDivideLine();

        //각 분할선마다 1개의 출입구 만들기
        if (line.DoorNum() != 0)
            return;

        if (line.Orientation == Orientation.Horizontal)
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(Random.Range(line.LeftVertex.x, line.RightVertex.x - entranceSize), line.LeftVertex.y);

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 x좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in room.GetDivideLine().GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Horizontal))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }
        else
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(line.LeftVertex.x, Random.Range(line.RightVertex.y, line.LeftVertex.y - entranceSize));

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 y좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in room.GetDivideLine().GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Vertical))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }

        line.AddDoor(doorCoordinate);

        //구분선 그리기
        DrawLine(line, doorCoordinate);
    }
    void GenerateEntranceLogic(Wall wall, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate = Vector2Int.zero;




        if (wall.Orientation == Orientation.Horizontal)
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x - entranceSize), wall.LeftVertex.y);

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 x좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Horizontal))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }
        else
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(wall.LeftVertex.x, Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 y좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Vertical))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }

        wall.AddDoor(doorCoordinate);

        //구분선 그리기
        DrawLine(wall, doorCoordinate);
    }

    void DrawLine(Wall line, Vector2Int doorCoordinate)
    {
        //OutLine
        GameObject door = new GameObject("Door");

        door.transform.position = Vector3.zero;
        door.transform.localScale = Vector3.one;


        LineRenderer lineDoor = door.AddComponent<LineRenderer>();
        lineDoor.positionCount = 2;
        lineDoor.startColor = Color.green;
        lineDoor.endColor = Color.green;
        lineDoor.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineDoor.sortingOrder = 1;

        lineDoor.enabled = false;

        if (line.Orientation == Orientation.Horizontal)
        {
            lineDoor.SetPosition(0, new Vector3(doorCoordinate.x, 2, doorCoordinate.y));
            lineDoor.SetPosition(1, new Vector3(doorCoordinate.x + entranceSize, 2, doorCoordinate.y));
        }
        else
        {
            lineDoor.SetPosition(0, new Vector3(doorCoordinate.x, 2, doorCoordinate.y));
            lineDoor.SetPosition(1, new Vector3(doorCoordinate.x, 2, doorCoordinate.y + entranceSize));
        }


        lineDoor.enabled = true;
    }
    bool IsDoorOverlap(Vector2Int newDoor, Vector2Int otherDoor, Orientation ori)
    {
        bool overlap = false;

        if (ori == Orientation.Horizontal)
        {
            if ((newDoor.x <= otherDoor.x && newDoor.y == otherDoor.y) && (otherDoor.x <= newDoor.x + entranceSize && newDoor.y == otherDoor.y))
                overlap = true;
            else if ((newDoor.x >= otherDoor.x && newDoor.y == otherDoor.y) && (newDoor.x <= otherDoor.x + entranceSize && newDoor.y == otherDoor.y))
                overlap = true;
        }
        else
        {
            if ((newDoor.y <= otherDoor.y && newDoor.x == otherDoor.x) && (otherDoor.y <= newDoor.y + entranceSize && newDoor.x == otherDoor.x))
                overlap = true;
            else if ((newDoor.y >= otherDoor.y && newDoor.x == otherDoor.x) && (newDoor.y <= otherDoor.y + entranceSize && newDoor.x == otherDoor.x))
                overlap = true;
        }

        return overlap;
    }

    bool IsOnVertex(Vector2Int newDoor, Vector2Int vertex, Orientation ori)
    {
        bool onVertex = false;
        if (ori == Orientation.Horizontal)
        {
            if ((newDoor.x <= vertex.x && newDoor.y == vertex.y) && (vertex.x <= newDoor.x + entranceSize && newDoor.y == vertex.y))
                onVertex = true;
        }
        else
        {
            if ((newDoor.y <= vertex.y && newDoor.x == vertex.x) && (vertex.y <= newDoor.y + entranceSize && newDoor.x == vertex.x))
                onVertex = true;
        }

        return onVertex;
    }
}