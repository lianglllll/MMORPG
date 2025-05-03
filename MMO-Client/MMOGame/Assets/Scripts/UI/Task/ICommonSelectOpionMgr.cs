using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommonSelectOpionMgr
{
    void InitOptions(List<CommonSelectOption> options, List<string> optionNames, List<int> flags);
    void Selected(CommonSelectOption commonSelectOption);
}
