using UnityEngine;


[System.Serializable]
public class PlayerData 
{
    public string nickname;
    public string avatar;
}

public class Model : MonoSingleton<Model> 
{
    public PlayerData playerData;
}