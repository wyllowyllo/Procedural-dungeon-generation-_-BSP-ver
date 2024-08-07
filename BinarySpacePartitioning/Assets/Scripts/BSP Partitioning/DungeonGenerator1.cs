using System;
using System.Collections.Generic;
using UnityEngine;

internal class DungeonGenerator
{
    RoomNode rootNode;
    List<RoomNode> allSpaceNodes = new List<RoomNode>();
     
    int dungeonWidth;
    int dungeonHeight;

    public DungeonGenerator(int dungeonWidth, int dungeonHeight)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonHeight = dungeonHeight;
    }
    public List<RoomNode> CalculateRooms(Vector2Int startPoint, int maxIterations, int roomWidthMin, int roomHeightMin, PUBLICSPACE type)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(startPoint,dungeonWidth, dungeonHeight);
        allSpaceNodes=bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomHeightMin, type); //트리를 구성하는 노드 전체 받아오기
        RoomNode rootNode = allSpaceNodes[0];

        List<RoomNode> roomSpaces = FindLeafes(rootNode); //반환값은 리프노드 전체

        return roomSpaces;
    }
   

    public List<RoomNode> FindLeafes(RoomNode parentNode)
    {
        Queue<RoomNode> nodesToCheck = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<RoomNode>() { parentNode }; 
        }
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            RoomNode currentNode = nodesToCheck.Dequeue();
            if (currentNode.ChildrenNodeList.Count == 0)
            {
                listToReturn.Add(currentNode); //자식 없다면(리프노드 도달) 반환값 리스트에 추가
            }
            else
            {
                foreach (var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child); //자식 노드 탐방해야 하므로 인큐
                }
            }
        }
        return listToReturn;
    }
    public RoomNode GetRootNode()
    {
        return rootNode;
    }
}