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
    public List<Node> CalculateRooms(int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax)
    {
        GroundPartitioner bsp = new GroundPartitioner(dungeonWidth, dungeonHeight);
        allSpaceNodes = bsp.PrepareNodesCollection(roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax); //Ʈ���� �����ϴ� ��� ��ü �޾ƿ���
        List<Node> roomSpaces = FineLeafes(bsp.RootNode); //��ȯ���� ������� ��ü

        
        List<RoomNode> roomList = GenerateRoomsInGivenSpaces(roomSpaces); 
        return new List<Node>(roomList);
    }
    public static List<Node> FineLeafes(RoomNode parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode }; //�׳� ���� �����ؼ� return(������ ȣ��)
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

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = space.BottomLeftAreaCorner;
            Vector2Int newTopRightPoint = space.TopRightAreaCorner;

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            listToReturn.Add((RoomNode)space);
        }
        return listToReturn;
    }
}
