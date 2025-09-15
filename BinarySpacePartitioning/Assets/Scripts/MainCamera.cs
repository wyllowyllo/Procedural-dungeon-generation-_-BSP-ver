using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Vector3 offSet;
    public Transform target;
    bool targetOn = false;
    // Update is called once per frame
    private void Awake()
    {
        //offSet= transform.position-target.position;
    }

    public void TargetSet()
    {

    }
    void Update()
    {
        if (!targetOn) return;

        transform.position=target.position+offSet;
      
    }
}
