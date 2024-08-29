using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
   
    public Camera camera;
    public float moveSpeed;
    public float turnSpeed;
    public float moveDelay;

    Vector2Int playerPosInGrid;
    Vector2Int destPosInGrid;
  
    

    bool isMoving;

    

    private void Awake()
    {
        camera = FindObjectOfType<MainCamera>().GetComponent<Camera>();
        moveSpeed = 10.0f;
        turnSpeed = 1f;
        moveDelay = 0.0001f;

        transform.localScale = new Vector3(transform.localScale.x/2, transform.localScale.y/2, transform.localScale.z/2);
        float bodyHalfSize = GetComponent<Collider>().bounds.extents.y;
        transform.position = new Vector3(transform.position.x,0+bodyHalfSize, transform.position.z);




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
            transform.LookAt(targetPosition);


            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
              
                yield return new WaitForSeconds(moveDelay);
            }
        }
        isMoving = false;
    }


}
