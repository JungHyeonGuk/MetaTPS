using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputSecretContainer : MonoBehaviour 
{
    public TMP_InputField input;
    public GameObject errorOutline;
    [SerializeField] Button visionBtn;
    [SerializeField] Image visionIcon;
    [SerializeField] Sprite vision;
    [SerializeField] Sprite invision;

    bool isVision;



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

    public void SetText(string text)
    {
        input.SetTextWithoutNotify(text);
        input.caretPosition = text.Length;
        input.stringPosition = text.Length;
        input.selectionAnchorPosition = text.Length;
        input.selectionFocusPosition = text.Length;
    }


    void OnEnable()
    {
        SetVision(false);
        visionBtn.AddEvent(OnClickVisionBtn);
    }

    void SetVision(bool isVision)
    {
        input.contentType = isVision ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        input.ForceLabelUpdate();

        visionIcon.sprite = isVision ? invision : vision;
        this.isVision = isVision;
    }

    void OnClickVisionBtn() 
    {
        SetVision(!isVision);
    }
}