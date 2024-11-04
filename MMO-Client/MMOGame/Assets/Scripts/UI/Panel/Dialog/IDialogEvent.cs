using System.Collections;

public interface IDialogEvent
{
    public bool Execute();
    public IEnumerator ExecuteBlocking();
    public bool ConverString(string excelString);
}
