using UnityEngine;

public class LoadingPanelController : PanelBase 
{
    [SerializeField] Transform logo;



    void Update() 
    {
        logo.Rotate(Vector3.forward, 100 * Time.deltaTime);
    }
}