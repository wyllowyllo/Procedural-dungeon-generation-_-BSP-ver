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

public enum UNITSIZE { SIZE_5=5, SIZE_10 = 10}
public enum PUBLICSPACE { none, left_bottom, right_bottom, left_top, right_top, center, plaza } //������� ���� Ÿ��
public class DungeonCreator : MonoBehaviour
{
    [Header("���� Ÿ��")] public PUBLICSPACE type;

    
    public UNITSIZE unitSize;//���� ����

    [Header("�׶��� ũ��")]
    [Tooltip("������ unitSize�Դϴ�.")]
    public int dungeonWidth;
    public int dungeonHeight;

    [Header("�ּ� �� ũ��")]
    [Tooltip("������ unitSize�̸�, dungeonWidth�� �ʰ��� �� �����ϴ�.")]
    public int roomWidthMin;
    [Tooltip("������ unitSize�̸�, dungeonHeight�� �ʰ��� �� �����ϴ�.")]
    public int roomHeightMin;

    [Header("������� ũ��")]
    [Tooltip("������ unitSize�̸�, dungeonWidth�� �ʰ��� �� �����ϴ�.")]
    public int publicSpaceWidth; 
    [Tooltip("������ unitSize�̸�, dungeonHeight�� �ʰ��� �� �����ϴ�.")]
    public int publicSpaceHeight;

   

    [Header("Plaza Ÿ�� ������")]
    [Tooltip("�ִ���� = (�׶��� �ʺ�, ���� �� ���� ���� ����)")]
    public int plazaRadius; //plaza Ÿ�� ������� ������ ����

    [Header("Plaza Ÿ�� �� ����")]
    [Range(3,20)]
    public int polygon; //��n����, ���� �ּ� 3

    [Header("None Ÿ�� �ִ���� Ƚ��")]
    public int maxIterations; //�ִ� ���� Ƚ�� - Ʈ�� ���� ����, �� ���� �������� ���� ū ���� ������� (PublicSpace Ÿ���� none�� ��쿡�� ����)

    [Header("etc")]
    public Material material; // For Visualizing
    public GameObject wallVertical, wallHorizontal;

    
    List<Vector2Int> WallHorizontalPos; //���� �� ��ǥ�� ��ü
    List<Vector2Int> WallVerticalPos; // ���� �� ��ǥ�� ��ü
    List<Vector2Int> EntranceHorizontalCandidate; //�湮 ���� ������ ���� �� ��ǥ(��� ���� �����ϴ� ��)
    List<Vector2Int> EntranceVerticalCandidate;//�湮 ���� ������ ���� �� ��ǥ(��� ���� �����ϴ� ��)
    List<Vector2Int> InnerWallHorizontalPos;
    List<Vector2Int> InnerWallVerticalPos;

    List<RoomNode> listOfRooms=new List<RoomNode>();
    RoomNode Ground;


    private void Awake()
    {
        

        if (type == PUBLICSPACE.none)
            return;

        if (type == PUBLICSPACE.plaza)
        {
            //������� ���Ʒ�/�翷���� �ּҰ��� Ȯ���ϱ� ����.
            if (roomWidthMin*2 > (dungeonWidth - plazaRadius*2) || roomHeightMin * 2 > (dungeonHeight - plazaRadius * 2))
            {
      
                if ((dungeonWidth - roomWidthMin * 2) > (dungeonHeight - roomHeightMin * 2))
                    plazaRadius = (dungeonHeight - roomHeightMin * 2)/2;
                else
                    plazaRadius = (dungeonWidth - roomWidthMin * 2)/2;

                Debug.Log("plazaRadius -> " + plazaRadius + "���� �����Ǿ����ϴ�");
            }
        }
        else if (type == PUBLICSPACE.center)
        {
            if (roomWidthMin*2 > (dungeonWidth - publicSpaceWidth))
            {
                publicSpaceWidth = dungeonWidth- roomWidthMin * 2;
                Debug.Log("������� �ʺ� = "+publicSpaceWidth+"�� �����Ǿ����ϴ�.");
            }
            if (roomHeightMin*2 > (dungeonHeight - publicSpaceHeight))
            {
                publicSpaceHeight = dungeonHeight- roomHeightMin * 2;
                Debug.Log("������� ���� = " + publicSpaceHeight + "�� �����Ǿ����ϴ�.");
            }
        }
        else
        {
            //�ּ� �� ũ��� (��ü �׶��� ũ�� - ������� ũ��) ���� �۾ƾ� ��
            if (roomWidthMin > (dungeonWidth - publicSpaceWidth))
            {
                publicSpaceWidth = dungeonWidth;
                Debug.Log("�� �ּҳʺ� (��ü�ʺ�-������� �ʺ�)���� Ŀ�� publicSpaceWidth -> " + dungeonWidth + "���� Ȯ��Ǿ����ϴ�");
            }
            if (roomHeightMin > (dungeonHeight - publicSpaceHeight))
            {
                publicSpaceHeight = dungeonHeight;
                Debug.Log("�� �ּҳ��� (��ü����-������� ����)���� Ŀ�� publicSpaceHeight -> " + dungeonHeight + "���� Ȯ��Ǿ����ϴ�");
            }
        }
    }
   
    void Start()
    {
        CreateDungeon();
        CreateEntrance();
        VisualizeWall();
    }

   
    public void CreateDungeon()
    {
        List<RoomNode> rootList;
        switch (type)
        {
            case PUBLICSPACE.none:
            default:
               GenerateDungeon(new Vector2Int(0,0),dungeonWidth, dungeonHeight, type);
                Visualize(); //�ٴ�Ÿ�� ��� ���м� �����
                break;

            case PUBLICSPACE.left_bottom:
            case PUBLICSPACE.right_bottom:
            case PUBLICSPACE.right_top:
            case PUBLICSPACE.left_top:
                rootList = SplitTheSpace(type);
                listOfRooms.Add(rootList[0]);
                listOfRooms[0].roomName = "LivingRoom";
                listOfRooms[0].SetWall((int)unitSize);
                listOfRooms[0].SetGrid((int)unitSize);

                //�������(0�� �ε���) �����ϰ� ���� ����
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
                listOfRooms[0].SetWall((int)unitSize);
                listOfRooms[0].SetGrid((int)unitSize);

                //�������(0�� �ε���) �����ϰ� ���� ����
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

                //�������(0�� �ε���) �����ϰ� ���� ����
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
        DungeonGenerator generator = new DungeonGenerator(totalWidth, totalHeight, (int)unitSize);
        List<RoomNode> leafList = generator.CalculateRooms(startPoint, maxIterations, roomWidthMin, roomHeightMin, type); //������� ����Ʈ(���� ������ �� ����Ʈ)

      

        if(type==PUBLICSPACE.none)
        {
           
            Ground = generator.GetRootNode();  //��ü �׶���(��Ʈ ���)
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
        //Vector2Int referencePoint; //������(���ʾƷ� ���� ������ �� �𼭸� ��ǥ �Ǵ� ���� �Ʒ� �𼭸� ��ǥ)
        List<RoomNode> rootList=new List<RoomNode> ();


        if(type == PUBLICSPACE.left_bottom)
        {
            Vector2Int referencePoint = new Vector2Int(publicSpaceWidth, publicSpaceHeight); //������(���ʾƷ� ���� ������ �� �𼭸� ��ǥ)

            //�������
            RoomNode node1 = new RoomNode(new Vector2Int(0,0), referencePoint
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ����
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
            Vector2Int referencePoint = new Vector2Int(dungeonWidth-publicSpaceWidth, 0); //������(�����ʾƷ� ���� ���� �Ʒ� �𼭸� ��ǥ)



            //�������
            RoomNode node1 = new RoomNode(referencePoint, new Vector2Int(referencePoint.x+publicSpaceWidth,referencePoint.y+publicSpaceHeight)
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ����
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
            Vector2Int referencePoint = new Vector2Int(dungeonWidth-publicSpaceWidth, dungeonHeight-publicSpaceHeight); //������(������ �� ���� ���ʾƷ� �𼭸� ��ǥ)

            //�������
            RoomNode node1 = new RoomNode(referencePoint, new Vector2Int(referencePoint.x+publicSpaceWidth, referencePoint.y + publicSpaceHeight)
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ����
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
            Vector2Int referencePoint = new Vector2Int(publicSpaceWidth, dungeonHeight); //������(���� �� ���� ������ �� �𼭸� ��ǥ)

            //�������
            RoomNode node1 = new RoomNode(new Vector2Int(0,referencePoint.y-publicSpaceHeight), referencePoint
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ����
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
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //�׶��� ���߾�
            Vector2Int leftBottomPoint=new Vector2Int(center.x-(publicSpaceWidth/2), center.y-(publicSpaceHeight/2));
            Vector2Int rightTopPoint = new Vector2Int(center.x + (publicSpaceWidth / 2), center.y + (publicSpaceHeight / 2));
 
            //�������
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ���� (�Ʒ�, ������, ��, ���� ����)
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
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //�׶��� ���߾�

            //plaza�� ���������� ������ �簢���� �ִٰ� �����ϰ� �� �簢�� �������� ����
            Vector2Int leftBottomPoint = new Vector2Int(center.x - plazaRadius, center.y - plazaRadius);
            Vector2Int rightTopPoint = new Vector2Int(center.x + plazaRadius, center.y + plazaRadius);

            //�������
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ���� (�Ʒ�, ������, ��, ���� ����)
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

        //��N���� ��ǥ(������ �������� ��ǥ ������ġ)
        Vector3[] vertices = new Vector3[polygon + 1];

        vertices[0] = new Vector3(dungeonWidth/2, 0, dungeonHeight / 2) ;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;
            vertices[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * plazaRadius + vertices[0];
        }

        //��N������ �̷�� �ﰢ���� ��ǥ
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
        
        //��N���� �޽�����
        CreateMeshForPlaza(vertices, triangles);


        // ������� �� ������ ���
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

        Vector3[] vertices = new Vector3[] //������ mesh�� �� ��. �簢���̸� 4�� ���� �ʿ�
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
        };

        Vector2[] uvs = new Vector2[vertices.Length]; //�ؽ����� � �κ��� �� Vertex�� ����Ǵ����� ��Ÿ��. �簢���̹Ƿ� �ϳ���  uv ������ 4�� ����
        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        int[] triangles = new int[] //���� �߿� -> ī�޶� ���ؼ� �ð� ��������
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



    }
    void CreateWallPosList(RoomNode roomNode)
    {
       
        List<Wall> wallList=roomNode.WallList; //���� ��-������ ��-�Ʒ� ��-�� �� ����

        foreach(var point in wallList[0].GetWallObjPoints())
            AddWallPosToList(point, WallVerticalPos, EntranceVerticalCandidate);
        foreach (var point in wallList[1].GetWallObjPoints())
            AddWallPosToList(point, WallVerticalPos, EntranceVerticalCandidate);
        foreach (var point in wallList[2].GetWallObjPoints())
            AddWallPosToList(point, WallHorizontalPos, EntranceHorizontalCandidate);
        foreach (var point in wallList[3].GetWallObjPoints())
            AddWallPosToList(point, WallHorizontalPos, EntranceHorizontalCandidate);
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

        //�� ������ ��ġ ��ǥ ����Ʈ �����
        foreach (var room in listOfRooms)
            CreateWallPosList(room);

        //�ܺ� ������ ����
        // ���� ���� (Deep Copy)
        InnerWallHorizontalPos = new List<Vector2Int>(EntranceHorizontalCandidate);
        InnerWallVerticalPos = new List<Vector2Int>(EntranceVerticalCandidate);


        EntranceGenerator entranceGenerator =new EntranceGenerator(listOfRooms,type, (int)unitSize);
        entranceGenerator.SetEntrancePossibleList(EntranceHorizontalCandidate, EntranceVerticalCandidate);
        entranceGenerator.GenerateEntrance();

        //��� ������ ��ġ�� �� ����
        List<Vector2Int> doorPos = entranceGenerator.GetEntrancePos();
        foreach(var door in doorPos)
        {
            InnerWallHorizontalPos.Remove(door);
            InnerWallVerticalPos.Remove(door);
        }

    }
    void VisualizeWall()
    {
        foreach(var point in WallHorizontalPos)
        {
            GameObject gameObject=Instantiate(wallHorizontal, new Vector3(point.x, 5, point.y), wallHorizontal.transform.rotation);
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        foreach (var point in WallVerticalPos)
        {
            GameObject gameObject=Instantiate(wallVertical, new Vector3(point.x, 5, point.y), wallVertical.transform.rotation);
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }


        foreach (var point in InnerWallHorizontalPos)
        {
            GameObject gameObject = Instantiate(wallHorizontal, new Vector3(point.x, 5, point.y), wallHorizontal.transform.rotation);
            gameObject.GetComponent<Renderer>().material.color = Color.white;


        }
        foreach (var point in InnerWallVerticalPos)
        {
            GameObject gameObject = Instantiate(wallVertical, new Vector3(point.x, 5, point.y), wallVertical.transform.rotation);
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }

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
    
    void OnDrawGizmos()
    {
        foreach(var room in listOfRooms)
        {
            if (room.RoomGrid != null)
            {
                for (int x = 0; x < room.RoomGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < room.RoomGrid.GetLength(1); y++)
                    {
                        Vector2Int worldPos = room.GridToWorldPosition(x, y, (int)unitSize);
                        worldPos.x += (int)unitSize / 2;
                        worldPos.y += (int)unitSize / 2;
                        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
                        Gizmos.DrawCube(new Vector3(worldPos.x,0, worldPos.y), Vector3.one* (int)unitSize);
                    }
                }
            }
        }
        
    }

    private void OnValidate() //�ν����� �󿡼� ���� ���� ����
    {
        dungeonWidth = Mathf.Max((int)unitSize, Mathf.RoundToInt(dungeonWidth / (int)unitSize) * (int)unitSize);
        dungeonHeight = Mathf.Max((int)unitSize, Mathf.RoundToInt(dungeonHeight / (int)unitSize) * (int)unitSize);
        roomWidthMin = Mathf.Max((int)unitSize, Mathf.RoundToInt(roomWidthMin / (int)unitSize) * (int)unitSize);
        roomHeightMin = Mathf.Max((int)unitSize, Mathf.RoundToInt(roomHeightMin / (int)unitSize) * (int)unitSize);
        publicSpaceWidth = Mathf.Max((int)unitSize, Mathf.RoundToInt(publicSpaceWidth / (int)unitSize) * (int)unitSize);
        publicSpaceHeight = Mathf.Max((int)unitSize, Mathf.RoundToInt(publicSpaceHeight / (int)unitSize) * (int)unitSize);
        plazaRadius = (dungeonWidth > dungeonHeight) ? Mathf.Clamp(plazaRadius, 5, dungeonHeight / 2)
                                                     : Mathf.Clamp(plazaRadius, 5, dungeonWidth / 2);
    }


}
