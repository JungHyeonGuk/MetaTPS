using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoSingleton<SystemManager> 
{
    [SerializeField] PanelBase[] panelBases;



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


    void Awake()
    {
        foreach (var panelBase in panelBases)
        {
            panelBase.Init();
        }

        ShowPanel("AuthPanel");
    }
}