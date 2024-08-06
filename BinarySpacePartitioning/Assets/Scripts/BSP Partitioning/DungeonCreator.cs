using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class DungeonCreator : MonoBehaviour
{
    
    public int dungeonWidth, dungeonHeight;
    public int roomWidthMin, roomHeightMin;
    public int maxIterations;

    public int entranceSize; //�� �ʺ�
    public int entranceNum; //�� ����
   
    public Material material; // For Visualizing

    List<RoomNode> listOfRooms;
    RoomNode Ground;
    
   

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
        //CreateDoor();
        CreateEntrance();
    }



    public void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth,dungeonHeight);
        listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomHeightMin); //������� ����Ʈ(���� ������ �� ����Ʈ)

        //��ü �׶���(��Ʈ ���)
        Ground = generator.GetRootNode(); 
       


        //�ٴ�Ÿ�� ��� ���м� �����
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

   /* void CreateDoor()
    {
        
        List<Vector2Int> roomVertexes=new List<Vector2Int>();

        //��� �� ������ ����Ʈ �����
        foreach (var room in listOfRooms)
        {
            roomVertexes.Add(room.BottomLeftAreaCorner);
            roomVertexes.Add(room.BottomRightAreaCorner);
            roomVertexes.Add(room.TopLeftAreaCorner);
            roomVertexes.Add(room.TopRightAreaCorner);
        }

        List<Wall> checkWalls = new List<Wall>();

        //�� ���� 4 ������ �� ����
        foreach (var room in listOfRooms)
        {
            GenerateDoor(room, roomVertexes, checkWalls);
        }


    }

    void GenerateDoor(RoomNode room, List<Vector2Int> roomVertexes, List<Wall> checkWalls)
    {
        Vector2Int doorCoordinate;
        
        bool isSame;
    
        foreach (var wall in room.WallList)
        {
            isSame = false;

            //�̹� ���� ������ ���̸�, �� �������� ����
           foreach(var checkWall in checkWalls)
            {
                if (checkWall.sameWall(wall))
                {
                    isSame = true;
                    break;
                }
                    
            }

            if (isSame)
                continue;

            if (wall.Orientation == Orientation.Horizontal)
            {
                doorCoordinate=new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x-entranceSize), wall.LeftVertex.y);
                foreach(var roomVertex in roomVertexes)
                {
                    //���� x��ǥ ������ ��� ���� �������� �ش�� ��� �����
                    while ((doorCoordinate.x<= roomVertex.x && doorCoordinate.y==roomVertex.y) && (roomVertex.x <= doorCoordinate.x + entranceSize && doorCoordinate.y == roomVertex.y)) 
                    {
                        doorCoordinate = new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x - entranceSize), wall.LeftVertex.y);  
                    }

                }
            }
            else
            {
                doorCoordinate = new Vector2Int(wall.LeftVertex.x,Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));
                foreach (var roomVertex in roomVertexes)
                {
                    //���� y��ǥ ������ ��� ���� �������� �ش�� ��� �����
                    while ((doorCoordinate.y <= roomVertex.y && doorCoordinate.x==roomVertex.x) && (roomVertex.y <= doorCoordinate.y + entranceSize && doorCoordinate.x == roomVertex.x)) 
                    {
                        doorCoordinate = new Vector2Int(wall.LeftVertex.x, Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));  
                    }

                }
            }
            wall.AddDoor(doorCoordinate);
            checkWalls.Add(wall);

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

            if (wall.Orientation == Orientation.Horizontal)
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


    }*/

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
            currentNode = room.Parent;

            while(currentNode != Ground)
            {
                GenerateEntracne(currentNode, roomVertexes);
                currentNode=currentNode.Parent;
            }

         
        }
    }

    void GenerateEntracne(RoomNode room, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate=Vector2Int.zero;
        Wall line = room.GetDivideLine();

        //�� ���Ҽ����� entranceNum ��ŭ�� ���Ա� �����
        if (line.DoorNum() > entranceNum)
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
