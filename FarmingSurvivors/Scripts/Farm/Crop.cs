using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Crop 
{
    public CropSetting cropSetting;
    public CropGradeSetting gradeSetting;
    public int count = 1;

    public Crop(SeedType seedType, SeedGrade grade, int count = 1)
    {
        cropSetting = DataManager.Instance.cropSettings.Where(c => c.seed == seedType).Last();
        gradeSetting = DataManager.Instance.cropGradeSettings.Where(c => c.grade == grade).Last();
        this.count = count;
    }

}
