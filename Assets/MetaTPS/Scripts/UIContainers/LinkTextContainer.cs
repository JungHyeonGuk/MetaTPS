using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LinkTextContainer : MonoBehaviour 
{
    public Button btn;
    public TMP_Text text;
    


    public void SetUnderline(bool isUnderline) 
    {
        if (isUnderline) 
        {
            text.fontStyle |= FontStyles.Underline; 
        }
        else
        {
            text.fontStyle &= ~FontStyles.Underline; 
        }
        text.ForceMeshUpdate();
    }

}