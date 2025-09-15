using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    Vector3 originPos;
    Vector3 curreuntPos;

    //일반모드, 정조준모드 위치 제한점.
    [SerializeField]
    Vector3 limitPos;
    [SerializeField]
    Vector3 fineSightLimitPos;

    [SerializeField]
    Vector3 smoothSway; //부드러운 움직임 정도

    [SerializeField]
    GunController theGunController;

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        TrySway();
    }

    void TrySway()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            Swaying();
        }
        else
            BackToOriginPos();
    }

    

    //무기 흔들림 구현
    private void Swaying()
    {
        float moveX = Input.GetAxisRaw("Mouse X");
        float moveY = Input.GetAxisRaw("Mouse Y");

        
        if (!theGunController.isFineSightMode)
        {
            //일반모드에서는 smooth값은 x
            curreuntPos.Set(Mathf.Clamp(Mathf.Lerp(curreuntPos.x, -moveX, smoothSway.x), -limitPos.x, limitPos.x),
                            Mathf.Clamp(Mathf.Lerp(curreuntPos.y, -moveY, smoothSway.x), -limitPos.y, limitPos.y),
                            originPos.z);
        }
        else
        {
            //정조준에서는 smooth값은 y
            curreuntPos.Set(Mathf.Clamp(Mathf.Lerp(curreuntPos.x, -moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                            Mathf.Clamp(Mathf.Lerp(curreuntPos.y, -moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                            originPos.z);
        }
        

        transform.localPosition = curreuntPos;

    }
    private void BackToOriginPos()
    {
        curreuntPos = Vector3.Lerp(curreuntPos, originPos, smoothSway.x);
        transform.localPosition = curreuntPos;
    }
}
