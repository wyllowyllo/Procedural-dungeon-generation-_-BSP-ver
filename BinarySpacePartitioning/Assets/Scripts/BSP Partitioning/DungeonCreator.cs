using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public enum PUBLICSPACE { none, left_bottom, right_bottom, left_top, right_top, middle, plaza } //������� ���� Ÿ��
public class DungeonCreator : MonoBehaviour
{
    public PUBLICSPACE type;
    public int dungeonWidth, dungeonHeight; //��ü �׶��� �ʺ�, ����
    public int roomWidthMin, roomHeightMin; //�ּ� �� ũ��

    public int publicSpaceWidth, publicSpaceHeight; //������� �ʺ�, ���� (type�� none�̸� ������� ����)
    public int publicSpaceDoorNumOnWall; //��������� ����� �� ���� (����� �ּ� 1��)

    
    public int maxIterations; //�ִ� ���� Ƚ�� - Ʈ�� ���� ����, �� ���� �������� ���� ū ���� ������� (PublicSpace Ÿ���� none�� ��쿡�� ����)

    public int entranceSize; //�� �ʺ�
   
   
    public Material material; // For Visualizing

    List<RoomNode> listOfRooms=new List<RoomNode>();
    RoomNode Ground;


    private void Awake()
    {
        if (type == PUBLICSPACE.none)
            return;

        //�ּ� �� ũ��� (��ü �׶��� ũ�� - ������� ũ��) ���� �۾ƾ� ��
        if(roomWidthMin > (dungeonWidth - publicSpaceWidth))
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
               
                //�������(0�� �ε���) �����ϰ� ���� ����
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

                //�������(0�� �ε���) �����ϰ� ���� ����
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
        List<RoomNode> leafList = generator.CalculateRooms(startPoint, maxIterations, roomWidthMin, roomHeightMin, type); //������� ����Ʈ(���� ������ �� ����Ʈ)

      

        if(type==PUBLICSPACE.none)
        {
           
            Ground = generator.GetRootNode();  //��ü �׶���(��Ʈ ���)
            listOfRooms = leafList;
            //�ٴ�Ÿ�� ��� ���м� �����
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
        else if (type == PUBLICSPACE.middle)
        {
            Vector2Int center = new Vector2Int(dungeonWidth / 2, dungeonHeight / 2); //�׶��� ���߾�
            Vector2Int leftBottomPoint=new Vector2Int(center.x-(publicSpaceWidth/2), center.y-(publicSpaceHeight/2));
            Vector2Int rightTopPoint = new Vector2Int(center.x + (publicSpaceWidth / 2), center.y + (publicSpaceHeight / 2));

            //�������
            RoomNode node1 = new RoomNode(leftBottomPoint, rightTopPoint
                                , null
                                , 0);
            rootList.Add(node1);

            //������ ���� ����
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

        //��� �� ������ ����Ʈ �����
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

        //��������� ���� ��� ��������� ��� ���� ����
        if(type!=PUBLICSPACE.none)
        {
            RoomNode publicSpaceNode=listOfRooms[0];
           
            switch(type)
            {
                //���� �Ʒ� �ڳʿ� ��������� ���� ���, �ش� ������ ������, ���� ���鿡 ��� ����
                case PUBLICSPACE.left_bottom:

                    while(publicSpaceNode.WallList[1].DoorNum()<publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[1], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //������ �Ʒ� �ڳʿ� ��������� ���� ���, �ش� ������ ����, ���� ���鿡 ��� ����
                case PUBLICSPACE.right_bottom:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[3].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[3], roomVertexes);
                    break;

                //������ �� �ڳʿ� ��������� ���� ���, �ش� ������ ����, �Ʒ��� ���鿡 ��� ����
                case PUBLICSPACE.right_top:
                    while (publicSpaceNode.WallList[0].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[0], roomVertexes);
                    while (publicSpaceNode.WallList[2].DoorNum() < publicSpaceDoorNumOnWall)
                        GenerateEntrance(publicSpaceNode.WallList[2], roomVertexes);
                    break;

                //���� �� �ڳʿ� ��������� ���� ���, �ش� ������ �Ʒ���, ������ ���鿡 ��� ����
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

        //�� ���Ҽ����� 1���� ���Ա� �����
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

                        //���� x��ǥ ������ ��� ���� �������� �ش�� ��� �����
                        if(IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal)) 
                        {
                            overlap = true;
                            break;
                        }
                        else
                        {
                            foreach(var doorPos in room.GetDivideLine().GetDoorList())
                            {
                                //���� ��ġ�� �ʵ��� ����
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

                        //���� y��ǥ ������ ��� ���� �������� �ش�� ��� �����
                        if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical)) 
                        {
                            overlap = true;
                            break;
                        }
                        else
                        {
                            foreach (var doorPos in room.GetDivideLine().GetDoorList())
                            {
                                //���� ��ġ�� �ʵ��� ����
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

            //���м� �׸���
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

                    //���� x��ǥ ������ ��� ���� �������� �ش�� ��� �����
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Horizontal))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //���� ��ġ�� �ʵ��� ����
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

                    //���� y��ǥ ������ ��� ���� �������� �ش�� ��� �����
                    if (IsOnVertex(doorCoordinate, roomVertex, Orientation.Vertical))
                    {
                        overlap = true;
                        break;
                    }
                    else
                    {
                        foreach (var doorPos in wall.GetDoorList())
                        {
                            //���� ��ġ�� �ʵ��� ����
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

        //���м� �׸���
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
