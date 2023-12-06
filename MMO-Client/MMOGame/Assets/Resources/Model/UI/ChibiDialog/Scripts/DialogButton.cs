using UnityEngine;

namespace Chibi.Free
{

    public class DialogButton : MonoBehaviour
    {

        public Dialog parent;
        public int index;

        public void OnClick()
        {
            parent.OnClickButton(index);
        }

    }

}