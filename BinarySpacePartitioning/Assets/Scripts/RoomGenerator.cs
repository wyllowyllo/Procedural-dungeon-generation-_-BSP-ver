using System;
using System.Collections.Generic;
using UnityEngine;

internal class RoomGenerator
{
    private int maxIterations;
    private int roomWidthMin;
    private int roomHeightMin;

    public RoomGenerator(int maxIterations, int roomWidthMin, int roomHeightMin)
    {
        this.maxIterations = maxIterations;
        this.roomWidthMin = roomWidthMin;
        this.roomHeightMin = roomHeightMin;
    }

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces)
    {
        List<RoomNode> listToReturn= new List<RoomNode>();
        foreach(var space in roomSpaces)
        {
            /*Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween( //생성할 방의 왼쪽 아래 좌표 
                space.BottomLeftAreaCorner,space.TopRightAreaCorner,0.1f,1);*/
            Vector2Int newBottomLeftPoint = space.BottomLeftAreaCorner;

            /*Vector2Int newTopRightPoint=StructureHelper.GenerateTopRightCornerBetween( //생성할 방의 오른쪽 위 좌표 
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.9f, 1);*/
            Vector2Int newTopRightPoint= space.TopRightAreaCorner;

            space.BottomLeftAreaCorner= newBottomLeftPoint;
            space.TopRightAreaCorner= newTopRightPoint;
            space.BottomRightAreaCorner= new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner=new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            listToReturn.Add((RoomNode)space);
        }
        return listToReturn;
    }
}