using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
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

        signInBtn.btn.AddEvent(async () => await OnClickSignInBtn());
    }


    void OnEnable()
    {
        usernameInput.ResetInput();
        passwordInput.ResetInput();
        CheckSignInBtn();
        errorText.text.text = string.Empty;
    }

    void OnDestroy()
    {
        _ = Authentication.Instance.SignOutAsync();
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

    public async Awaitable OnClickSignInBtn() 
    {
        PanelManager.Instance.loading.Show();
        (bool ok, Exception error) = await Authentication.Instance.SignInUsernameAsync(usernameInput.input.text, passwordInput.input.text);
        PanelManager.Instance.loading.Hide();

        if (!ok)
        {
            errorText.text.text = GetSinginErrorMessage(error);
            return;
        }

        PanelManager.Instance.home.Show();
        Hide();
    }

    string GetSinginErrorMessage(Exception error) 
    {
        if (error is AuthenticationException authError) 
        {
            if (authError.ErrorCode == AuthenticationErrorCodes.InvalidParameters) 
            {
                return "Please check your username and password.";
            }
            else if (authError.ErrorCode == AuthenticationErrorCodes.ClientInvalidUserState) 
            {
                return "You are already signed in";
            }
            else 
            {
                return authError.Message;
            }
        }
        else if (error is RequestFailedException requestError) 
        {
            if (requestError.ErrorCode == CommonErrorCodes.Conflict) 
            {
                return "Username already exists";
            }
            else if (requestError.ErrorCode == CommonErrorCodes.TransportError)
            {
                return "Network error. Please try again.";
            }
            else if (requestError.ErrorCode == CommonErrorCodes.TooManyRequests)
            {
                return "Too many attempts. Please wait and try again.";
            }
            else 
            {
                return requestError.Message;
            }
        }
        else 
        {
            return error.Message;
        }
    }
}