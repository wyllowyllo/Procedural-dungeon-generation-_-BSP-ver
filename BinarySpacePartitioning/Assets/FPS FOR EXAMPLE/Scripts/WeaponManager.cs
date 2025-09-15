using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 중복교체 실행 x
    public static bool isChangedWeapon = false;

    //현재 무기, 현재 무기 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    //현재무기 타입
    [SerializeField]
    private string currentWeaponType;

    //무기 교체 딜레이
    [SerializeField]
    private float changedWeaponDelay;
    [SerializeField]
    private float changedWeaponEndDelayTime; //교체 끝나는 시점

    //무기, 손 종류
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private Hand[] hands;

    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, Hand> handDictionary = new Dictionary<string, Hand>();

    //교체시 각각 활성화/비활성화함
    [SerializeField]
    private GunController theGunController;
    [SerializeField]
    private HandController theHandController;

    // Start is called before the first frame update
    [SerializeField]
    private void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].handName, hands[i]);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isChangedWeapon)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //무기 교체(맨손)
            StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));

            
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //무기 교체(서브머신건)
            StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
        }
    }
    public IEnumerator ChangeWeaponCoroutine(string type, string name)
    {
        isChangedWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changedWeaponDelay);

       
        CancelPreWeaponAction();  //이전에 실행되고 있던 코루틴 모두 종료시킴
        WeaponChange(type, name);

        yield return new WaitForSeconds(changedWeaponEndDelayTime);
        currentWeaponType = type;
        isChangedWeapon = false;
    }
    void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
        }
    }
    void WeaponChange(string type, string name)
    {
        if (type == "GUN")
        {
            theGunController.GunChange(gunDictionary[name]);
        }
        else if (type == "HAND")
        {
            theHandController.HandChange(handDictionary[name]);
        }
    }
}
