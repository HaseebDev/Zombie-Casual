using System;
using System.Security.Cryptography;
using System.Text;
using Framework.Parsing;
using UnityEngine;

namespace Framework.Utility
{
    public static class Converter
    {
        public static Color StringToColor(string s)
        {
            if (s == "")
            {
                return Color.gray;
            }
            int num = Convert.ToInt32(s, 16);
            int num2 = num >> 16 & 255;
            int num3 = num >> 8 & 255;
            int num4 = num & 255;
            return new Color((float)num2 / 255f, (float)num3 / 255f, (float)num4 / 255f);
        }

        public static JSON TextToJSON(string txt)
        {
            if (txt == null || txt == "")
            {
                return null;
            }
            return new JSON
            {
                serialized = txt
            };
        }

        public static string SecondsToTimeString(int seconds)
        {
            int num = seconds / 60;
            int num2 = seconds % 60;
            return string.Format("{0:00}", num) + ":" + string.Format("{0:00}", num2);
        }

        public static string Md5Sum(string strToEncrypt)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(strToEncrypt);
            byte[] array = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string text = "";
            for (int i = 0; i < array.Length; i++)
            {
                text += Convert.ToString(array[i], 16).PadLeft(2, '0');
            }
            return text.PadLeft(32, '0');
        }

        public static string CurrencyConvert(long value)
        {
            string[] array = new string[]
            {
                "",
                "k",
                "M",
                "B",
                "T",
                "aa",
                "bb",
                "cc",
                "dd",
                "ee",
                "ff",
                "gg",
                "hh",
            };
            int num = 0;
            while (Math.Pow(10.0, (double)num) < (double)(value + 1L))
            {
                num += 3;
            }
            num -= 3;
            string str;
            if (num <= 3)
            {
                str = "0.##";
            }
            else
            {
                str = "0.###";
            }
            if (num >= 3)
            {
                return Convert.ToDouble((double)value / Math.Pow(10.0, (double)num)).ToString(str + array[num / 3]).Replace('.', ',');
            }
            return value.ToString("#,0");
        }

        public static byte[] StringToBytesArray(string str)
        {
            byte[] array = new byte[str.Length * 2];
            Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
            return array;
        }

        public static string BytesArrayToString(byte[] bytes)
        {
            char[] array = new char[bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
            return new string(array);
        }
    }
}
