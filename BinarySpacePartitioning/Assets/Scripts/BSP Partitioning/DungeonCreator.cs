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
    public int entranceSize; //문 너비
   
    public Material material; // For Visualizing

    List<RoomNode> listOfRooms;
    RoomNode Ground;
    
   

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
        CreateDoor();
        
    }



    public void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth,dungeonHeight);
        listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomHeightMin); //리프노드 리스트(실제 생성된 방 리스트)

        //전체 그라운드(루트 노드)
        Ground = generator.GetRootNode(); 
       


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

    void CreateDoor()
    {
        
        List<Vector2Int> roomVertexes=new List<Vector2Int>();

        //모든 방 꼭짓점 리스트 만들기
        foreach (var room in listOfRooms)
        {
            roomVertexes.Add(room.BottomLeftAreaCorner);
            roomVertexes.Add(room.BottomRightAreaCorner);
            roomVertexes.Add(room.TopLeftAreaCorner);
            roomVertexes.Add(room.TopRightAreaCorner);
        }
        //각 방의 4 벽마다 문 생성
        foreach (var room in listOfRooms)
        {
            GenerateDoor(room, roomVertexes);
        }


    }

    void GenerateDoor(RoomNode room, List<Vector2Int> roomVertexes)
    {
        Vector2Int doorCoordinate;
       
    
        foreach (var wall in room.WallList)
        {
            if (wall.DoorNum()!=0)
                return;

            if (wall.Orientation == Orientation.Horizontal)
            {
                doorCoordinate=new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x-entranceSize), wall.LeftVertex.y);
                foreach(var roomVertex in roomVertexes)
                {
                    while((doorCoordinate.x<= roomVertex.x && doorCoordinate.y==roomVertex.y) && (roomVertex.x <= doorCoordinate.x + entranceSize && doorCoordinate.y == roomVertex.y)) //문의 x좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우
                    {
                        doorCoordinate = new Vector2Int(Random.Range(wall.LeftVertex.x, wall.RightVertex.x - entranceSize), wall.LeftVertex.y);  //재생성
                    }

                }
            }
            else
            {
                doorCoordinate = new Vector2Int(wall.LeftVertex.x,Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));
                foreach (var roomVertex in roomVertexes)
                {
                    while ((doorCoordinate.y <= roomVertex.y && doorCoordinate.x==roomVertex.x) && (roomVertex.y <= doorCoordinate.y + entranceSize && doorCoordinate.x == roomVertex.x)) //문의 y좌표 범위가 어떠한 방의 꼭짓점에 해당될 경우
                    {
                        doorCoordinate = new Vector2Int(wall.LeftVertex.x, Random.Range(wall.RightVertex.y, wall.LeftVertex.y - entranceSize));  //재생성
                    }

                }
            }
            wall.AddDoor(doorCoordinate);

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


    }

}
