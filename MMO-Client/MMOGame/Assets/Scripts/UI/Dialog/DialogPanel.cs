using GameClient.UI.Dialog;
using UnityEngine;

public class DialogPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.init();
        DialogConfigImporter.Instance.Init();
        var item = DialogConfigImporter.Instance.GetDialogConfigByDid(0);
    }

}
