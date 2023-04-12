using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Security.Cryptography
{
	public static class AES
	{
		public static string Encrypt(byte[] value, string password)
		{
			byte[] bytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("ShMG8hLyZ7k~Ge5@")).GetBytes(AES.KeyLength / 8);
			ICryptoTransform transform = new RijndaelManaged
			{
				Mode = CipherMode.CBC,
				Padding = PaddingMode.Zeros
			}.CreateEncryptor(bytes, Encoding.UTF8.GetBytes("~6YUi0Sv5@|{aOZO"));
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(value, 0, value.Length);
					cryptoStream.FlushFinalBlock();
					cryptoStream.Close();
					memoryStream.Close();
					result = Convert.ToBase64String(memoryStream.ToArray());
				}
			}
			return result;
		}

		public static string Encrypt(string value, string password)
		{
			return AES.Encrypt(Encoding.UTF8.GetBytes(value), password);
		}

		public static string Decrypt(string value, string password)
		{
			byte[] array = Convert.FromBase64String(value);
			byte[] bytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("ShMG8hLyZ7k~Ge5@")).GetBytes(AES.KeyLength / 8);
			ICryptoTransform transform = new RijndaelManaged
			{
				Mode = CipherMode.CBC,
				Padding = PaddingMode.None
			}.CreateDecryptor(bytes, Encoding.UTF8.GetBytes("~6YUi0Sv5@|{aOZO"));
			string result;
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
				{
					byte[] array2 = new byte[array.Length];
					int count = cryptoStream.Read(array2, 0, array2.Length);
					memoryStream.Close();
					cryptoStream.Close();
					result = Encoding.UTF8.GetString(array2, 0, count).TrimEnd("\0".ToCharArray());
				}
			}
			return result;
		}

		public static int KeyLength = 128;

		private const string SaltKey = "ShMG8hLyZ7k~Ge5@";

		private const string VIKey = "~6YUi0Sv5@|{aOZO";
	}
}
