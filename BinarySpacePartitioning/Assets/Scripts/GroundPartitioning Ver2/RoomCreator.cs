using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonHeight;
    public int roomWidthMin, roomHeightMin;
    public int roomWidthMax, roomHeightMax;
   
   
    public Material material; // For Visualizing




    // Start is called before the first frame update
    void Start()
    {
        CreateRoom();
    }



    public void CreateRoom()
    {
        RoomSpaceGenerator generator = new RoomSpaceGenerator(dungeonWidth, dungeonHeight);
        var listOfRooms = generator.CalculateRooms(roomWidthMin, roomHeightMin, roomWidthMax, roomHeightMax);

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
    }

    void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
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
        for (int i = 0; i < uvs.Length; i++)
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
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        Mesh mesh2 = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;

        //OutLine
        LineRenderer line = dungeonFloor.AddComponent<LineRenderer>();
        line.positionCount = 5;
        line.enabled = false;

        line.SetPosition(0, vertices[0]);
        line.SetPosition(1, vertices[1]);
        line.SetPosition(2, vertices[3]);
        line.SetPosition(3, vertices[2]);
        line.SetPosition(4, vertices[0]);


        line.enabled = true;

    }
}
