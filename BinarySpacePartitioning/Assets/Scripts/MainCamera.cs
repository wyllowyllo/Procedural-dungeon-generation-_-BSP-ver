using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Vector3 offSet;
    public Transform target;

    // Update is called once per frame
    private void Awake()
    {
        offSet= transform.position-target.position;
    }
    void Update()
    {
        transform.position=target.position+offSet;
      
    }
}
