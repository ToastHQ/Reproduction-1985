using System;
using UnityEngine;

[Serializable]
public class CharacterSelector
{
    public string characterName;
    public int currentCostume;
    public CharacterCostume[] allCostumes;

    public void SwapCharacter(int costumeIndex)
    {
        currentCostume = costumeIndex;
    }
}

[Serializable]
public class CharacterCostume
{
    //This is if a costume has a different rig.
    public enum CostumeT
    {
        Costume,
        Retrofit,
        Innards
    }

    public string costumeName;
    public string costumeDesc;
    public Sprite costumeIcon;
    public string yearOfCostume;
    public CostumeT costumeType;
    public Vector3 offsetPos;
}