using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using System.IO;
using System.Text;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
public class GridManager
{
    //Grid배열
    private gridTile[,] dungeonGrid;
    

    int cellSize;
    private int dungeonWidth;
    private int dungeonHeight;
    private int gridWidth;
    private int gridHeight;

    private List<RoomNode> listOfRooms;
    private List<Door> listOfDoors;

    Vector3 startPoint = Vector3.zero;

    public gridTile[,] DungeonGrid { get => dungeonGrid; }

    public GridManager(int dungeonWidth, int dungeonHeight, List<RoomNode> listOfRooms, List<Door> listOfDoors, int unitSize)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonHeight = dungeonHeight;
        this.listOfRooms = listOfRooms;
        this.listOfDoors = listOfDoors;
        cellSize = unitSize;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridWidth = (dungeonWidth / cellSize);
        gridHeight=(dungeonHeight / cellSize);


        dungeonGrid = new gridTile[gridHeight, gridWidth];

        for (int i = 0; i < gridHeight; i++)
        {
            for(int j=0;j< gridWidth; j++)
            {
                Vector2Int gridLeftBottom=GridToWorldPosition(i, j);
                gridTile tile=new gridTile(gridLeftBottom, new Vector2Int(gridLeftBottom.x+cellSize, gridLeftBottom.y+cellSize),j, i);
                FindMasterRoom(tile);

                dungeonGrid[i,j] = tile;
                
                
            }
        }

    }

    public Vector2Int GridToWorldPosition(int gridRow, int gridCol)
    {
        int worldX = (int)(startPoint.x + gridCol * cellSize);
        int worldY = (int)(startPoint.y + gridRow * cellSize);
        return new Vector2Int(worldX, worldY); //반환값은 각 타일의 왼쪽 아래 월드좌표
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        if (worldPosition.x < 0 || worldPosition.x >= dungeonWidth || worldPosition.z < 0 || worldPosition.z >= dungeonHeight)
            return new Vector2Int(-1, -1);

        int gridX = (int)((worldPosition.x - startPoint.x) / cellSize);
        int gridY = (int)((worldPosition.z - startPoint.y) / cellSize); // Z축 사용
        return new Vector2Int(gridX, gridY);
    }

    void FindMasterRoom(gridTile tile)
    {
        foreach(var room in listOfRooms)
        {
            if((room.BottomLeftAreaCorner.x<=tile.CenterPoint.x)&&(room.BottomLeftAreaCorner.y <= tile.CenterPoint.y)&&
                (room.TopRightAreaCorner.x >= tile.CenterPoint.x) && (room.TopRightAreaCorner.y >= tile.CenterPoint.y))
            {
                tile.SetMasterRoom(room);
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int startTile, Vector2Int endTile)
    {
        List<gridTile> openSet = new List<gridTile>();
        HashSet<gridTile> closedSet = new HashSet<gridTile>();

        InitializePathVariables();

        gridTile start = dungeonGrid[startTile.y, startTile.x];
        start.gCost = 0;
        start.hCost = GetDistance(start, dungeonGrid[endTile.y, endTile.x]);
       
        openSet.Add(start);

       

        gridTile current;
        Stack<gridTile> path = new Stack<gridTile>();

        while (openSet.Count > 0)
        {
            //최소비용 타일 선택
            current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost || openSet[i].fCost == current.fCost)
                {
                    if (openSet[i].hCost < current.hCost)
                        current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == dungeonGrid[endTile.y, endTile.x]) //목적지 도착시 경로 스택에 저장
            {
               
                while (current!= null)
                {
                    path.Push(current);
                    current = current.prev;
                }

                break;
            }

            List<gridTile> neigbours=GetNeighbours(current); //이동 가능한 주변타일
            foreach(var neighbour in neigbours)
            {
                if (closedSet.Contains(neighbour))
                    continue;

                int new_gcost = current.gCost + GetDistance(current, neighbour);
                if (new_gcost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = new_gcost;
                    neighbour.hCost = GetDistance(neighbour, dungeonGrid[endTile.y,endTile.x]);
                    neighbour.prev = current;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
           

        }

        List<Vector2Int> pathInfo;
        //목적지까지의 경로 찾았을 때
        if (path.Count > 0)
        {
            pathInfo = new List<Vector2Int>();
            while (path.Count > 0)
            {
                gridTile tile= path.Pop();
                pathInfo.Add(tile.CenterPoint);
            }
        }
        //경로 찾지 못했을 때
        else
        {
            pathInfo = null;

        }

        openSet.Clear();
        closedSet.Clear();

        return pathInfo;
    }
    public List<gridTile> GetNeighbours(gridTile tile)
    {
        List<gridTile> neighbours = new List<gridTile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int nextX = tile.gridX + x;
                int nextY = tile.gridY + y;

              
                if (nextX >= 0 && nextX < gridWidth && nextY >= 0 && nextY < gridHeight)
                {
                
                    //타일이 속한 방이 다를 경우 그 사이에 출입구 위치 있으면 이동
                    if (tile.GetMasterRoom() != dungeonGrid[nextY, nextX].GetMasterRoom())
                    {
                        //타일이 속한 방이 다를 경우 대각선 이동 금지
                        if ((x == -1 && y == -1) || (x == 1 && y == 1) || (x == -1 && y == 1) || (x == 1 && y == -1))
                                continue;

                        else
                        {
                            if(!IsEntranceExistBetween(tile, dungeonGrid[nextY, nextX]))
                                continue;
                        }
                    }

                    neighbours.Add(dungeonGrid[nextY, nextX]);
                }
            }
        }

        return neighbours;
    }

    int GetDistance(gridTile nodeA, gridTile nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //대각선 이동이 가능한 만큼 최대한 대각선으로 이동.
        if (dstX > dstY)
            return 7 * dstY + 5 * (dstX - dstY);
        return 7 * dstX + 5 * (dstY - dstX);
    }

    bool IsEntranceExistBetween(gridTile FromTile, gridTile ToTile)
    {
        bool EntranceExist = false;
       
        foreach (var door in listOfDoors)
        {
            if ((FromTile.CenterPoint.x<= door.DoorPosition.x&&door.DoorPosition.x<=ToTile.CenterPoint.x)&&door.DoorPosition.y==FromTile.CenterPoint.y)
            {
                EntranceExist=true; break;
            }
            else if ((FromTile.CenterPoint.y <= door.DoorPosition.y && door.DoorPosition.y <= ToTile.CenterPoint.y) && door.DoorPosition.x == FromTile.CenterPoint.x)
            {
                EntranceExist = true; break;
            }
            else if ((FromTile.CenterPoint.x >= door.DoorPosition.x && door.DoorPosition.x >= ToTile.CenterPoint.x) && door.DoorPosition.y == FromTile.CenterPoint.y)
            {
                EntranceExist = true; break;
            }
            else if ((FromTile.CenterPoint.y >= door.DoorPosition.y && door.DoorPosition.y >= ToTile.CenterPoint.y) && door.DoorPosition.x == FromTile.CenterPoint.x)
            {
                EntranceExist = true; break;
            }
        }

        return EntranceExist;
    }

    public void InitializePathVariables()
    {
        foreach (var tile in dungeonGrid)
        {
            tile.gCost = int.MaxValue;
            tile.hCost = int.MaxValue;
            tile.prev = null;
        }
    }


}