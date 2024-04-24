using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeedType
{
    Peas, Corn, Cotton, Strawberry, Pear, Plump
}
[CreateAssetMenu]
public class CropSetting : ScriptableObject
{
    public SeedType seed = SeedType.Peas;
    public string cropName;
    public Equipment.Parts rewardParts;
    public Sprite seedSprite;
    public Sprite[] growUpSprites;

}
