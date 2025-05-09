using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommonListSelectOpionMgr
{
    void InitOptions(List<CommonListSelectOption> options, List<string> optionNames, List<int> flags);
    void Selected(CommonListSelectOption commonSelectOption);
}
