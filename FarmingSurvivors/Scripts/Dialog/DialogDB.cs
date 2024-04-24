using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogDB : ScriptableObject
{
	public List<DialogContents> Intro; 
	public List<DialogContents> Chapter1; 
	public List<DialogContents> Chapter2;
	public List<DialogContents> Chapter3; 

	public Dictionary<string, List<DialogContents>> MakeDialogDict()
	{
		Dictionary<string, List<DialogContents>> newDict = new Dictionary<string, List<DialogContents>>();

		newDict.Add("0", Intro);
		newDict.Add("1", Chapter1);
		newDict.Add("2", Chapter2);
		newDict.Add("3", Chapter3);
		return newDict;
	}
}
