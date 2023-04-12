using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Ez.Pooly;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions;
using Random = UnityEngine.Random;

public class SpawnCollectAnim : MonoBehaviour
{
    public void SpawnCollectAnimation(Sprite sprite, int num, Transform destination, Transform holder, Vector3 position,
        float delayEachSpawn = 0.2f, int min = 10, int max = 50)
    {
        num = Mathf.Clamp(num, min, max);
        bool playedSoundCollect = false;

        float startDelay = 0f;

        var images = new List<Image>();

        for (int i = 0; i < num; i++)
        {
            var newstar = Pooly.Spawn<Image>(POOLY_PREF.COLLECT_ANIM_PREFAB, Vector3.zero, Quaternion.identity, holder);

            newstar.gameObject.SetActive(true);
            newstar.transform.position = position;
            newstar.sprite = sprite;
            images.Add(newstar);

            int index = i;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delayEachSpawn);
            sequence.AppendCallback(() =>
            {
                newstar.transform.localScale = Vector3.zero;
                newstar.transform.DOScale(Random.Range(1.7f, 1.9f), 0.1f).OnComplete(() =>
                {
                    newstar.transform.DOScale(Random.Range(1.4f, 1.7f), 0.1f);
                });
                newstar.transform.DOJump(
                    position + new Vector3(Utils.ConvertToMatchWidthRatio(Random.Range(-150, 150)),
                        Utils.ConvertToMatchHeightRatio(Random.Range(-150, 150)), 0),
                    Utils.ConvertToMatchHeightRatio(Random.Range(10, 100)),
                    Random.Range(1, 3),
                    0.2f);

                newstar.transform.DORotate(
                    newstar.transform.rotation.eulerAngles + new Vector3(0, 0, Random.Range(-20, 20)), 0.2f);
            });

            sequence.OnComplete(() =>
            {
                if (index == num - 1)
                {
                    for (int j = 0; j < images.Count; j++)
                    {
                        var star = images[j];
                        float waitToFly = 0.35f; //Random.Range(0.25f, 0.4f);
                        Sequence flySequence = DOTween.Sequence();
                        flySequence.AppendInterval(waitToFly);

                        // var tempIndex = j;
                        flySequence.OnComplete(() =>
                        {
                            // var curveHelper = star.GetComponent<CurveHelper>();
                            // if (curveHelper == null)
                            //     curveHelper = star.gameObject.AddComponent<CurveHelper>();

                            float time = 0.5f; //Random.Range(0.4f, 0.6f);
                            star.transform.DOScale(1f, time).SetEase(Ease.InQuad);
                            star.transform.DOMove(destination.position, time).OnComplete(() =>
                            {
                              
                                if(!playedSoundCollect)
                                {
                                    AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_COLLECT_COIN);
                                    playedSoundCollect = true;
                                }
                                
                                destination.DOScale(1.2f, 0.1f).OnComplete(() => { destination.DOScale(1f, 0.1f); });
                                Pooly.Despawn(star.transform);
                            });

                            //     curveHelper.CurveMove(destination.position, time,
                            //         1, () =>
                            //         {
                            //             destination.DOScale(1.2f, 0.1f).OnComplete(() =>
                            //             {
                            //                 destination.DOScale(1f, 0.1f);
                            //             });
                            //
                            //             Pooly.Despawn(star.transform);
                            //         });
                        });
                    }
                }
            });

            startDelay += delayEachSpawn;
        }
    }
}