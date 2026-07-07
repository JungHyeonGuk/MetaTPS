using UnityEngine;

public class PanelManager : MonoSingleton<PanelManager> 
{
    public AuthPanelController auth;
    public SignUpPanelController signUp;
    public SignInPanelController signIn;
    public LoadingPanelController loading;



    void Start()
    {
        auth.Init();
        signUp.Init();
        signIn.Init();
        loading.Init();

        auth.Show();
    }
}