using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public enum SeedGrade
{
    Normal, Rare, SuperRare, Unique
}

[CreateAssetMenu]
public class CropGradeSetting : ScriptableObject
{
    public SeedGrade grade;
    public List<Sprite>seedPackImages;
    public Sprite seedPackImage;
    public int maxGrowthValue = 100;
    public int minGold;
    public int maxGold;
    public SerializedDictionary<Equipment.Grade,float> rewardRates;

}
