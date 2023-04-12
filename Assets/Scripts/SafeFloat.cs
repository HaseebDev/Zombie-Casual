using System;

namespace Framework.Security.SafeTypes
{
	public struct SafeFloat
	{
		public SafeFloat(float value = 0f)
		{
			Random random = new Random();
			this.offset = (float)random.Next(-1000, 1000);
			this.value = value + this.offset;
		}

		public float GetValue()
		{
			return this.value - this.offset;
		}

		public void Dispose()
		{
			this.offset = 0f;
			this.value = 0f;
		}

		public override string ToString()
		{
			return this.GetValue().ToString();
		}

		public static SafeFloat operator +(SafeFloat f1, SafeFloat f2)
		{
			return new SafeFloat(f1.GetValue() + f2.GetValue());
		}

		public static SafeFloat operator -(SafeFloat f1, SafeFloat f2)
		{
			return new SafeFloat(f1.GetValue() - f2.GetValue());
		}

		public static SafeFloat operator /(SafeFloat f1, SafeFloat f2)
		{
			return new SafeFloat(f1.GetValue() / f2.GetValue());
		}

		public static SafeFloat operator *(SafeFloat f1, SafeFloat f2)
		{
			return new SafeFloat(f1.GetValue() * f2.GetValue());
		}

		public static SafeFloat operator %(SafeFloat f1, SafeFloat f2)
		{
			return new SafeFloat(f1.GetValue() % f2.GetValue());
		}

		public static bool operator ==(SafeFloat f1, SafeFloat f2)
		{
			return (double)Math.Abs(f1.GetValue() - f2.GetValue()) < 0.01;
		}

		public static bool operator !=(SafeFloat f1, SafeFloat f2)
		{
			return (double)Math.Abs(f1.GetValue() - f2.GetValue()) > 0.01;
		}

		public static bool operator <(SafeFloat f1, SafeFloat f2)
		{
			return f1.GetValue() < f2.GetValue();
		}

		public static bool operator >(SafeFloat f1, SafeFloat f2)
		{
			return f1.GetValue() < f2.GetValue();
		}

		public static bool operator <=(SafeFloat f1, SafeFloat f2)
		{
			return f1.GetValue() <= f2.GetValue();
		}

		public static bool operator >=(SafeFloat f1, SafeFloat f2)
		{
			return f1.GetValue() >= f2.GetValue();
		}

		private float offset;

		private float value;
	}
}
