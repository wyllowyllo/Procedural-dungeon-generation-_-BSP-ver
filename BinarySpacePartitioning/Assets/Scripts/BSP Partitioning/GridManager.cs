using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using System.IO;
using System.Text;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System;
using System.Linq;
public class GridManager
{
    //Grid배열
    private gridTile[,] dungeonGrid;
    

    int cellSize = 1;
    private int dungeonWidth;
    private int dungeonHeight;
    private int gridWidth;
    private int gridHeight;

    private List<RoomNode> listOfRooms;
    private List<Door> listOfDoors;

    Vector2Int startPoint = Vector2Int.zero;

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
                gridTile tile=new gridTile(gridLeftBottom, new Vector2Int(gridLeftBottom.x+cellSize, gridLeftBottom.y+cellSize));
                FindMasterRoom(tile);

                dungeonGrid[i,j] = tile;
                
                
            }
        }

    }

    public Vector2Int GridToWorldPosition(int gridRow, int gridCol)
    {
        int worldX = startPoint.x + gridCol * cellSize;
        int worldY = startPoint.y + gridRow * cellSize;
        return new Vector2Int(worldX, worldY); //반환값은 각 타일의 왼쪽 아래 월드좌표
    }

    public Vector2Int WorldToGridPosition(Vector2Int worldPosition, int unitSize = 5)
    {
        int gridX = (worldPosition.x - startPoint.x) / unitSize;
        int gridY = (worldPosition.y - startPoint.y) / unitSize;
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

        //왼쪽, 아래, 오른쪽, 위
        int[,] dir = new int[,] { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } };
        bool[,] visited = new bool[gridHeight, gridWidth];

        //grid의 행,열 인덱스 값과 그에 해당하는 gridTile전달
        Block startBlock = new Block(startTile.x, startTile.y,dungeonGrid[startTile.y, startTile.x]);
        startBlock.PrevBlock=null;

        SortedDictionary<int, List<Block>> blockQueue = new SortedDictionary<int, List<Block>>(); //키값 중복 허용하는 Priority Queue구현. 이때 키는 totalWeight.
        AddToQueue(blockQueue, startBlock);

        Block current;
        Stack<Block> path = new Stack<Block>();

        while (blockQueue.Count != 0)
        {
            current = GetNextBlock(blockQueue);
            visited[current.GridY, current.GridX] = true;

            if (current.BlockTile == dungeonGrid[endTile.y, endTile.x]) //목적지 도착시 경로 스택에 저장
            {
                Block pathBlock = current;
                while (pathBlock!= null)
                {
                    path.Push(pathBlock);
                    pathBlock = pathBlock.PrevBlock;
                }

                break;
            }

            for(int i = 0; i < dir.GetLength(0); i++)
            {
                int nextX = current.GridX + dir[i, 0];
                int nextY = current.GridY + dir[i, 1];
                bool IsPossible = false;
                //그리드 범위 내인지
                if(nextX >= 0 && nextX < gridWidth && nextY >= 0 && nextY < gridHeight)
                {
                    //같은 방의 타일인지
                    if (current.BlockTile.GetMasterRoom() == dungeonGrid[nextY, nextX].GetMasterRoom())
                    {
                        //방문하지 않았다면
                        if (!visited[nextY,nextX])
                        {
                           IsPossible = true;
                        }
                    }
                    else
                    {
                        //다른 방이라면, 두 타일 사이 위치에 출입구가 존재하는지
                       if(IsEntranceExistBetween(current.BlockTile, dungeonGrid[nextY, nextX]) && !visited[nextY, nextX])
                        {
                           IsPossible=true;
                        }
                    }
                }
                if (IsPossible)
                {
                    //블럭 추가
                    Block nextBlock = new Block(nextX, nextY, dungeonGrid[nextY, nextX]);
                    nextBlock.PrevBlock = current;
                    nextBlock.FromStartPointWeight = current.FromStartPointWeight + cellSize;
                    nextBlock.FromDestPointWeight = (Mathf.Abs(endTile.x - nextX) + Mathf.Abs(endTile.y - nextY)) * cellSize;

                    nextBlock.UpdateTotalWeight();
                    AddToQueue(blockQueue, nextBlock); //TotalWeight 오름차순으로 정렬됨.
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
                Block block= path.Pop();
                pathInfo.Add(block.BlockTile.CenterPoint);
            }
        }
        //경로 찾지 못했을 때
        else
        {
            pathInfo = null;

        }

        return pathInfo;
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
            if ((FromTile.CenterPoint.y <= door.DoorPosition.y && door.DoorPosition.y <= ToTile.CenterPoint.y) && door.DoorPosition.x == FromTile.CenterPoint.x)
            {
                EntranceExist = true; break;
            }
        }

        return EntranceExist;
    }

    //Dictionary에 원소 추가
    private void AddToQueue(SortedDictionary<int, List<Block>> queue, Block block)
    {
        if (!queue.ContainsKey(block.TotalWeight))
        {
            queue[block.TotalWeight] = new List<Block>();
        }
        queue[block.TotalWeight].Add(block);
    }

    //Dictionary에서 첫번째 원소 가져오기
    private Block GetNextBlock(SortedDictionary<int, List<Block>> queue)
    {
        var firstKey = queue.Keys.First();
        var block = queue[firstKey][0];
        queue[firstKey].RemoveAt(0);
        if (queue[firstKey].Count == 0)
        {
            queue.Remove(firstKey);
        }
        return block;
    }


}