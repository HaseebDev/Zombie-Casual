using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using TMPro;

public static class FBUtils
{
    public static T DeepClone<T>(this T toClone)
    {
        using (var stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, toClone);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    public static string FirstCharToUpper(this string input)
    {
        TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
        string output = cultInfo.ToTitleCase(input);
        return output;
    }

    public static void SetActiveIfNot(this GameObject go, bool enable)
    {
        if (go.activeSelf != enable)
            go.SetActive(enable);
    }

    public static bool CheckInSideRect(this Vector3 pos, Vector3 topleft, Vector3 botright)
    {
        bool inside = false;
        inside = (pos.x >= topleft.x && pos.x <= botright.x && pos.z >= topleft.z && pos.z <= botright.z);
        return inside;
    }

    public static Color HexToColor(string hex)
    {
        Color result = Color.white;
        ColorUtility.TryParseHtmlString(hex, out result);
        return result;
    }

    public static void SetColorAlpha(this Image img, float alpha)
    {
        var color = img.color;
        color.a = alpha;
        img.color = color;
    }

    public static void SetColorAlpha(this TextMeshProUGUI txt, float alpha)
    {
        var color = txt.color;
        color.a = alpha;
        txt.color = color;
    }

    public static void LookAtTarget(this Transform transform, Transform target)
    {
        Vector3 targetPostition = new Vector3(target.transform.position.x,
            transform.position.y,
            target.transform.position.z);
        transform.LookAt(targetPostition);
    }

    public static T AddComponentIfNot<T>(this GameObject go) where T : Component
    {
        T result = default(T);
        if (!go.TryGetComponent<T>(out result))
        {
            result = go.AddComponent<T>();
        }

        return result;
    }

    public static TEnum ParseEnum<TEnum>(this string strEnumValue, TEnum defaultValue) where TEnum : struct
    {
        TEnum enumValue;
        if (!Enum.TryParse(strEnumValue, true, out enumValue))
        {
            return defaultValue;
        }

        return enumValue;
    }

    public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
    {
        float totalWeight = sequence.Sum(weightSelector);
        // The weight we are after....
        float itemWeightIndex = (float)(new System.Random().NextDouble() * totalWeight);
        float currentWeightIndex = 0;

        foreach (var item in from weightedItem in sequence
                             select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
        {
            currentWeightIndex += item.Weight;

            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if (currentWeightIndex >= itemWeightIndex)
                return item.Value;
        }

        return default(T);
    }

    public static string RandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static void ResetParticle(this ParticleSystem par)
    {
        var pars = par.GetComponentsInChildren<ParticleSystem>();
        if (pars != null && pars.Length > 0)
        {
            foreach (var p in pars)
            {
                p.time = 0;
            }
        }

        par.time = 0;
    }

    public static void LookAtYAxis(this Transform transform, Vector3 targetPos)
    {
        Vector3 targetPostition = new Vector3(targetPos.x,
            transform.position.y,
            targetPos.z);
        transform.LookAt(targetPostition);
    }

    public static string CurrencyAddComma(float value)
    {
        return String.Format("{0:n0}", value);
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
            return Convert.ToDouble((double)value / Math.Pow(10.0, (double)num)).ToString(str + array[num / 3])
                .Replace('.', ',');
        }

        return value.ToString("#,0");
    }

    public static Vector3 RandomPointInBounds(this Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public static Transform FindChildRecursively(this Transform transform, string NameToFind)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(transform);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == NameToFind)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }

        return null;
    }

    public static T GetComponentInChildrenRecursively<T>(this GameObject go)
    {
        T result = default(T);
        Queue<GameObject> queue = new Queue<GameObject>();
        queue.Enqueue(go);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            result = c.GetComponent<T>();
            if (result != null)
                return result;
            foreach (Transform t in c.transform)
                queue.Enqueue(t.gameObject);
        }

        return result;
    }

    public static List<T> GetComponentsInChildrenRecursively<T>(this GameObject go)
    {
        List<T> result = new List<T>();
        Queue<GameObject> queue = new Queue<GameObject>();
        queue.Enqueue(go);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            var component = c.GetComponent<T>();
            if (component != null)
            {
                result.Add(component);
            }
            foreach (Transform t in c.transform)
                queue.Enqueue(t.gameObject);
        }

        return result;
    }

    public static void DestroyAllChilds(this GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static AnimationClip GetAnimationInfo(this Animator anim, string AnimClipName)
    {
        AnimationClip result = null;
        RuntimeAnimatorController ac = anim.runtimeAnimatorController; //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++) //For all animations
        {
            if (ac.animationClips[i].name == AnimClipName) //If it has the same name as your clip
            {
                result = ac.animationClips[i];
                break;
            }
        }

        return result;
    }

    public static bool IsInside(this Collider c, Vector3 point)
    {
        Vector3 closest = c.ClosestPoint(point);
        // Because closest=point if point is inside - not clear from docs I feel
        return Vector3.Distance(closest, point) <= 0.1f;
    }

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }

    public static string FormatStringSkill(this string skillID)
    {
        string result = skillID;

        var splits = skillID.Split('/');
        if (splits != null && splits.Length > 0)
            result = splits[0];

        return result;
    }

    public static float ParseFloatFromString(string s)
    {
        return float.Parse(s, CultureInfo.InvariantCulture);
    }


}

