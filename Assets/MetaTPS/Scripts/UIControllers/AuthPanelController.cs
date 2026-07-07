using UnityEngine;

public class AuthPanelController : PanelBase 
{
    [SerializeField] BtnContainer signUpBtn;
    [SerializeField] BtnContainer signInBtn;



    public override void Init()
    {
        base.Init();
        signUpBtn.btn.AddEvent(OnClickSignUpBtn);
        signInBtn.btn.AddEvent(OnClickSignInBtn);

    }


    void OnClickSignUpBtn() 
    {
        PanelManager.Instance.signUp.Show();
        Hide();
    }

    void OnClickSignInBtn() 
    {
        PanelManager.Instance.signIn.Show();
        Hide();
    }

}