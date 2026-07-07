using UnityEngine;

public abstract class PanelBase : MonoBehaviour 
{
    [SerializeField] protected GameObject mainPanel;


    public virtual void Init() 
    {
        
    }

    public virtual void Show()
    {
        mainPanel.SetActive(true);
    }
    
    public virtual void Hide()
    {
        mainPanel.SetActive(false);
    }
}