using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeafes(RoomNode parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();

        if (parentNode.ChildrenNodeList.Count == 0)
        {
            return new List<Node>() { parentNode }; //그냥 새로 생성해서 return(생성자 호출)
        }
        foreach (var child in parentNode.ChildrenNodeList)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            var currentNode=nodesToCheck.Dequeue();
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

    public static Vector2Int GenerateBottomLeftCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        //왼쪽아래, 오른쪽 위 좌표 부여받으면 생성할 방의 왼쪽좌표 반환
        //Offset ? 방 크기 결정하는 건가
        int minX = boundaryLeftPoint.x + offset;
        int maxX= boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
                                Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));

    }

    public static Vector2Int GenerateTopRightCornerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
                                Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY));

    }
}