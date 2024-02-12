using UnityEngine;

public class ChibiDialogMain : MonoBehaviour
{

    public Chibi.Free.Dialog dialog;

    public void OnClickButton()
    {
        var cancel = new Chibi.Free.Dialog.ActionButton("Cancel", null, new Color(0.9f, 0.9f, 0.9f));
        var ok = new Chibi.Free.Dialog.ActionButton("OK", () =>
        { 
            Debug.Log("click ok");
        }, new Color(0f, 0.9f, 0.9f));
        Chibi.Free.Dialog.ActionButton[] buttons = { cancel, ok };
        dialog.ShowDialog("★Chibi Dialog★", "It's easy!", buttons);
    }

    public void OnClickButton2()
    {
        var cancel = new Chibi.Free.Dialog.ActionButton("Cancel", null, new Color(0.9f, 0.9f, 0.9f));
        var ok = new Chibi.Free.Dialog.ActionButton("OK", () =>
        {
            Debug.Log("click ok");
        }, new Color(0f, 0.9f, 0.9f));
        Chibi.Free.Dialog.ActionButton[] buttons = { cancel, ok };
        dialog.ShowDialog("★Chibi Dialog★", "It's easy!", buttons, () =>
        {
            Debug.Log("closed dialog.");
        }, true);
    }
}

