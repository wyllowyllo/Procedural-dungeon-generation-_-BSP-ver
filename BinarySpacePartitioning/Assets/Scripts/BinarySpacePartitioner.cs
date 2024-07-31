﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;
    private int dungeonWidth;
    private int dungeonHeight;

    public RoomNode RootNode { get => rootNode; }
    public BinarySpacePartitioner(int dungeonWidth, int dungeonHeight)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight), null, 0);
    }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomHeightMin)
    {
        Queue<RoomNode> graph= new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        graph.Enqueue(rootNode); //순차 방문하여 Split하기 위함
        listToReturn.Add(rootNode); //전체 Room Tree

        int i = 0;
        while (i < maxIterations && graph.Count > 0)
        {
            i++;
            RoomNode currentNode= graph.Dequeue();
            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Height >= roomHeightMin * 2) 
            {
                SplitTheSpace(currentNode, listToReturn, roomWidthMin, roomHeightMin, graph);
            }

        }
        return listToReturn;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomWidthMin, int roomHeightMin, Queue<RoomNode> graph)
    {
        Line line=GetLineDividingSpace(currentNode.BottomLeftAreaCorner, currentNode.TopRightAreaCorner, roomWidthMin, roomHeightMin);
        RoomNode node1, node2;

        //자식노드 생성
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y)
                                , currentNode
                                , currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y),currentNode.TopRightAreaCorner
                               , currentNode
                               , currentNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y)
                                , currentNode
                                , currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y), currentNode.TopRightAreaCorner
                               , currentNode
                               , currentNode.TreeLayerIndex + 1);
        }

        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);
    }

    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node); 
    }

    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomHeightMin)
    {
        Orientation orientation;
        bool heightStatus=(topRightAreaCorner.y-bottomLeftAreaCorner.y) >=2*roomHeightMin;
        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= 2 * roomWidthMin;

        if (heightStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;

        }
        return new Line(orientation, GetCoordinatgesFororientation(
            orientation,
            bottomLeftAreaCorner,
            topRightAreaCorner,
            roomWidthMin,
            roomHeightMin
            ));
    }

    private Vector2Int GetCoordinatgesFororientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomHeightMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        //방향에 따라 생성할 구획 좌표값 설정 
        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2Int(
                0,
                Random.Range(
                    (bottomLeftAreaCorner.y + roomHeightMin), // 최소 크기의 방이 들어갈 수 있는 정도로 구획 나눔
                    (topRightAreaCorner.y - roomHeightMin)
                ));
        }
        else
        {
            coordinates = new Vector2Int(
                Random.Range(
                    (bottomLeftAreaCorner.x + roomWidthMin),
                    (topRightAreaCorner.x - roomWidthMin)
                )
                ,0);
        }

        return coordinates;
    }
}