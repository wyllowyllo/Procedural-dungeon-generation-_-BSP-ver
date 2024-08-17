using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using System;

public enum PUBLICSPACE { none, left_bottom, right_bottom, left_top, right_top, center, plaza } //공용공간 생성 타입
public class DungeonCreator : MonoBehaviour
{
    [Header("생성 타입")] public PUBLICSPACE type;

    [Header("그라운드 크기")]
    public int dungeonWidth;
    public int dungeonHeight;

    [Header("최소 방 크기")]
    [Tooltip("크기는 10보다 커야하며, dungeonWidth를 초과할 수 없습니다.")]
    public int roomWidthMin;
    [Tooltip("크기는 10보다 커야하며, dungeonHeight를 초과할 수 없습니다.")]
    public int roomHeightMin;

    [Header("공용공간 크기")]
    [Tooltip("크기는 10보다 커야하며,dungeonWidth를 초과할 수 없습니다.")]
    public int publicSpaceWidth; 
    [Tooltip("크기는 10보다 커야하며,dungeonHeight를 초과할 수 없습니다.")]
    public int publicSpaceHeight;

    [Header("공용공간 벽면당 문 개수")]
    public int publicSpaceDoorNumOnWall; 

    [Header("Plaza 타입 반지름")]
    [Tooltip("최대길이 = (그라운드 너비, 높이 중 작은 값의 절반)")]
    public int plazaRadius; //plaza 타입 공용공간 반지름 길이

    [Header("Plaza 타입 각 개수")]
    [Range(3,20)]
    public int polygon; //정n각형, 값은 최소 3

    [Header("None 타입 최대분할 횟수")]
    public int maxIterations; //최대 분할 횟수 - 트리 높이 제한, 이 값이 작을수록 비교적 큰 방이 만들어짐 (PublicSpace 타입이 none일 경우에만 적용)

    [Header("etc")]
    [Tooltip("문의 크기는 벽의 크기보다 클 수 없습니다.")]
    public int entranceSize; //문 너비
   
   
    public Material material; // For Visualizing
    public GameObject wallVertical, wallHorizontal;

    List<Vector2Int> WallHorizontalPos; //가로 벽 좌표값 전체
    List<Vector2Int> WallVerticalPos; // 세로 벽 좌표값 전체
    List<Vector2Int> EntranceHorizontalCandidate; //방문 생성 가능한 가로 벽 좌표(방과 방이 공유하는 선)
    List<Vector2Int> EntranceVerticalCandidate;//방문 생성 가능한 세로 벽 좌표(방과 방이 공유하는 선)

    List<RoomNode> listOfRooms=new List<RoomNode>();
    RoomNode Ground;


    private void Awake()
    {
        if (type == PUBLICSPACE.none)
            return;

        if (type == PUBLICSPACE.plaza)
        {
            //최소 방 크기는 (전체 그라운드 크기 - 광장 크기) 보다 작아야 함
            if (roomWidthMin > (dungeonWidth - plazaRadius*2))
            {
                if(dungeonWidth>dungeonHeight)
                    plazaRadius = dungeonHeight / 2;
                else
                    plazaRadius = dungeonWidth / 2;
                Debug.Log("방 최소너비 또는 최소높이가 (전체너비-광장 크기)보다 커서 plazaRadius -> " + plazaRadius + "으로 확장되었습니다");
            }
        }
        else
        {
            //최소 방 크기는 (전체 그라운드 크기 - 공용공간 크기) 보다 작아야 함
            if (roomWidthMin > (dungeonWidth - publicSpaceWidth))
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
    }
   
    void Start()
    {
        CreateDungeon();
        CreateEntrance();
        //VisualizeWall();
    }

    private void VisualizeWall()
    {
       

        //생성
        /* foreach (var wallPosition in WallHorizontalPos)
         {

             Instantiate(wallHorizontal, wallPosition, Quaternion.identity);
         }

         foreach (var wallPosition in WallVerticalPos)
         {

             Instantiate(wallVertical, wallPosition, Quaternion.identity);
         }*/

    }

    public void CreateDungeon()
    {
        List<RoomNode> rootList;
        switch (type)
        {
            case PUBLICSPACE.none:
            default:
               GenerateDungeon(new Vector2Int(0,0),dungeonWidth, dungeonHeight, type);
                Visualize(); //바닥타일 깔고 구분선 만들기
                break;

            case PUBLICSPACE.left_bottom:
            case PUBLICSPACE.right_bottom:
            case PUBLICSPACE.right_top:
            case PUBLICSPACE.left_top:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);
                listOfRooms[0].roomName = "LivingRoom";

                //공용공간(0번 인덱스) 제외하고 분할 시작
                for (int i = 1; i < rootList.Count; i++)
                {
                    RoomNode rootNode = rootList[i];
                    GenerateDungeon(rootNode.BottomLeftAreaCorner,rootNode.Width, rootNode.Height, type);
                }

                Visualize();
                break;

           
            case PUBLICSPACE.center:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);
                listOfRooms[0].roomName = "CenterRoom";

                //공용공간(0번 인덱스) 제외하고 분할 시작
                for (int i = 1; i < rootList.Count; i++)
                {
                    RoomNode rootNode = rootList[i];
                    GenerateDungeon(rootNode.BottomLeftAreaCorner, rootNode.Width, rootNode.Height, type);
                }

                Visualize();
                break;
            case PUBLICSPACE.plaza:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);
                listOfRooms[0].roomName = "Plaza";

                //공용공간(0번 인덱스) 제외하고 분할 시작
                for (int i = 1; i < rootList.Count; i++)
                {
                    RoomNode rootNode = rootList[i];
                    GenerateDungeon(rootNode.BottomLeftAreaCorner, rootNode.Width, rootNode.Height, type);
                }

                VisualizeForPlaza();
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
        else if (type == PUBLICSPACE.center)
        {
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //그라운드 정중앙
            Vector2Int leftBottomPoint=new Vector2Int(center.x-(publicSpaceWidth/2), center.y-(publicSpaceHeight/2));
            Vector2Int rightTopPoint = new Vector2Int(center.x + (publicSpaceWidth / 2), center.y + (publicSpaceHeight / 2));

            //공용공간
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할 (아래, 오른쪽, 위, 왼쪽 순서)
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
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //그라운드 정중앙

            //plaza를 내접원으로 가지는 사각형이 있다고 가정하고 그 사각형 기준으로 분할
            Vector2Int leftBottomPoint = new Vector2Int(center.x - plazaRadius, center.y - plazaRadius);
            Vector2Int rightTopPoint = new Vector2Int(center.x + plazaRadius, center.y + plazaRadius);

            //공용공간
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //나머지 공간 분할 (아래, 오른쪽, 위, 왼쪽 순서)
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



        return rootList;
    }

    void Visualize()
    {
        SetRoomName();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, listOfRooms[i].roomName);
        }
    }
    private void VisualizeForPlaza()
    {
        SetRoomName();

        //정N각형 좌표(중점을 기준으로 좌표 원형배치)
        Vector3[] vertices = new Vector3[polygon + 1];

        vertices[0] = new Vector3(dungeonWidth/2, 0, dungeonHeight / 2) ;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;
            vertices[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * plazaRadius + vertices[0];
        }

        //정N각형을 이루는 삼각형들 좌표
        int[] triangles = new int[3 * polygon];
        for (int i = 0; i < polygon - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        triangles[3 * polygon - 3] = 0;
        triangles[3 * polygon - 2] = polygon;
        triangles[3 * polygon - 1] = 1;
        
        //정N각형 메쉬생성
        CreateMeshForPlaza(vertices, triangles);


        // 공용공간 외 나머지 방들
        for (int i = 1; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, listOfRooms[i].roomName);
        }
    }

    void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner, string roomName)
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
        GameObject dungeonFloor = new GameObject(roomName + "("+width+", "+height+")", typeof(MeshFilter), typeof(MeshRenderer));

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
    void CreateWallPosList(RoomNode roomNode)
    {
        Vector2Int bottomLeftV = roomNode.BottomLeftAreaCorner;
        Vector2Int bottomRightV = roomNode.BottomRightAreaCorner;
        Vector2Int topLeftV = roomNode.TopLeftAreaCorner;
        Vector2Int topRightV = roomNode.TopRightAreaCorner;
      

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
         {
             var wallPosition = new Vector2Int(row, bottomLeftV.y);
             AddWallPosToList(wallPosition, WallHorizontalPos, EntranceHorizontalCandidate);
         }
         for (int row = (int)topLeftV.x; row < (int)topRightV.x; row++)
         {
             var wallPosition = new Vector2Int(row, topLeftV.y);
            AddWallPosToList(wallPosition, WallHorizontalPos, EntranceHorizontalCandidate);
         }
         for (int col = (int)bottomLeftV.y; col < (int)topLeftV.y; col++)
         {
             var wallPosition = new Vector2Int(bottomLeftV.x, col);
            AddWallPosToList(wallPosition, WallVerticalPos,EntranceVerticalCandidate);
         }
         for (int col = (int)bottomRightV.y; col < (int)topRightV.y; col++)
         {
            var wallPosition = new Vector2Int(bottomRightV.x, col);
            AddWallPosToList(wallPosition, WallVerticalPos, EntranceVerticalCandidate);
         }

    }
    private void AddWallPosToList(Vector2Int wallPosition, List<Vector2Int> wallList, List<Vector2Int> doorCandidnate)
    {
        Vector2Int point=wallPosition;

        if (wallList.Contains(point))
        {
            wallList.Remove(point);
            doorCandidnate.Add(point);
        }
        else
        {
            wallList.Add(point);
        }
    }


    void CreateMeshForPlaza(Vector3[] vertices, int[] triangles)
    {
        Mesh mesh= new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.RecalculateNormals();

        GameObject plazaFloor = new GameObject("Plaza" , typeof(MeshFilter), typeof(MeshRenderer));

        plazaFloor.transform.position = Vector3.zero;
        plazaFloor.transform.localScale = Vector3.one;
        plazaFloor.GetComponent<MeshFilter>().mesh = mesh;
        plazaFloor.GetComponent<MeshRenderer>().material = material;

        //OutLine
        LineRenderer line = plazaFloor.AddComponent<LineRenderer>();
        line.positionCount = polygon+1;
        line.enabled = false;

        for (int i = 1; i <= polygon; i++)
        {
            line.SetPosition(i-1, vertices[i]);
    
        }
        line.SetPosition(polygon, vertices[1]);


        line.enabled = true;


    }

    void CreateEntrance()
    {
        WallHorizontalPos = new List<Vector2Int>();
        WallVerticalPos = new List<Vector2Int>();
        EntranceHorizontalCandidate = new List<Vector2Int>();
        EntranceVerticalCandidate = new List<Vector2Int>();

        //문 생성할 위치 좌표 리스트 만들기
        foreach (var room in listOfRooms)
            CreateWallPosList(room);

      
      

        EntranceGenerator entranceGenerator=new EntranceGenerator(listOfRooms,type, publicSpaceDoorNumOnWall, entranceSize);
        entranceGenerator.SetEntrancePossibleList(EntranceHorizontalCandidate, EntranceVerticalCandidate);
        entranceGenerator.GenerateEntrance();
    }

    void SetRoomName()
    {
        int idx = 0;
        foreach(var room in listOfRooms)
        {
            if (room.roomName == null)
            {
                room.roomName = "room" + idx++;
            }
        }
    }
    private void OnValidate() //인스펙터 상에서 변수 범위 제한
    {
        roomWidthMin = Mathf.Clamp(roomWidthMin, 10, dungeonWidth);
        roomHeightMin = Mathf.Clamp(roomHeightMin, 10, dungeonHeight);

        publicSpaceWidth= Mathf.Clamp(publicSpaceWidth, roomWidthMin, dungeonWidth);
        publicSpaceHeight = Mathf.Clamp(publicSpaceHeight, roomHeightMin, dungeonWidth);
        publicSpaceDoorNumOnWall = (publicSpaceDoorNumOnWall < 1) ? 1 : publicSpaceDoorNumOnWall;
        
        entranceSize = (entranceSize >= roomWidthMin) ? 5 : entranceSize;

        plazaRadius = (dungeonWidth > dungeonHeight) ? Mathf.Clamp(plazaRadius, 5, dungeonHeight / 2) : Mathf.Clamp(plazaRadius, 5, dungeonWidth / 2);
    }


}
