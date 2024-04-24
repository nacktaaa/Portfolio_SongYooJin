using System;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Converters;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Util;
using System.Globalization;
using Equipment;

public class SaveManager : SingletoneBehaviour<SaveManager>
{
	public const string equipmentIdKey = "gotItemAmount";
	public const string mountedKey = "mounted";
	public const string equipInventoryKey = "equipInventory";
	public const string seedInventoryKey = "seedInventory";
	public const string goldKey = "Gold";
	public const string FarmKey = "Farm";
	public const string TimeKey = "Time";
	public const string GrowthPointKey = "GrowthPoint";
	public const string AdKey = "Ad";
	public const string PackageKey = "Package";
	public const string SkillsKey = "Skills";

	DateTime currentTime;
	public int PackagePurchased = 0;
	JsonSerializerSettings jsonSerializerSettings;

	public void Init()
	{
		jsonSerializerSettings = new JsonSerializerSettings
		{
			Converters = { new StringEnumConverter { AllowIntegerValues = true } },
			NullValueHandling = NullValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			Error = (sender, args) =>
			{
				if (args.ErrorContext.Error.InnerException is JsonSerializationException)
				{
					args.ErrorContext.Handled = true;
				}
			}
		};
		Load();
	}

	private void Load()
	{
		LoadEquipId();
		LoadGold();
		LoadMounted();
		LoadEquipInventory();
		LoadSeeds();
		LoadGrowthPoint();
		LoadAd();
		LoadTime();
		LoadPackage();
	}
	private void LoadGold()
	{
		int curgold = PlayerPrefs.GetInt(goldKey);
		GoldManager.Instance.CurrentGold = curgold;
	}
	private void LoadEquipInventory()
	{
		if (PlayerPrefs.HasKey(equipInventoryKey))
		{
			EquipManager.Instance.inventory.LoadInventory(PlayerPrefs.GetString(equipInventoryKey));
		}
	}
	private void LoadMounted()
	{
		if (PlayerPrefs.HasKey(mountedKey))
		{
			EquipManager.Instance.inventory.LoadMounted(PlayerPrefs.GetString(mountedKey));
		}
	}
	private void LoadEquipId()
	{
		int equipID = PlayerPrefs.GetInt(equipmentIdKey);
		EquipManager.Instance.equipID = equipID;
	}
	private void LoadSeeds()
	{
		if (PlayerPrefs.HasKey(seedInventoryKey))
		{
			FarmManager.Instance.LoadSeeds(PlayerPrefs.GetString(seedInventoryKey));
		}
	}
	private void LoadGrowthPoint()
	{
		FarmManager.Instance.LoadGrowthPoint(PlayerPrefs.GetInt(GrowthPointKey));
	}
	public void LoadTime()
	{
		if(!PlayerPrefs.HasKey(TimeKey))
			return;
			
		currentTime = DateTime.Now;
		var lastTime = DateTime.ParseExact(PlayerPrefs.GetString(TimeKey), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		if (currentTime.Day != lastTime.Day||currentTime.Year != lastTime.Year || currentTime.Month != lastTime.Month)
		{
			DateChanged();
		}
		SaveTime();
	}
	public void LoadAd()
	{
		AdManager.LoadAdWatched(PlayerPrefs.GetInt(AdKey));
	}
	public void LoadPackage(){
		PackagePurchased = PlayerPrefs.GetInt(PackageKey);
	}

	public Dictionary<int, int> LoadSkills()
	{
		if(PlayerPrefs.HasKey(SkillsKey))
		{
			var mySkills = JsonConvert.DeserializeObject<Dictionary<int, int>>(PlayerPrefs.GetString(SkillsKey),
			new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			});

			return mySkills;
		}

		return null;
	}

	private void DateChanged()
	{
		Managers.DateChanged();
	}
	public void Save()
	{
		SaveGold();
		SaveEquipInventory();
		SaveMounted();
		SaveEquipId();
		SaveSeeds();
		SaveTime();
		SavePackage();
		SaveAd();
		SaveFarm();
	}
	public void SaveGold()
	{
		PlayerPrefs.SetInt(goldKey, GoldManager.Instance.CurrentGold);
		PlayerPrefs.Save();
	}
	public void SaveSkills()
	{
		if(GameManager.Instance.player != null)
		{
			Dictionary<int, int> mySkills = new Dictionary<int, int>();
			foreach(var s in GameManager.Instance.player.currentWeapons)
				mySkills.Add(s.stat.id, s.stat.currentLevel);
			
			var json = JsonConvert.SerializeObject(mySkills, Formatting.Indented, jsonSerializerSettings);
			PlayerPrefs.SetString(SkillsKey, json);
			PlayerPrefs.Save();
		}
	}
	public void SaveEquipInventory()
	{
		var equipInventory = EquipManager.Instance.inventory.equipmentsInventory;
		var json = JsonConvert.SerializeObject(equipInventory, Formatting.Indented, jsonSerializerSettings);
		PlayerPrefs.SetString(equipInventoryKey, json);
		PlayerPrefs.Save();
	}
	public void SaveMounted()
	{
		var mounted = EquipManager.Instance.inventory.mountedEquipments;
		string json = JsonConvert.SerializeObject(mounted, Formatting.Indented, jsonSerializerSettings);
		PlayerPrefs.SetString(mountedKey, json);
		PlayerPrefs.Save();
	}
	public void SaveEquipId()
	{
		PlayerPrefs.SetInt(equipmentIdKey, EquipManager.Instance.equipID);
		PlayerPrefs.Save();
	}
	public void SaveSeeds()
	{
		var farmInvendata = new FarmInvenSaveData();
		farmInvendata.SaveFarmInvenData();
		var json = JsonConvert.SerializeObject(farmInvendata, Formatting.Indented, jsonSerializerSettings);
		PlayerPrefs.SetString(seedInventoryKey, json);
		PlayerPrefs.Save();
	}

	public void SaveFarm()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			var farmdata = new FarmSaveData();
			farmdata.SaveFarmData();

			var json = JsonConvert.SerializeObject(farmdata, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
			PlayerPrefs.SetString(FarmKey, json);
		}
		else
		{
			PlayerPrefs.SetInt(GrowthPointKey, FarmManager.Instance.growthPoint);
		}
		PlayerPrefs.Save();
	}
	public void SaveTime()
	{
		PlayerPrefs.SetString(TimeKey, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
	}
	public void SaveAd()
	{
		PlayerPrefs.SetInt(AdKey, AdManager.adWatched);
	}
	public void SavePackage(){
		PlayerPrefs.SetInt(PackageKey,PackagePurchased);
	}

	public void DeleteStageData()
	{
		PlayerPrefs.DeleteKey("Map");
		PlayerPrefs.DeleteKey(SkillsKey);
	}
	public void OnPurchasePackage(){
		PackagePurchased = 1;
		SavePackage();
	}
	public void OnApplicationQuit()
	{
		Save();
	}
}

