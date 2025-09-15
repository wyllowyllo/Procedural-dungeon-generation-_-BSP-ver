using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{

    //활성화 여부
    public static bool isActivate = false;

    //현재 장착된 Hand형 타입 무기
    [SerializeField]
    private Hand currentHand;

    //공격 중
    bool isAttack;
    bool isSwing; //팔을 편 상태

    RaycastHit hitInfo;

    private void Update()
    {
        if (!isActivate)
            return;

         TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (isAttack)
                return;
            StartCoroutine(AttackCoroutine());
        }
    }
    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        //공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay-(currentHand.attackDelayA+ currentHand.attackDelayB)); // 이전에서 콜라이더 활성/비활성 때 이미 딜레이했으므로, (전체 딜레이 - 콜라이더 활성/비활성 딜레이) 만큼 딜레이
        isAttack = false;
       
    }

    IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            //Swing시간동안 계속 물체 탐지
            if (CheckObject()) 
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false; 
    }

    public void HandChange(Hand hand)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentHand = hand;
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentHand.anim;

        currentHand.transform.localPosition = Vector3.zero;
        currentHand.gameObject.SetActive(true);
        isActivate = true;
    }
}
