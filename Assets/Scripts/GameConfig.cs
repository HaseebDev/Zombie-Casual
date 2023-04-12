using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Create game config")]
public class GameConfig : ScriptableObject
{
	public string androidAppodealKey;

	public string iOSAppodealKey;

	public bool levelCheatPanelStatus;

	public int maxLevels;
}
