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
    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomHeightMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonHeight);
        allSpaceNodes=bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomHeightMin); //트리를 구성하는 노드 전체 받아오기
        RoomNode rootNode = allSpaceNodes[0];

        List<Node> roomSpaces = FindLeafes(rootNode); //반환값은 리프노드 전체

        return roomSpaces;
    }

    public List<RoomNode> NodeToRoomNode(List<Node> nodeList)
    {
        List<RoomNode> roomList = new List<RoomNode>();
        foreach (var space in nodeList)
        {
           /*space.BottomLeftAreaCorner = space.BottomLeftAreaCorner;
            space.TopRightAreaCorner = space.TopRightAreaCorner;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            roomList.Add((RoomNode)space);*/
        }
        return roomList;
    }
    public List<Node> FindLeafes(RoomNode parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode }; 
        }
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();
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
}