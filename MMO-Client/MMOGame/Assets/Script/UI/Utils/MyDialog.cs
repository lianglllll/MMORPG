using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDialog : MonoBehaviour
{
    public static Chibi.Free.Dialog dialog;

    private void Start()
    {
        dialog = GameObject.Find("ChibiDialog").GetComponent<Chibi.Free.Dialog>();
    }
    public static void Show(string title, string content, Chibi.Free.Dialog.ActionButton[] buttons)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            dialog.ShowDialog(title, content, buttons);
        });
    }

    //显示消息
    public static void ShowMessage(string title, string content)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //ui设置按钮
            var ok = new Chibi.Free.Dialog.ActionButton("确定", () =>{}, new Color(0f, 0.9f, 0.9f));
            Chibi.Free.Dialog.ActionButton[] buttons = { ok };
            //调用工具类进行弹窗,工具类mydialog里面实现了异步
            MyDialog.Show(title, content, buttons);
        });
    }
}
