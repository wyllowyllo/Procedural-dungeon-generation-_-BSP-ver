using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    //크로스헤어 상태에 따른 총의 정확도
    float gunAccuracy;

    [SerializeField]
    private GameObject go_CrosshairHUD;
    [SerializeField]
    GunController theGunController;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WalkingAnimation(bool flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Walk", flag);
        anim.SetBool("Walking", flag);
    }
    public void RunningAnimation(bool flag)
    {
        WeaponManager.currentWeaponAnim.SetBool("Run", flag);
        anim.SetBool("Running", flag);
    }
    public void JumpingAniamtion(bool flag)
    {
        anim.SetBool("Running", flag);
    }
    public void CrouchingAnimation(bool flag)
    {
        anim.SetBool("Crouching", flag);
    }
    public void FineSightAnimation(bool flag)
    {
        anim.SetBool("FineSight", flag);
    }

    public void FireAnimation()
    {
        if (anim.GetBool("Walking"))
            anim.SetTrigger("Walk_Fire");
        
        else if (anim.GetBool("Crouching"))       
            anim.SetTrigger("Crouch_Fire");
        
        else 
            anim.SetTrigger("Idle_Fire");
        
    }

    public float GetAccuacy()
    {
        if (anim.GetBool("Walking"))
            gunAccuracy = 0.06f;

        else if (anim.GetBool("Crouching"))
            gunAccuracy = 0.015f;

        else if (theGunController.GetFineSightMode())
            gunAccuracy = 0.001f;
        else
            gunAccuracy = 0.03f;

        return gunAccuracy;
    }
}
