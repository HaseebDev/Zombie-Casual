using System;
using Framework.Security.Cryptography;
using Framework.Utility;
using UnityEngine;

namespace Framework.Security
{
    public static class SafePlayerPrefs
    {
        public static string GetString(string _key, string _defValue = "")
        {
            string @string = PlayerPrefs.GetString(_key, _defValue);
            if (@string == _defValue)
            {
                return _defValue;
            }
            return AES.Decrypt(@string, SafePlayerPrefs.secretKey);
        }

        public static void SetString(string _key, string _value)
        {
            string value = AES.Encrypt(Converter.StringToBytesArray(_value), SafePlayerPrefs.secretKey);
            PlayerPrefs.SetString(_key, value);
        }

        public static int GetInt(string _key, int _defValue = 0)
        {
            return int.Parse(SafePlayerPrefs.GetString(_key, _defValue.ToString()));
        }

        public static void SetInt(string _key, int _value)
        {
            SafePlayerPrefs.SetString(_key, _value.ToString());
        }

        public static float GetFloat(string _key, float _defValue = 0f)
        {
            return FBUtils.ParseFloatFromString(SafePlayerPrefs.GetString(_key, _defValue.ToString()));
        }

        public static void SetFloat(string _key, float _value)
        {
            SafePlayerPrefs.SetString(_key, _value.ToString());
        }

        private static readonly string secretKey = "zFrqoln5";
    }
}
