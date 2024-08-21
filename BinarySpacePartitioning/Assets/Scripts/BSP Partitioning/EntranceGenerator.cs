using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using System.Reflection;
public class EntranceGenerator
{
    PUBLICSPACE type;
    List<RoomNode> listOfRooms;
    List<Vector2Int> EntranceHorizontalPos;
    List<Vector2Int> EntranceVerticalPos;
    List<Vector2Int> CreatedEntrancePos; //실제 생성 완료된 통로 위치 저장
    

    
    int entranceSize;
   
    public EntranceGenerator(List<RoomNode> listOfRooms,PUBLICSPACE type, int unitSize)
    {
        this.listOfRooms = listOfRooms;
        this.type = type;
        this.entranceSize = unitSize;

    }
    public void SetEntrancePossibleList(List<Vector2Int> entranceHPos, List<Vector2Int> entranceVList)
    {
        this.EntranceHorizontalPos = entranceHPos;
        this.EntranceVerticalPos= entranceVList;
    }

    public void GenerateEntrance()
    {

        MakepossibleEntrancePos();
        GenerateEntranceLogic();

    }
   

    void GenerateEntranceLogic() 
    {
        CreatedEntrancePos = new List<Vector2Int>();
        List<Vector2Int> prevPoint = new List<Vector2Int>();

        //통로 생성 가능한 가로 벽면 하나당 1개의 문 생성
        for (int i = 0; i < EntranceHorizontalPos.Count-1; i++)
        {
            Vector2Int point1 = EntranceHorizontalPos[i];
            List<Vector2Int> pointList = new List<Vector2Int>();


            bool flag=false;
            foreach (var prev in prevPoint)
                if (prev == point1)
                    flag=true;

            if (flag) continue;

            pointList.Add(point1);
            for (int j=i+1;j<EntranceHorizontalPos.Count ; j++)
            {
                Vector2Int point2 = EntranceHorizontalPos[j];
              

                if (IsOnSameLine(point1, point2, Orientation.Horizontal))
                {
                    pointList.Add(point2);
                    prevPoint.Add(point2);
                    point1 = point2;
                }
             

            }

            if (pointList.Count > 0)
            {
                //문 생성
                Vector2Int entranceCoordinate = pointList[Random.Range(0, pointList.Count)];
                Door door = ConnectWallAndDoorInfo(entranceCoordinate, Orientation.Horizontal);
                CreatedEntrancePos.Add(entranceCoordinate);
                //TODO:문 객체 생성하기
                
            }
           
        }


        //통로 생성 가능한 세로 벽면 하나당 1개의 문 생성
        prevPoint = new List<Vector2Int>();
        for (int i = 0; i < EntranceVerticalPos.Count - 1; i++)
        {
            Vector2Int point1 = EntranceVerticalPos[i];
            List<Vector2Int> pointList = new List<Vector2Int>();

            bool flag = false;
            foreach (var prev in prevPoint)
                if (prev == point1)
                    flag = true;

            if (flag) continue;

            pointList.Add(point1);

            for (int j = i + 1; j < EntranceVerticalPos.Count; j++)
            {
                Vector2Int point2 = EntranceVerticalPos[j];
                if (IsOnSameLine(point1, point2, Orientation.Vertical))
                {
                    pointList.Add(point2);
                    prevPoint.Add(point2);
                    point1 = point2;
                }
               
            }

            //문 생성
            if (pointList.Count > 0)
            {
                Vector2Int entranceCoordinate = pointList[Random.Range(0, pointList.Count)];
                Door door = ConnectWallAndDoorInfo(entranceCoordinate, Orientation.Vertical);
                CreatedEntrancePos.Add(entranceCoordinate);
                //TODO:문 객체 생성하기
            }


        }

    }


    Door ConnectWallAndDoorInfo(Vector2Int doorCoordinate, Orientation doorOrientation)
    {
       List<RoomNode> parentRooms = new List<RoomNode>();
       Door door = new Door(doorCoordinate, entranceSize, doorOrientation);

        //모든 방의 벽면 조사하여, 해당 문 좌표에 해당하는 벽을 가진 방에 문 정보 추가
        foreach (var room in listOfRooms)
        {
            foreach(var wall in room.WallList)
            {
                if(IsDoorInWallRange(wall, doorCoordinate, doorOrientation))
                {
                    wall.AddDoor(door);
                    parentRooms.Add(room);
                }
                   
            }
        }
        
        door.ParentRooms = parentRooms; //해당 문이 연결하는 두 방(안쪽, 바깥쪽) 추가

        return door;
    }

    bool IsDoorInWallRange(Wall wall, Vector2Int doorPos, Orientation ori)
    {
        bool InRange = false;

        if(ori == Orientation.Horizontal)
        {
            if((wall.LeftVertex.x<=doorPos.x-entranceSize/2 && wall.LeftVertex.y==doorPos.y) &&(wall.RightVertex.x>=doorPos.x+entranceSize/2))
                InRange= true;
        }
        else
        {
            if ((wall.RightVertex.y <= doorPos.y-entranceSize/2 && wall.RightVertex.x == doorPos.x) && (wall.LeftVertex.y >= doorPos.y + entranceSize/2))
                InRange = true;
        }

        return InRange;
    }

    
    void DrawLine(Orientation ori,Door door)
    {
        //OutLine
        GameObject doorLine = new GameObject("Door  " + (door.ParentRooms[0].roomName)+"-"+(door.ParentRooms[1].roomName));

        doorLine.transform.position = Vector3.zero;
        doorLine.transform.localScale = Vector3.one;


        LineRenderer lineDoor = doorLine.AddComponent<LineRenderer>();
        lineDoor.positionCount = 2;
        lineDoor.startColor = Color.green;
        lineDoor.endColor = Color.green;
        lineDoor.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineDoor.sortingOrder = 1;

        lineDoor.enabled = false;

        if (ori == Orientation.Horizontal)
        {
            lineDoor.SetPosition(0, new Vector3(door.DoorLeftPoint.x, 2, door.DoorLeftPoint.y));
            lineDoor.SetPosition(1, new Vector3(door.DoorRightPoint.x + entranceSize, 2, door.DoorRightPoint.y));
        }
        else
        {
            lineDoor.SetPosition(0, new Vector3(door.DoorRightPoint.x, 2, door.DoorRightPoint.y));
            lineDoor.SetPosition(1, new Vector3(door.DoorLeftPoint.x, 2, door.DoorLeftPoint.y));
        }


        lineDoor.enabled = true;
    }

    bool IsOnVertex(Vector2Int newDoor, Vector2Int vertex, Orientation ori)
    {
        bool onVertex = false;
        if (ori == Orientation.Horizontal)
        {
            /*if ((newDoor.x <= vertex.x && newDoor.y == vertex.y) && (vertex.x <= newDoor.x + entranceSize && newDoor.y == vertex.y))
                onVertex = true;*/
            if(IsWithinRange(vertex.x, newDoor.x-entranceSize/2, newDoor.x+entranceSize/2)&& vertex.y==newDoor.y)
                onVertex = true;
        }
        else
        {
            /* if ((newDoor.y <= vertex.y && newDoor.x == vertex.x) && (vertex.y <= newDoor.y + entranceSize && newDoor.x == vertex.x))
                 onVertex = true;*/
            if (IsWithinRange(vertex.y, newDoor.y - entranceSize / 2, newDoor.y + entranceSize / 2) && vertex.x == newDoor.x)
                onVertex = true;
        }

        return onVertex;
    }
    bool IsWithinRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    void MakepossibleEntrancePos()
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

        //방문 생성 위치(가로)가 꼭짓점에 겹치지 않도록 함
        var pointsToRemove = new List<Vector2Int>();
        foreach (var point in EntranceHorizontalPos)
        {
            foreach(var vertexPoint in roomVertexes)
            {
                if (IsOnVertex(point, vertexPoint, Orientation.Horizontal))
                    pointsToRemove.Add(point);
            }
        }
        foreach (var point in pointsToRemove)
            EntranceHorizontalPos.Remove(point);

        //방문 생성 위치(세로)가 꼭짓점에 겹치지 않도록 함
        pointsToRemove = new List<Vector2Int>();
        foreach (var point in EntranceVerticalPos)
        {
            foreach (var vertexPoint in roomVertexes)
            {
                if (IsOnVertex(point, vertexPoint, Orientation.Vertical))
                    pointsToRemove.Add(point);
            }
        }
        foreach (var point in pointsToRemove)
            EntranceVerticalPos.Remove(point);

       
    }

    bool IsOnSameLine(Vector2Int point1, Vector2Int point2, Orientation ori)
    {
        bool OnSameLine = false;

        if(ori == Orientation.Horizontal)
        {
            if ((point1.y == point2.y) && (point1.x + entranceSize == point2.x))
                OnSameLine = true;
        }
            

        else
        {
            if ((point1.x == point2.x) && (point1.y - entranceSize == point2.y))
                OnSameLine = true;
        }
        

        return OnSameLine;
    }

    public List<Vector2Int> GetEntrancePos()
    {
        return CreatedEntrancePos;
    }
}