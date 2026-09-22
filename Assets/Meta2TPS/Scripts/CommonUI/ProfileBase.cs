using UnityEngine;
using UnityEngine.UI;

public class ProfileBase : MonoBehaviour 
{
    [SerializeField] Image avatarImage;
    [SerializeField] Text nicknameText;
    [SerializeField] DB db;



    void OnEnable()
    {
        nicknameText.text = Model.Instance.playerData.nickname;
        avatarImage.sprite = db.GetAvatar(Model.Instance.playerData.avatar);
    }

}