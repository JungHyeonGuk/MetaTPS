using UnityEngine;

public abstract class PanelBase : MonoBehaviour 
{
    [SerializeField] protected GameObject mainPanel;

    bool isInitShow = false;



    public virtual void Init() 
    {
        
    }

    public virtual void InitShow() 
    {

    }

    public virtual void Show()
    {
        if (!isInitShow) 
        {
            InitShow();
            isInitShow = true;
        }

        mainPanel.SetActive(true);
    }
    
    public virtual void Hide()
    {
        mainPanel.SetActive(false);
    }
}