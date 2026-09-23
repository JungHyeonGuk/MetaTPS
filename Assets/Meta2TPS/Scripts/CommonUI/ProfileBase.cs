using UnityEngine;
using UnityEngine.UI;

public class ProfileBase : MonoBehaviour 
{
    [SerializeField] Image avatarImage;
    [SerializeField] Text nicknameText;



    void OnEnable()
    {
        CacheManager.Instance.myPlayerDataChanged += OnMyPlayerDataChanged;
        OnMyPlayerDataChanged(CacheManager.Instance.myPlayerData);
    }

    void OnDisable()
    {
        if (CacheManager.Instance != null) 
        {
            CacheManager.Instance.myPlayerDataChanged -= OnMyPlayerDataChanged;
        }
    }

    async void OnMyPlayerDataChanged(PlayerData playerData)
    {
        nicknameText.text = playerData.nickname;
        avatarImage.sprite = await CacheManager.Instance.LoadAvatarAsync(playerData.avatarId);
    }

}