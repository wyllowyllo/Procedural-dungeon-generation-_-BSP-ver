using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public enum PUBLICSPACE { none, left_bottom, right_bottom, left_top, right_top, middle, plaza } //공용공간 생성 타입
public class DungeonCreator : MonoBehaviour
{
    public PUBLICSPACE type;
    public int dungeonWidth, dungeonHeight; //전체 그라운드 너비, 높이
    public int roomWidthMin, roomHeightMin; //최소 방 크기

    public int publicSpaceWidth, publicSpaceHeight; //공용공간 너비, 높이 (type이 none이면 적용되지 않음)
    public int publicSpaceDoorNumOnWall; //공용공간의 벽면당 문 개수 (벽면당 최소 1개)

    
    public int maxIterations; //최대 분할 횟수 - 트리 높이 제한, 이 값이 작을수록 비교적 큰 방이 만들어짐 (PublicSpace 타입이 none일 경우에만 적용)

    public int entranceSize; //문 너비
   
   
    public Material material; // For Visualizing

    List<RoomNode> listOfRooms=new List<RoomNode>();
    RoomNode Ground;


    private void Awake()
    {
        if (type == PUBLICSPACE.none)
            return;

        //최소 방 크기는 (전체 그라운드 크기 - 공용공간 크기) 보다 작아야 함
        if(roomWidthMin > (dungeonWidth - publicSpaceWidth))
        {           
            publicSpaceWidth = dungeonWidth;
            Debug.Log("방 최소너비가 (전체너비-공용공간 너비)보다 커서 publicSpaceWidth -> " + dungeonWidth + "으로 확장되었습니다");
        }
        if (roomHeightMin > (dungeonHeight - publicSpaceHeight))
        {          
            publicSpaceHeight = dungeonHeight;
            Debug.Log("방 최소높이 (전체높이-공용공간 높이)보다 커서 publicSpaceHeight -> " + dungeonHeight + "으로 확장되었습니다");
        }
    }
   
    void Start()
    {
        CreateDungeon();
        //CreateEntrance();
    }



    public void CreateDungeon()
    {
        List<RoomNode> rootList;
        switch (type)
        {
            case PUBLICSPACE.none:
            default:
               GenerateDungeon(new Vector2Int(0,0),dungeonWidth, dungeonHeight, type);
                break;

            case PUBLICSPACE.left_bottom:
            case PUBLICSPACE.right_bottom:
            case PUBLICSPACE.right_top:
            case PUBLICSPACE.left_top:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);
               
                //공용공간(0번 인덱스) 제외하고 분할 시작
                for (int i = 1; i < rootList.Count; i++)
                {
                    RoomNode rootNode = rootList[i];
                    GenerateDungeon(rootNode.BottomLeftAreaCorner,rootNode.Width, rootNode.Height, type);
                }

                Visualize();
                break;

           
               
            case PUBLICSPACE.middle:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);

                //공용공간(0번 인덱스) 제외하고 분할 시작
                for (int i = 1; i < rootList.Count; i++)
                {
                    RoomNode rootNode = rootList[i];
                    GenerateDungeon(rootNode.BottomLeftAreaCorner, rootNode.Width, rootNode.Height, type);
                }

                Visualize();
                break;
            case PUBLICSPACE.plaza:
                break;
        }
      

    }
    private void GenerateDungeon(Vector2Int startPoint, int totalWidth, int totalHeight, PUBLICSPACE type)
    {
        DungeonGenerator generator = new DungeonGenerator(totalWidth, totalHeight);
        List<RoomNode> leafList = generator.CalculateRooms(startPoint, maxIterations, roomWidthMin, roomHeightMin, type); //리프노드 리스트(실제 생성된 방 리스트)

      

        if(type==PUBLICSPACE.none)
        {
           
            Ground = generator.GetRootNode();  //전체 그라운드(루트 노드)
            listOfRooms = leafList;
            //바닥타일 깔고 구분선 만들기
            Visualize();
        }
        else
        {
            foreach(var leaf in leafList)
                listOfRooms.Add(leaf);
        }
       
    }
    
    private List<RoomNode> SplitTheSpace(PUBLICSPACE type)
    {
        //Vector2Int referencePoint; //기준점(왼쪽아래 방의 오른쪽 위 모서리 좌표 또는 왼쪽 아래 모서리 좌표)
        List<RoomNode> rootList=new List<RoomNode> ();


        if(type == PUBLICSPACE.left_bottom)
        {
            Vector2Int referencePoint = new Vector2Int(publicSpaceWidth, publicSpaceHeight); //기준점(왼쪽아래 방의 오른쪽 위 모서리 좌표)

            //공용공간
            RoomNode node1 = new RoomNode(new Vector2Int(0,0), referencePoint
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할
            if (dungeonWidth-publicSpaceWidth>0)
            {
                RoomNode node2 = new RoomNode(new Vector2Int(referencePoint.x, 0), new Vector2Int(dungeonWidth, dungeonHeight)
                               , null
                               , 0);
                rootList.Add(node2);
            }

            if (dungeonHeight - publicSpaceHeight > 0)
            {
                RoomNode node3 = new RoomNode(new Vector2Int(0, referencePoint.y), new Vector2Int(referencePoint.x, dungeonHeight)
                              , null
                              , 0);
                rootList.Add(node3);
            }
           
          
        }
        else if(type == PUBLICSPACE.right_bottom)
        {
            Vector2Int referencePoint = new Vector2Int(dungeonWidth-publicSpaceWidth, 0); //기준점(오른쪽아래 방의 왼쪽 아래 모서리 좌표)



            //공용공간
            RoomNode node1 = new RoomNode(referencePoint, new Vector2Int(referencePoint.x+publicSpaceWidth,referencePoint.y+publicSpaceHeight)
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할
            if (dungeonWidth - publicSpaceWidth > 0)
            {
                RoomNode node2 = new RoomNode(new Vector2Int(0, 0), new Vector2Int(referencePoint.x, dungeonHeight)
                              , null
                              , 0);
                rootList.Add(node2);
            }
            if (dungeonHeight - publicSpaceHeight > 0)
            {
                RoomNode node3 = new RoomNode(new Vector2Int(referencePoint.x, referencePoint.y + publicSpaceHeight), new Vector2Int(referencePoint.x + publicSpaceWidth, dungeonHeight)
                              , null
                              , 0);
                rootList.Add(node3);
            }

          
        }
        else if (type == PUBLICSPACE.right_top)
        {
            Vector2Int referencePoint = new Vector2Int(dungeonWidth-publicSpaceWidth, dungeonHeight-publicSpaceHeight); //기준점(오른쪽 위 방의 왼쪽아래 모서리 좌표)

            //공용공간
            RoomNode node1 = new RoomNode(referencePoint, new Vector2Int(referencePoint.x+publicSpaceWidth, referencePoint.y + publicSpaceHeight)
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할
            if (dungeonWidth - publicSpaceWidth > 0)
            {
                RoomNode node2 = new RoomNode(new Vector2Int(0, 0), new Vector2Int(referencePoint.x, dungeonHeight)
                              , null
                              , 0);
                rootList.Add(node2);
            }
               
            if (dungeonHeight - publicSpaceHeight > 0)
            {
                RoomNode node3 = new RoomNode(new Vector2Int(referencePoint.x, 0), new Vector2Int(dungeonWidth, referencePoint.y)
                                , null
                                , 0);
                rootList.Add(node3);
            }
                
        }
        else if (type == PUBLICSPACE.left_top)
        {
            Vector2Int referencePoint = new Vector2Int(publicSpaceWidth, dungeonHeight); //기준점(왼쪽 위 방의 오른쪽 위 모서리 좌표)

            //공용공간
            RoomNode node1 = new RoomNode(new Vector2Int(0,referencePoint.y-publicSpaceHeight), referencePoint
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할
            if (dungeonWidth - publicSpaceWidth > 0)
            {
                RoomNode node2 = new RoomNode(new Vector2Int(referencePoint.x, 0), new Vector2Int(dungeonWidth, dungeonHeight)
                               , null
                               , 0);
                rootList.Add(node2);
            }
                
            if (dungeonHeight - publicSpaceHeight > 0)
            {
                RoomNode node3 = new RoomNode(new Vector2Int(0, 0), new Vector2Int(referencePoint.x, referencePoint.y - publicSpaceHeight)
                                , null
                                , 0);
                rootList.Add(node3);
            }       
        }
        else if (type == PUBLICSPACE.middle)
        {
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //그라운드 정중앙
            Vector2Int leftBottomPoint=new Vector2Int(center.x-(publicSpaceWidth/2), center.y-(publicSpaceHeight/2));
            Vector2Int rightTopPoint = new Vector2Int(center.x + (publicSpaceWidth / 2), center.y + (publicSpaceHeight / 2));

            //공용공간
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할
            if (leftBottomPoint.y > 0)
            {
                RoomNode node2 = new RoomNode(new Vector2Int(leftBottomPoint.x, 0), new Vector2Int(rightTopPoint.x, leftBottomPoint.y)
                             , null
                             , 0);
                rootList.Add(node2);
            }
            if (dungeonWidth - rightTopPoint.x > 0)
            {
                RoomNode node3 = new RoomNode(new Vector2Int(rightTopPoint.x, 0), new Vector2Int(dungeonWidth, dungeonHeight)
                             , null
                             , 0);
                rootList.Add(node3);
            }
            if (dungeonHeight - rightTopPoint.y > 0)
            {
                RoomNode node4 = new RoomNode(new Vector2Int(leftBottomPoint.x, rightTopPoint.y), new Vector2Int(rightTopPoint.x, dungeonHeight)
                             , null
                             , 0);
                rootList.Add(node4);
            }
            if (leftBottomPoint.x > 0)
            {
                RoomNode node5 = new RoomNode(new Vector2Int(0, 0), new Vector2Int(leftBottomPoint.x, dungeonHeight)
                             , null
                             , 0);
                rootList.Add(node5);
            }

        }
        else if (type == PUBLICSPACE.plaza)
        {

        }



        return rootList;
    }

    void Visualize()
    {
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
    }

    void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV=new Vector3(topRightCorner.x,0,bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[] //생성할 mesh의 각 점. 사각형이면 4개 정점 필요
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
        };

        Vector2[] uvs = new Vector2[vertices.Length]; //텍스쳐의 어떤 부분이 각 Vertex에 적용되는지를 나타냄. 사각형이므로 하나의  uv 정점이 4개 있음
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        int[] triangles = new int[] //순서 중요 -> 카메라를 향해서 시계 방향으로
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh=new Mesh();
        mesh.vertices=vertices;
        mesh.uv=uvs;
        mesh.triangles=triangles;

        int width = (int)(topRightCorner.x - bottomLeftCorner.x);
        int height=(int)(topRightCorner.y-bottomLeftCorner.y);
        GameObject dungeonFloor = new GameObject("Mesh" + "("+width+", "+height+")", typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position= Vector3.zero;
        dungeonFloor.transform.localScale= Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        //OutLine
        LineRenderer line=dungeonFloor.AddComponent<LineRenderer>();
        line.positionCount = 5;
        line.enabled = false;

        line.SetPosition(0, vertices[0]);
        line.SetPosition(1, vertices[1]);
        line.SetPosition(2, vertices[3]);
        line.SetPosition(3, vertices[2]);
        line.SetPosition(4, vertices[0]);


        line.enabled = true;

    }

    void CreateEntrance()
    {
        List<Vector2Int> roomVertexes = new List<Vector2Int>();

        //모든 방 꼭짓점 리스트 만들기
        foreach (var room in listOfRooms)
        {
            roomVertexes.Add(room.BottomLeftAreaCorner);
            roomVertexes.Add(room.BottomRightAreaCorner);
            roomVertexes.Add(room.TopLeftAreaCorner);
            roomVertexes.Add(room.TopRightAreaCorner);
        }

        RoomNode currentNode;
      
        foreach (var room in listOfRooms)
        {
            if (room.Parent == null)
                continue;

            currentNode = room.Parent;

            while(currentNode.Parent!=null)
            {
                GenerateEntrance(currentNode, roomVertexes);
                currentNode=currentNode.Parent;
            }

         
        }

        //공용공간이 있을 경우 공용공간에 통로 따로 생성
        if(type!=PUBLICSPACE.none)
        {
            RoomNode publicSpaceNode=listOfRooms[0];
           
            switch(type)
            {
                //왼쪽 아래 코너에 공용공간이 있을 경우, 해당 공간의 오른쪽, 위쪽 벽면에 통로 생성
                case PUBLICSPACE.left_bottom:

                    while(publicSpaceNode.WallList[1].DoorNum()<publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[1], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //오른쪽 아래 코너에 공용공간이 있을 경우, 해당 공간의 왼쪽, 위쪽 벽면에 통로 생성
                case PUBLICSPACE.right_bottom:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //오른쪽 위 코너에 공용공간이 있을 경우, 해당 공간의 왼쪽, 아래쪽 벽면에 통로 생성
                case PUBLICSPACE.right_top:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[2], roomVertexes);
                    break;

                //왼쪽 위 코너에 공용공간이 있을 경우, 해당 공간의 아래쪽, 오른쪽 벽면에 통로 생성
                case PUBLICSPACE.left_top:
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[2], roomVertexes);
                    while (publicSpaceNode.WallList[1].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[1], roomVertexes);
                    break;
            }
        }
    }

    void GenerateEntrance(RoomNode room, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate=Vector2Int.zero;
        Wall line = room.GetDivideLine();

        //각 분할선마다 1개의 출입구 만들기
        if (line.DoorNum() !=0)
            return;

            if (line.Orientation == Orientation.Horizontal)
            {
          
                bool overlap=true;
                while (overlap)
                {
                    doorCoordinate = new Vector2Int(Random.Range(line.LeftVertex.x, line.RightVertex.x - entranceSize), line.LeftVertex.y);

                    foreach (var roomVertex in roomVertexes)
                    {
                        overlap = false;

                        //문의 x좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                        if(IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal)) 
                        {
                            overlap = true;
                            break;
                        }
                        else
                        {
                            foreach(var doorPos in room.GetDivideLine().GetDoorList())
                            {
                                //문이 겹치지 않도록 생성
                                if(IsDoorOverlap(doorCoordinate, doorPos, Orientation.Horizontal))
                                {
                                    overlap = true;
                                    break;
                                }

                            }
                        }
                    }
                }
        }
        else
            {
      
                bool overlap = true;
                while (overlap)
                {
                    doorCoordinate = new Vector2Int(line.LeftVertex.x, Random.Range(line.RightVertex.y, line.LeftVertex.y - entranceSize));

                    foreach (var roomVertex in roomVertexes)
                    {
                        overlap = false;

                        //문의 y좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                        if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical)) 
                        {
                            overlap = true;
                            break;
                        }
                        else
                        {
                            foreach (var doorPos in room.GetDivideLine().GetDoorList())
                            {
                                //문이 겹치지 않도록 생성
                                if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Vertical))
                                {
                                    overlap = true;
                                    break;
                                }

                            }
                        }
                    }
                }
        }

            line.AddDoor(doorCoordinate);

            //구분선 그리기
            DrawLine(line, doorCoordinate);
        }
    void GenerateEntrance(Wall wall, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate = Vector2Int.zero;
       

       

        if (wall.Orientation == Orientation.Horizontal)
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x - entranceSize), wall.LeftVertex.y);

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 x좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Horizontal))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }
        else
        {

            bool overlap = true;
            while (overlap)
            {
                doorCoordinate = new Vector2Int(wall.LeftVertex.x, Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));

                foreach (var roomVertex in roomVertexes)
                {
                    overlap = false;

                    //문의 y좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우 재생성
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //문이 겹치지 않도록 생성
                            if (IsDoorOverlap(doorCoordinate, doorPos, Orientation.Vertical))
                            {
                                overlap = true;
                                break;
                            }

                        }
                    }
                }
            }
        }

        wall.AddDoor(doorCoordinate);

        //구분선 그리기
        DrawLine(wall, doorCoordinate);
    }

    void DrawLine(Wall line, Vector2Int doorCoordinate)
    {
        //OutLine
        GameObject door = new GameObject("Door");

        door.transform.position = Vector3.zero;
        door.transform.localScale = Vector3.one;


        LineRenderer lineDoor = door.AddComponent<LineRenderer>();
        lineDoor.positionCount = 2;
        lineDoor.startColor = Color.green;
        lineDoor.endColor = Color.green;
        lineDoor.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineDoor.sortingOrder = 1;

        lineDoor.enabled = false;

        if (line.Orientation == Orientation.Horizontal)
        {
            lineDoor.SetPosition(0, new Vector3(doorCoordinate.x, 2, doorCoordinate.y));
            lineDoor.SetPosition(1, new Vector3(doorCoordinate.x + entranceSize, 2, doorCoordinate.y));
        }
        else
        {
            lineDoor.SetPosition(0, new Vector3(doorCoordinate.x, 2, doorCoordinate.y));
            lineDoor.SetPosition(1, new Vector3(doorCoordinate.x, 2, doorCoordinate.y + entranceSize));
        }


        lineDoor.enabled = true;
    }
    bool IsDoorOverlap(Vector2Int newDoor, Vector2Int otherDoor, Orientation ori)
    {
        bool overlap = false;

        if(ori == Orientation.Horizontal)
        {
            if ((newDoor.x <= otherDoor.x && newDoor.y == otherDoor.y) && (otherDoor.x <= newDoor.x + entranceSize && newDoor.y == otherDoor.y))
                overlap = true;
            else if ((newDoor.x >= otherDoor.x && newDoor.y == otherDoor.y) && (newDoor.x <= otherDoor.x + entranceSize && newDoor.y == otherDoor.y))
                overlap = true;
        }
        else
        {
            if ((newDoor.y <= otherDoor.y && newDoor.x == otherDoor.x) && (otherDoor.y <= newDoor.y + entranceSize && newDoor.x == otherDoor.x))
                overlap = true;
            else if ((newDoor.y >= otherDoor.y && newDoor.x == otherDoor.x) && (newDoor.y <= otherDoor.y + entranceSize && newDoor.x == otherDoor.x))
                overlap = true;
        }

        return overlap;
    }

    bool IsOnVertex(Vector2Int newDoor, Vector2Int vertex, Orientation ori)
    {
        bool onVertex = false;
        if(ori== Orientation.Horizontal)
        {
            if((newDoor.x <= vertex.x && newDoor.y == vertex.y) && (vertex.x <= newDoor.x + entranceSize && newDoor.y == vertex.y))
                    onVertex = true;
        }
        else
        {
            if ((newDoor.y <= vertex.y && newDoor.x == vertex.x) && (vertex.y <= newDoor.y + entranceSize && newDoor.x == vertex.x))
                onVertex = true;
        }

        return onVertex;
    }


}
