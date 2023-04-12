using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Ez.Pooly;
using QuickType.Shop;
using UnityEngine;
using UnityEngine.Events;

public static class MathUtils
{
    public static double CalculateArithmeticSequenceAtIndex(double firstValue, float step, int index)
    {
        return firstValue + (index - 1) * step;
    }

    public static double CalculateSumArithmeticSequenceAtIndex(double firstValue, float step, int index)
    {
        return index * (firstValue + CalculateArithmeticSequenceAtIndex(firstValue, step, index)) / 2;
    }
}

public static class Utils
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
    
    public static string ToTitleCase(this string title)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower()); 
    }
    
    public static string AsLocalizeString(this string localizeId)
    {
        return LocalizeController.GetText(localizeId);
    }

    public static T Remove<T>(this Stack<T> stack, T element)
    {
        T obj = stack.Pop();
        if (obj.Equals(element))
        {
            return obj;
        }
        else
        {
            T toReturn = stack.Remove(element);
            stack.Push(obj);
            return toReturn;
        }
    }

    public static int GetListenerNumber(this UnityEventBase unityEvent)
    {
        var field = typeof(UnityEventBase).GetField("m_Calls",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        var invokeCallList = field.GetValue(unityEvent);
        var property = invokeCallList.GetType().GetProperty("Count");
        return (int) property.GetValue(invokeCallList);
    }

    public static void DestroyAllChild(this Transform t)
    {
        foreach (Transform child in t)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static void DestroyAllChildImmediate(this Transform t)
    {
        foreach (Transform child in t)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public static void DespawnAllChild(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Pooly.Despawn(t.GetChild(i));
        }
    }


    public static T GetLastItem<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    public static T ToEnum<T>(this string value)
    {
        return (T) Enum.Parse(typeof(T), value, true);
    }

    public static CurrencyType ConvertToCurrencyType(this CostType type)
    {
        switch (type)
        {
            case CostType.NONE:
            case CostType.ADS:
            case CostType.FREE:
            case CostType.IAP:
                return CurrencyType.NONE;
            case CostType.DIAMOND:
                return CurrencyType.DIAMOND;
            case CostType.GOLD:
                return CurrencyType.GOLD;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static float ScreenWidthRatio => Screen.width / 1242f;
    public static float ScreenHeightRatio => Screen.height / 2688f;

    public static float ConvertToMatchWidthRatio(float num)
    {
        return num * ScreenWidthRatio;
    }

    public static float ConvertToMatchHeightRatio(float num)
    {
        return num * ScreenHeightRatio;
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }


    public static int HashStringToInt(string s)
    {
        MD5 md5Hasher = MD5.Create();
        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
        var ivalue = BitConverter.ToInt32(hashed, 0);
        return ivalue;
    }
}