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
        allSpaceNodes = bsp.PrepareNodesCollection(roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax); //Ʈ���� �����ϴ� ��� ��ü �޾ƿ���
        List<RoomNode> roomSpaces = FindLeafes(allSpaceNodes[0]); //��ȯ���� ������� ��ü


       
        return roomSpaces;
    }
    public static List<RoomNode> FindLeafes(RoomNode parentNode)
    {
        Queue<RoomNode> nodesToCheck = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<RoomNode>() { parentNode }; //�׳� ���� �����ؼ� return(������ ȣ��)
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
                listToReturn.Add(currentNode); //�ڽ� ���ٸ�(������� ����) ��ȯ�� ����Ʈ�� �߰�
            }
            else
            {
                foreach (var child in currentNode.ChildrenNodeList)
                {
                    nodesToCheck.Enqueue(child); //�ڽ� ��� Ž���ؾ� �ϹǷ� ��ť
                }
            }
        }
        return listToReturn;
    }

    
}
