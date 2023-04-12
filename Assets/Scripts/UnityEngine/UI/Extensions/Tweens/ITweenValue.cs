using System;

namespace UnityEngine.UI.Extensions.Tweens
{
	internal interface ITweenValue
	{
		void TweenValue(float floatPercentage);

		bool ignoreTimeScale { get; }

		float duration { get; }

		bool ValidTarget();

		void Finished();
	}
}
