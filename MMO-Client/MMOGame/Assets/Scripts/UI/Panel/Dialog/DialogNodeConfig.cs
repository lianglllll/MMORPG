using System.Collections.Generic;

public class DialogNodeConfig
{
    public bool isPlayer;
    public string contexts;
    public List<IDialogEvent> onStartEventList = new();
    public List<IDialogEvent> onEndEventList = new();
}
