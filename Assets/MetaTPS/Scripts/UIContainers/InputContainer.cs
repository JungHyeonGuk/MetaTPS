using UnityEngine;
using TMPro;

public class InputContainer : MonoBehaviour 
{
    public TMP_InputField input;
    public GameObject errorOutline;



    public void ResetInput() 
    {
        input.SetTextWithoutNotify(string.Empty);
        input.caretPosition = 0;
        input.stringPosition = 0;
        input.selectionAnchorPosition = 0;
        input.selectionFocusPosition = 0;
        input.ForceLabelUpdate();
        input.DeactivateInputField(); 

        errorOutline.SetActive(false);
    }
}