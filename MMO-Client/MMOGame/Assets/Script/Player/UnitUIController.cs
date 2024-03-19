using GameClient.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUIController : MonoBehaviour
{
    private Actor owner;
    private GameObject SelectMark;

    private void Awake()
    {
        SelectMark = transform.Find("Canvas/SelectMark").gameObject;
    }

    private void Start()
    {
        SelectMark.SetActive(false);
    }

    public void Init(Actor actor)
    {
        this.owner = actor;
    }


    public void ShowSelectMark()
    {
        SelectMark.SetActive(true);
    }

    public void HideSelectMark()
    {
        SelectMark.SetActive(false);
    }
}
