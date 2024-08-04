using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpaceGenerator : MonoBehaviour
{
    RoomNode rootNode;
    List<RoomNode> allSpaceNodes = new List<RoomNode>();

    int dungeonWidth;
    int dungeonHeight;

    public RoomSpaceGenerator(int dungeonWidth, int dungeonHeight)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonHeight = dungeonHeight;
    }
    public List<RoomNode> CalculateRooms(int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax)
    {
        GroundPartitioner bsp = new GroundPartitioner(dungeonWidth, dungeonHeight);
        allSpaceNodes = bsp.PrepareNodesCollection(roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax); //트리를 구성하는 노드 전체 받아오기
        List<RoomNode> roomSpaces = FindLeafes(allSpaceNodes[0]); //반환값은 리프노드 전체


       
        return roomSpaces;
    }
    public static List<RoomNode> FindLeafes(RoomNode parentNode)
    {
        Queue<RoomNode> nodesToCheck = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<RoomNode>() { parentNode }; //그냥 새로 생성해서 return(생성자 호출)
        }
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            var currentNode = nodesToCheck.Dequeue();
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
