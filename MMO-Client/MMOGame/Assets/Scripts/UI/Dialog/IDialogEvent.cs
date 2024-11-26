using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogEvent 
{
    public void Execute();
    //阻塞执行 
    public IEnumerator ExcuteBlocking();
    public void ConverString(string excelString);
}
