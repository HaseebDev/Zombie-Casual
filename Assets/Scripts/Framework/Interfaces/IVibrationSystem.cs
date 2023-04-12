using System;

namespace Framework.Interfaces
{
	public interface IVibrationSystem
	{
		void Cancel();

		void Vibrate();

		void Vibrate(long milliseconds);

		void Vibrate(long[] pattern, int repeat);
	}
}
