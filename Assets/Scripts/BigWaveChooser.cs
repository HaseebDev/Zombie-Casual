using System;
using UnityEngine;

public class BigWaveChooser : MonoBehaviour
{
	public void ChooseNewBigWave(int currentWaveNumb, int maxWaveNumb)
	{
		this.numberOfBigWave = UnityEngine.Random.Range(1, maxWaveNumb - 1);
	}

	public bool isBigWave(int currentWaveNumb)
	{
		return currentWaveNumb == this.numberOfBigWave;
	}

	private int numberOfBigWave;
}
