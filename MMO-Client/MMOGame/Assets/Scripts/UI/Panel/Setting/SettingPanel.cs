using BaseSystem.MyDelayedTaskScheduler;
using GameClient.HSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingType
{
    Game, Controller, Key, Vido, Audio, KeyHelp
}

public class SettingPanel : BasePanel
{
    private SettingType curType;
    private SettingSelectOption curOption;
    private Dictionary<SettingType, Transform> menuDict = new();

    protected override void Start()
    {
        base.Start();

        //建立映射
        SettingType[] values = (SettingType[])Enum.GetValues(typeof(SettingType));
        var options = transform.Find("SelectOptions");
        var menus = transform.Find("Menu");
        for (int index = 0;index < options.childCount; ++index)
        {
            var s = options.GetChild(index).GetComponent<SettingSelectOption>();
            s.Init(this, values[index]);
            var obj = menus.GetChild(index);
            obj.gameObject.SetActive(false);
            menuDict[values[index]] = obj;
        }

        //默认选择第一个menu
        DelayedTaskScheduler.Instance.AddDelayedTask(0.1f, () => {
            var option = options.GetChild(0).GetComponent<SettingSelectOption>();
            Selected(option);
        });
    }

    public void Selected(SettingSelectOption option)
    {

        if(curOption != null)
        {
            curOption.CancelClick();
            menuDict[curType].gameObject.SetActive(false);
        }
        curOption = option;
        curType = option.type;
        curOption.OnClick();
        menuDict[curType].gameObject.SetActive(true);
    }

}
