using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public string nickname;
    public string avatarId;
}

[System.Serializable]
public class MapData 
{
    public string title;
    public string id;
}

[System.Serializable]
public class AvatarData 
{
    public string id;
}

public class Model : MonoSingleton<Model> 
{
    public string currentMapId;

}