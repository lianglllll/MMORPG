using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> weapons;
    private Dictionary<int, Weapon> weaponsDict;
    private int curWeaponId;

    public void Init()
    {
        foreach(var item in weapons){
            item.GetComponent<Weapon>().Init();
        }
    }

    public void ShowCurWeapon()
    {
        weapons[0].gameObject.SetActive(true);
        weapons[0].gameObject.GetComponent<Weapon>().Show();
    }

    public void HideCurWeapon()
    {
        weapons[0].gameObject.GetComponent<Weapon>().Hide();
    }

}
