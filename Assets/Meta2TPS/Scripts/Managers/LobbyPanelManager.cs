using UnityEngine;

public class LobbyPanelManager : MonoSingleton<LobbyPanelManager> 
{
    [SerializeField] PanelBase[] panelBases;
    [SerializeField] GameObject[] lobbyObjects;



    public void ShowPanel(string panelName)
    {
        foreach (var panelBase in panelBases)
        {
            if (panelBase.name == panelName) 
            {
                panelBase.Show();
            }
            else
            {
                panelBase.Hide();
            }
        }
    }

    public void ShowLobbyObjects(bool isShow)
    {
        foreach (GameObject lobbyObject in lobbyObjects)
        {
            lobbyObject.SetActive(isShow);
        }
    }


    void Awake()
    {
        foreach (var panelBase in panelBases)
        {
            panelBase.Init();
        }

        ShowPanel("AuthPanel");
    }
}