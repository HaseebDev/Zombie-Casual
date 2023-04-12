using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class StarHelper : MonoBehaviour
{
    public GameObject startPrefab;

    [SerializeField] private List<GameObject> stars;

    private List<SFX_ENUM> listSoundStars = new List<SFX_ENUM>()
        {SFX_ENUM.SFX_1STAR, SFX_ENUM.SFX_2STAR, SFX_ENUM.SFX_3STAR};

    [Button]
    public void FindReference()
    {
        stars = new List<GameObject>();
        
        foreach (Transform child in transform)
        {
           stars.Add(child.GetChild(0).gameObject);
        } 
    }

    [Button]
    public void PlayTestAnim()
    {
        PlayAnimSingle(2);
    }
    
    public void Load(int num)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].SetActive(i < num);
        }
    }

    public void LoadWithAnim(int num)
    {
        Timing.RunCoroutine(PlayAnimStarCoroutine(num));
    }

    public void PlayAnimSingle(int index)
    {
        Timing.RunCoroutine(PlayAnimSingleCoroutine(index));
    }

    private IEnumerator<float> PlayAnimSingleCoroutine(int index)
    {
        AudioSystem.instance.PlaySFX(listSoundStars[0]);
        yield return Timing.WaitForSeconds(0.1f);
        stars[index].gameObject.SetActiveIfNot(true);
        stars[index].transform.localScale = Vector3.one * 6f;

        stars[index].transform.DOScale(1, 0.2f).SetEase(Ease.Linear);
        yield return Timing.WaitForSeconds(0.2f);
        stars[index].transform.localScale = Vector3.one;

        stars[index].transform.DOScale(2f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
        yield return Timing.WaitForSeconds(0.03f);
    }

    IEnumerator<float> PlayAnimStarCoroutine(int num)
    {
        float delay = 0.5f;
        ResetAllStars(num);
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < num)
            {
                int index = i;
                //yield return Timing.WaitForSeconds(0.03f);
                var soundIndex = index % listSoundStars.Count;
                AudioSystem.instance.PlaySFX(listSoundStars[soundIndex]);
                yield return Timing.WaitForSeconds(0.1f);
                stars[index].gameObject.SetActiveIfNot(true);
                stars[index].transform.localScale = Vector3.one * 2f;

                stars[index].transform.DOScale(1, 0.1f).SetEase(Ease.Linear);
                yield return Timing.WaitForSeconds(0.1f);
                stars[index].transform.localScale = Vector3.one;

                stars[index].transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
                yield return Timing.WaitForSeconds(0.03f);
            }
            else
            {
                stars[i].SetActive(false);
            }
        }
    }

    private void ResetAllStars(int num)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < num)
            {
                int index = i;
                stars[i].transform.localScale = Vector3.zero;
                stars[i].SetActive(false);
            }
            else
            {
                stars[i].SetActive(false);
            }
        }
    }
}