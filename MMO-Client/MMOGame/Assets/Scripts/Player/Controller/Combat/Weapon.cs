using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private int weaponId;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init()
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void Show()
    {
        animator.SetBool("Showing", true);
    }
    public void Hide()
    {
        // animator.SetBool("Hideing", true);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }



    protected void ShowEnd()
    {
        animator.SetBool("Showing", false);
    }
    protected void HideEnd()
    {
        animator.SetBool("Hideing", false);
        gameObject.SetActive(false);
    }




}
