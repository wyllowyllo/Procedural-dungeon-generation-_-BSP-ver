using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    public float moveSpeed;

    Vector2Int playerPosInGrid;
    Vector2Int destPosInGrid;
    bool isMoving;

    // Update is called once per frame

    private void Awake()
    {
        camera = FindObjectOfType<MainCamera>().GetComponent<Camera>();
        moveSpeed = 10.0f;
    }
    void Update()
    {
        Move();
    }


    private void Move()
    {
        if (!Input.GetMouseButtonDown(1)||isMoving)
            return;

        Ray ray=camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray , out hit))
        {
            Vector3 hitPoint = hit.point;
            Vector2Int gridPosition = DungeonCreator.gridManager.WorldToGridPosition(hitPoint);

            if(gridPosition!=new Vector2Int(-1, -1))
            {
                playerPosInGrid = DungeonCreator.gridManager.WorldToGridPosition(transform.position);
                destPosInGrid = gridPosition;

                List<Vector2Int> path = DungeonCreator.gridManager.FindPath(playerPosInGrid,destPosInGrid);

                if (path != null)
                {
                    isMoving= true;
                    /* foreach (var dot in path)
                     {
                         GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                         cube.transform.position = new Vector3(dot.x, 5, dot.y);

                     }*/
                    StopAllCoroutines();
                    StartCoroutine(FollowPath(path));

                }
                else
                {
                    Debug.Log("가능한 경로가 없습니다");
                }

            }

           
        }

        
        Debug.Log(destPosInGrid);


    }
    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        foreach (var point in path)
        {
            Vector3 targetPosition = new Vector3(point.x, transform.position.y, point.y);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
    }


}
