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
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode); //반환값은 리프노드 전체

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomWidthMin, roomHeightMin); //이전까지는 방의 구획을 나눴으니, 실제로 방 생성하는 코드
        List<RoomNode> roomList=roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces); //리프노드 리스트 전달하여 방 생성하기
        return new List<Node>(roomList);
    }
}