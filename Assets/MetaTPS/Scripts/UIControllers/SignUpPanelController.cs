using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class SignUpPanelController : PanelBase 
{
    [SerializeField] CloseBtnContainer closeBtn;
    [SerializeField] InputContainer usernameInput;
    [SerializeField] ErrorTextContainer usernameErrorText;
    [SerializeField] InputSecretContainer passwordInput;
    [SerializeField] ErrorTextContainer passwordErrorText;
    [SerializeField] InputSecretContainer confirmPasswordInput;
    [SerializeField] ErrorTextContainer confirmPasswordErrorText;
    [SerializeField] BtnContainer signUpBtn;
    [SerializeField] RectTransform guideline;

    string normalHex;
    string errorHex;
    bool isUsernameValid;
    bool isPasswordValid;
    bool isConfirmPasswordValid;


    public override void Init()
    {
        base.Init();
        closeBtn.btn.AddEvent(OnClickCloseBtn);
        signUpBtn.btn.AddEvent(OnClickSignUpBtn);

        usernameInput.input.AddValueChangedEvent(OnValueChangedUsernameInput);
        usernameInput.input.AddSelectEvent(OnSelectUsernameInput);
        usernameInput.input.AddDeselectEvent(OnDeselectUsernameInput);
        passwordInput.input.AddValueChangedEvent(OnValueChangedPasswordInput);
        passwordInput.input.AddSelectEvent(OnSelectPasswordInput);
        passwordInput.input.AddDeselectEvent(OnDeselectPasswordInput);
        confirmPasswordInput.input.AddValueChangedEvent(OnValueChangedConfirmPasswordInput);
        confirmPasswordInput.input.AddSelectEvent(OnSelectConfirmPasswordInput);
        confirmPasswordInput.input.AddDeselectEvent(OnDeselectConfirmPasswordInput);

        normalHex = ColorUtility.ToHtmlStringRGB(usernameErrorText.normalColor);
        errorHex = ColorUtility.ToHtmlStringRGB(usernameErrorText.errorColor);
    }

    void OnEnable()
    {
        isUsernameValid = isPasswordValid = isConfirmPasswordValid = false;
        CheckSignUpBtn();

        usernameInput.ResetInput();
        passwordInput.ResetInput();
        confirmPasswordInput.ResetInput();
    }

    void OnClickCloseBtn() 
    {
        PanelManager.Instance.auth.Show();
        Hide();
    }

    string GetConditionText((string text, bool isValid)[] conditions, out bool isAllValid) 
    {
        StringBuilder sb = new();
        isAllValid = true;
        foreach (var condition in conditions) 
        {
            string hexColor = condition.isValid ? normalHex : errorHex;
            sb.AppendLine($"<color=#{hexColor}>{condition.text}</color>");

            if (!condition.isValid) 
            {
                isAllValid = false;
            }
        }
        return sb.ToString();
    }

    void OnValueChangedUsernameInput(string value) 
    {
        bool hasValue = !string.IsNullOrWhiteSpace(value);

        usernameErrorText.text.text = GetConditionText(new (string text, bool isValid)[] {
            ("3-20 characters", hasValue ? value.Length >= 3 && value.Length <= 20 : true),
            ("Letters, numbers, and \"_\" only", hasValue ? Regex.IsMatch(value, "^[a-zA-Z0-9_]+$") : true)
        }, out bool isAllValid);

        if (hasValue) 
        {
            isUsernameValid = isAllValid;
        }

        usernameInput.errorOutline.SetActive(!isAllValid);

        LayoutRebuilder.ForceRebuildLayoutImmediate(guideline);
        CheckSignUpBtn();
    }

    void OnSelectUsernameInput(string value) 
    {
        usernameErrorText.gameObject.SetActive(true);
        OnValueChangedUsernameInput(value);
        CheckSignUpBtn();
    }

    void OnDeselectUsernameInput(string value) 
    {
        usernameErrorText.gameObject.SetActive(false);
        CheckSignUpBtn();
    }

    void OnValueChangedPasswordInput(string value) 
    {
        bool hasValue = !string.IsNullOrWhiteSpace(value);

        passwordErrorText.text.text = GetConditionText(new (string text, bool isValid)[] {
            ("At least 8 characters", hasValue ? value.Length >= 8 : true),
            ("Different from username", hasValue ? usernameInput.input.text != value : true),
            ("Include at least 1 uppercase, lowercase, number, and symbol", hasValue ? Regex.IsMatch(value, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*])[a-zA-Z\\d!@#$%^&*]{8,30}$") : true),
        }, out bool isAllValid);

        if (hasValue) 
        {
            isPasswordValid = isAllValid;
        }

        passwordInput.errorOutline.SetActive(!isAllValid);

        LayoutRebuilder.ForceRebuildLayoutImmediate(guideline);
        CheckSignUpBtn();
    }

    void OnSelectPasswordInput(string value) 
    {
        passwordErrorText.gameObject.SetActive(true);
        OnValueChangedPasswordInput(value);
        CheckSignUpBtn();
    }

    void OnDeselectPasswordInput(string value) 
    {
        passwordErrorText.gameObject.SetActive(false);
        CheckSignUpBtn();
    }

    void OnValueChangedConfirmPasswordInput(string value) 
    {
        bool hasValue = !string.IsNullOrWhiteSpace(value);

        confirmPasswordErrorText.text.text = GetConditionText(new (string text, bool isValid)[] {
            ("Confirm password", hasValue ? value == passwordInput.input.text : true),
        }, out bool isAllValid);

        if (hasValue) 
        {
            isConfirmPasswordValid = isAllValid;
        }

        confirmPasswordInput.errorOutline.SetActive(!isAllValid);

        LayoutRebuilder.ForceRebuildLayoutImmediate(guideline);
        CheckSignUpBtn();
    }

    void OnSelectConfirmPasswordInput(string value) 
    {
        confirmPasswordErrorText.gameObject.SetActive(true);
        OnValueChangedConfirmPasswordInput(value);
        CheckSignUpBtn();
    }

    void OnDeselectConfirmPasswordInput(string value) 
    {
        confirmPasswordErrorText.gameObject.SetActive(false);
        CheckSignUpBtn();
    }

    void CheckSignUpBtn() 
    {
        bool isAllFilled = !string.IsNullOrWhiteSpace(usernameInput.input.text) 
            && !string.IsNullOrWhiteSpace(passwordInput.input.text) 
            && !string.IsNullOrWhiteSpace(confirmPasswordInput.input.text);
        signUpBtn.btn.interactable = isAllFilled && isUsernameValid && isPasswordValid && isConfirmPasswordValid;
    }

    async void OnClickSignUpBtn() 
    {
        PanelManager.Instance.loading.Show();
        (bool ok, Exception error) = await Authentication.Instance.SignUpUsernameAsync(usernameInput.input.text, passwordInput.input.text);

        if (!ok)
        {
            Debug.LogError(error.Message);
            PanelManager.Instance.loading.Hide();
            return;
        }

        PanelManager.Instance.loading.Hide();
        PanelManager.Instance.auth.Show();
        Hide();

    }


}