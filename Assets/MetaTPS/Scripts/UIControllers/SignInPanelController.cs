using UnityEngine;

public class SignInPanelController : PanelBase 
{
    [SerializeField] CloseBtnContainer closeBtn;
    [SerializeField] InputContainer usernameInput;
    [SerializeField] InputSecretContainer passwordInput;
    [SerializeField] ErrorTextContainer errorText;
    [SerializeField] BtnContainer signInBtn;



    public override void Init()
    {
        base.Init();
        closeBtn.btn.AddEvent(OnClickCloseBtn);

        usernameInput.input.AddValueChangedEvent(OnValueChangedInput);
        passwordInput.input.AddValueChangedEvent(OnValueChangedInput);

        signInBtn.btn.AddEvent(OnClickSignInBtn);
    }


    void OnEnable()
    {
        usernameInput.ResetInput();
        passwordInput.ResetInput();
        CheckSignInBtn();
    }

    void OnClickCloseBtn() 
    {
        Hide();
        PanelManager.Instance.auth.Show();
    }

    void OnValueChangedInput(string value) 
    {
        CheckSignInBtn();
    }

    void CheckSignInBtn() 
    {
        signInBtn.btn.interactable = !string.IsNullOrWhiteSpace(usernameInput.input.text) 
            && !string.IsNullOrWhiteSpace(passwordInput.input.text);
    }

    void OnClickSignInBtn() 
    {
        // Sign In
    }


}