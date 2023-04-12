using System;

public struct LevelProgressData
{
	public LevelProgressData(float _exp, float _maxExp, int _level)
	{
		this.exp = _exp;
		this.maxExp = _maxExp;
		this.level = _level;
	}

	public float exp;

	public float maxExp;

	public int level;
}
