using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;
    int unitSize;


    public RoomNode RootNode { get => rootNode; }
    public BinarySpacePartitioner(Vector2Int startPoint,int dungeonWidth, int dungeonHeight, int unitSize)
    {
        this.rootNode = new RoomNode(startPoint, new Vector2Int(startPoint.x+dungeonWidth, startPoint.y+dungeonHeight), null, 0);
        this.unitSize = unitSize;
    }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomHeightMin, PUBLICSPACE type)
    {
        Queue<RoomNode> graph= new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        graph.Enqueue(rootNode); //순차 방문하여 Split하기 위함
        listToReturn.Add(rootNode); //전체 Room Tree

        if(type==PUBLICSPACE.none)
        {
            int i = 0;

            // maxIterations으로 분할 수(트리높이) 조정 
            while (i < maxIterations && graph.Count > 0)
            {
                i++;
                RoomNode currentNode = graph.Dequeue();
                if (currentNode.Width >= roomWidthMin * 2 || currentNode.Height >= roomHeightMin * 2)
                {
                    SplitSpace(currentNode, listToReturn, roomWidthMin, roomHeightMin, graph);
                }

            }
        }

        else
        {
            while (graph.Count > 0)
            {
                RoomNode currentNode = graph.Dequeue();
                if (currentNode.Width >= roomWidthMin * 2 || currentNode.Height >= roomHeightMin * 2)
                {
                    SplitSpace(currentNode, listToReturn, roomWidthMin, roomHeightMin, graph);
                }

            }
        }
       
        return listToReturn;
    }

    private void SplitSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomWidthMin, int roomHeightMin, Queue<RoomNode> graph)
    {
        Line line=GetLineDividingSpace(currentNode.BottomLeftAreaCorner, currentNode.TopRightAreaCorner, roomWidthMin, roomHeightMin);
        RoomNode node1, node2;

        //자식노드 생성
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y)
                                , currentNode
                                , currentNode.TreeIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y),currentNode.TopRightAreaCorner
                               , currentNode
                               , currentNode.TreeIndex + 1);

            
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y)
                                , currentNode
                                , currentNode.TreeIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y), currentNode.TopRightAreaCorner
                               , currentNode
                               , currentNode.TreeIndex + 1);

            
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

        //가로, 세로 모두 나눌 수 있을 때
        if (heightStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }

        //세로로 나누기
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }

        //가로로 나누기
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
            int y_value = Random.Range(
                    (bottomLeftAreaCorner.y + roomHeightMin), // 최소 크기의 방이 들어갈 수 있는 정도로 구획 나눔
                    (topRightAreaCorner.y - roomHeightMin));
            y_value = Mathf.RoundToInt(y_value / unitSize) * unitSize; //유닛 사이즈의 배수로 설정

            //재조정
            y_value = (y_value < (bottomLeftAreaCorner.y + roomHeightMin)) ? (bottomLeftAreaCorner.y + roomHeightMin) : y_value;
            y_value = (y_value > (topRightAreaCorner.y - roomHeightMin)) ? (topRightAreaCorner.y - roomHeightMin) : y_value;

            
            coordinates = new Vector2Int(0,y_value);
        }
        else
        {
            int x_value = Random.Range(
                    (bottomLeftAreaCorner.x + roomWidthMin),
                    (topRightAreaCorner.x - roomWidthMin));
            x_value = Mathf.RoundToInt(x_value / unitSize) * unitSize; //유닛 사이즈의 배수로 설정

            //재조정
            x_value = (x_value < (bottomLeftAreaCorner.x + roomWidthMin)) ? (bottomLeftAreaCorner.x + roomWidthMin) : x_value;
            x_value = (x_value > (topRightAreaCorner.x - roomWidthMin)) ? (topRightAreaCorner.x - roomWidthMin) : x_value;
           

            coordinates = new Vector2Int(x_value, 0);
               
        }

        return coordinates;
    }
    
}