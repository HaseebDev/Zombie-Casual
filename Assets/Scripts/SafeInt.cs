using System;

namespace Framework.Security.SafeTypes
{
	public struct SafeInt
	{
		public SafeInt(int value = 0)
		{
			Random random = new Random();
			this.offset = random.Next(-1000, 1000);
			this.value = value + this.offset;
		}

		public int GetValue()
		{
			return this.value - this.offset;
		}

		public void Dispose()
		{
			this.offset = 0;
			this.value = 0;
		}

		public override string ToString()
		{
			return this.GetValue().ToString();
		}

		public static SafeInt operator +(SafeInt f1, SafeInt f2)
		{
			return new SafeInt(f1.GetValue() + f2.GetValue());
		}

		public static SafeInt operator -(SafeInt f1, SafeInt f2)
		{
			return new SafeInt(f1.GetValue() - f2.GetValue());
		}

		public static SafeInt operator /(SafeInt f1, SafeInt f2)
		{
			return new SafeInt(f1.GetValue() / f2.GetValue());
		}

		public static SafeInt operator *(SafeInt f1, SafeInt f2)
		{
			return new SafeInt(f1.GetValue() * f2.GetValue());
		}

		public static SafeInt operator %(SafeInt f1, SafeInt f2)
		{
			return new SafeInt(f1.GetValue() % f2.GetValue());
		}

		public static bool operator ==(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() == f2.GetValue();
		}

		public static bool operator !=(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() != f2.GetValue();
		}

		public static bool operator <(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() < f2.GetValue();
		}

		public static bool operator >(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() < f2.GetValue();
		}

		public static bool operator <=(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() <= f2.GetValue();
		}

		public static bool operator >=(SafeInt f1, SafeInt f2)
		{
			return f1.GetValue() >= f2.GetValue();
		}

		private int offset;

		private int value;
	}
}
