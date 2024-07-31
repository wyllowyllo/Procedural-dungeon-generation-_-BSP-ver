using System.Collections.Generic;
using UnityEngine;

public class GroundPartitioner
{
    RoomNode rootNode;
    private int totalWidth;
    private int totalHeight;

    public RoomNode RootNode { get => rootNode; }
    public GroundPartitioner(int dungeonWidth, int dungeonHeight)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight), null, 0);
    }

    public List<RoomNode> PrepareNodesCollection(int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        graph.Enqueue(rootNode); //순차 방문하여 Split하기 위함
        listToReturn.Add(rootNode); //전체 Room Tree

       
        while (graph.Count > 0)
        {
          
            RoomNode currentNode = graph.Dequeue();
            if (currentNode.Width >= roomWidthMin * 2 && currentNode.Height >= roomHeightMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax, graph);
            }

        }
        return listToReturn;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomWidthMin, int roomHeightMin, int roomWidthMax, int roomHeightMax, Queue<RoomNode> graph)
    {
        RoomNode node1, node2, node3, node4;
        int roomWidthRange_Min=roomWidthMin;
        int roomHeightRange_Min=roomHeightMin;
        int roomWidthRange_Max;
        int roomHeightRange_Max;

        if ((currentNode.TopRightAreaCorner.x - roomWidthMin) > (currentNode.BottomLeftAreaCorner.x + roomWidthMax))
            roomWidthRange_Max = (currentNode.BottomLeftAreaCorner.x + roomWidthMax);
        else
            roomWidthRange_Max = (currentNode.TopRightAreaCorner.x - roomWidthMin);

        if ((currentNode.TopRightAreaCorner.y - roomHeightMin) > (currentNode.BottomLeftAreaCorner.y + roomHeightMax))
            roomHeightRange_Max = (currentNode.BottomLeftAreaCorner.y + roomHeightMax);
        else
            roomHeightRange_Max = (currentNode.TopRightAreaCorner.y - roomHeightMin);

            //왼쪽아래 방
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(Random.Range(roomWidthRange_Min, roomWidthRange_Max)
                                ,Random.Range(roomHeightRange_Min, roomHeightRange_Max))
                                , currentNode
                                , currentNode.TreeLayerIndex + 1);
            //오른쪽 아래 방
            node2 = new RoomNode(node1.BottomRightAreaCorner, new Vector2Int(currentNode.BottomRightAreaCorner.x, node1.TopRightAreaCorner.y)
                               , currentNode
                               , currentNode.TreeLayerIndex + 1);

            //오른쪽 위 방
            node3 = new RoomNode(node1.TopRightAreaCorner, currentNode.TopRightAreaCorner
                                , currentNode
                                , currentNode.TreeLayerIndex + 1);

            //왼쪽 위 방
            node4 = new RoomNode(node1.TopLeftAreaCorner, node3.TopLeftAreaCorner
                               , currentNode
                               , currentNode.TreeLayerIndex + 1);
        

        AddNewNodeToCollections(listToReturn, graph, node1);
        AddNewNodeToCollections(listToReturn, graph, node2);
        AddNewNodeToCollections(listToReturn, graph, node3);
        AddNewNodeToCollections(listToReturn, graph, node4);
    }

    private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

}