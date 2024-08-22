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
    List<Door> CreatedEntranceList; //실제 생성 완료된 통로 위치 저장

    //생성할 통로 오브젝트
    GameObject doorHorizontalObj; 
    GameObject doorVerticalObj;

    
    int entranceSize;
   
    public EntranceGenerator(List<RoomNode> listOfRooms,PUBLICSPACE type, int unitSize, GameObject doorHorizontalObj, GameObject doorVerticalObj)
    {
        this.listOfRooms = listOfRooms;
        this.type = type;
        this.entranceSize = unitSize;

        this.doorHorizontalObj = doorHorizontalObj;
        this.doorVerticalObj = doorVerticalObj;
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
        CreatedEntranceList = new List<Door>();
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
                //TODO: 랜덤함수 활용해서 문 생성확률 설정할수 있게 하고, 이후 방 조사해서 출입구 없는 방은 통로 1개 따로 생성하도록 하기
                Vector2Int entranceCoordinate = pointList[Random.Range(0, pointList.Count)];
                Door door = ConnectWallAndDoorInfo(entranceCoordinate, Orientation.Horizontal);
                CreatedEntranceList.Add(door);
               
                
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
                CreatedEntranceList.Add(door);
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

    
   

    bool IsOnVertex(Vector2Int newDoor, Vector2Int vertex, Orientation ori)
    {
        bool onVertex = false;
        if (ori == Orientation.Horizontal)
        {
            if(IsWithinRange(vertex.x, newDoor.x-entranceSize/2, newDoor.x+entranceSize/2)&& vertex.y==newDoor.y)
                onVertex = true;
        }
        else
        {
            if (IsWithinRange(vertex.y, newDoor.y - entranceSize / 2, newDoor.y + entranceSize / 2) && vertex.x == newDoor.x)
                onVertex = true;
        }

        return onVertex;
    }
    bool IsWithinRange(int value, int min, int max)
    {
        return value > min && value < max;
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

    public List<Door> GetEntrancePos()
    {
        return CreatedEntranceList;
    }
}