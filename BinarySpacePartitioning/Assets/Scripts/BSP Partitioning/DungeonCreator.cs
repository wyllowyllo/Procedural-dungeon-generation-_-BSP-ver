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
    public int entranceSize;
   
    public Material material; // For Visualizing

    List<Node> listOfRooms;
    
   

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }



    public void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth,dungeonHeight);
        listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomHeightMin); //������� ����Ʈ(���� ������ �� ����Ʈ)

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

        Mesh mesh2 = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

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

        //��� �� ������ ����Ʈ �����
        foreach (var room in listOfRooms)
        {
            roomVertexes.Add(room.BottomLeftAreaCorner);
            roomVertexes.Add(room.BottomRightAreaCorner);
            roomVertexes.Add(room.TopLeftAreaCorner);
            roomVertexes.Add(room.TopRightAreaCorner);
        }

        //�� �� ����� ���� set
        foreach(var room in listOfRooms)
        {
           
        }


    }
}
