using UnityEngine;

public class PanelManager : MonoSingleton<PanelManager> 
{
    public AuthPanelController auth;
    public SignUpPanelController signUp;
    public SignInPanelController signIn;
    public LoadingPanelController loading;
    public HomePanelController home;
    public ProfilePanelController profile;
    public AvatarEditPanelController avatarEdit;
    public ProfileEditPanelController profileEdit;
    public DisplaynameEditPanelController displaynameEdit;
    public IntroduceEditPanelController introduceEdit;
    public ContentDetailPanelController contentDetail;



    void Start()
    {
        auth.Init();
        signUp.Init();
        signIn.Init();
        loading.Init();
        home.Init();
        profile.Init();
        avatarEdit.Init();
        profileEdit.Init();
        displaynameEdit.Init();
        introduceEdit.Init();
        contentDetail.Init();
        
        auth.Show();
    }
}