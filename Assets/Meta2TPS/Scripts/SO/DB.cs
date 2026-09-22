using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DB", menuName = "Scriptable Objects/DB")]
public class DB : ScriptableObject
{
    public Color[] colors;
    public Sprite[] avatars;



    public Sprite GetAvatar(string name)
    {
        return Array.Find(avatars, avatar => avatar.name == name);
    }
}
