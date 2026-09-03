using UnityEngine;
using UnityEngine.UI;

public class TestSignIn : MonoBehaviour 
{
    [SerializeField] string testUsername;
    [SerializeField] string testPassword;
    [SerializeField] InputContainer usernameInput;
    [SerializeField] InputSecretContainer passwordInput;
    [SerializeField] Button testInputBtn;



    void Awake()
    {
        testInputBtn.AddEvent(async () => await OnClickTestInputBtn());
    }

    async Awaitable OnClickTestInputBtn()
    {
        usernameInput.SetText(testUsername);
        passwordInput.SetText(testPassword);
        await PanelManager.Instance.signIn.OnClickSignInBtn();
    }
}