using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class StarChangeAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Transform _destination;
    [SerializeField] private Transform _starRewardBox;
    [SerializeField] private Transform _starHolder;

    [Button]
    public void Test()
    {
        SpawnStar(100);
    }
    
    public void Load()
    {
        int lastTotalStar = SaveManager.Instance.Data.StarData.LastTotalStar;
        int currentTotalStar = SaveGameHelper.GetTotalStar();
        if (lastTotalStar != currentTotalStar)
        {
            SpawnStar(currentTotalStar - lastTotalStar);
            SaveManager.Instance.Data.StarData.LastTotalStar = currentTotalStar;
        }
    }

    public void SpawnStar(int num)
    {
        float totalSpawnTime = 1;
        float delta = totalSpawnTime / num;
        
        List<Vector3> deltaPosition = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
        };

        if (num == 2)
        {
            deltaPosition = new List<Vector3>()
            {
                new Vector3(50, 0, 0),
                new Vector3(-50, 0, 0),
            };
        }
        else if (num == 3)
        {
            deltaPosition = new List<Vector3>()
            {
                new Vector3(0, 25, 0),
                new Vector3(-50, -25, 0),
                new Vector3(50, -25, 0),
            };
        }

        float delayEachSpawn = 0f;
        List<GameObject> stars = new List<GameObject>();

        for (int i = 0; i < num; i++)
        {
            var newstar = Instantiate(_starPrefab, _starHolder);
            stars.Add(newstar);

            int index = i;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delayEachSpawn);
            sequence.AppendCallback(() =>
            {
                newstar.SetActive(true);
                var position = _spawnPosition.position;
                newstar.transform.position = position;
                newstar.transform.localScale = Vector3.zero;
                newstar.transform.DOScale(Random.Range(1.9f, 2.3f), 0.1f).OnComplete(() =>
                {
                    newstar.transform.DOScale(2f, 0.1f);
                });
                if (index <= deltaPosition.Count - 1)
                {
                    newstar.transform.DOMove(position + deltaPosition[index], 0.2f);
                }
                else
                {
                    newstar.transform.DOMove(position + new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0),
                        0.2f);
                }

                newstar.transform.DORotate(
                    newstar.transform.rotation.eulerAngles + new Vector3(0, 0, Random.Range(-20, 20)), 0.2f);
            });

            sequence.OnComplete(() =>
            {
                if (index == num - 1)
                {
                    foreach (var star in stars)
                    {
                        float waitToFly = Random.Range(0.25f, 0.4f);
                        Sequence flySequence = DOTween.Sequence();
                        flySequence.AppendInterval(waitToFly);
                        flySequence.Append(star.transform.DOMove(_destination.position, Random.Range(0.25f, 0.4f)).SetEase(Ease.InQuad));
                        flySequence.OnComplete(() =>
                        {
                            _starRewardBox.DOKill();
                            _starRewardBox.DOScale(1.2f, 0.05f).OnComplete(() => { _starRewardBox.DOScale(1f, 0.05f); });
                            Destroy(star.gameObject);
                            AudioSystem.instance.PlaySFX(SFX_ENUM.SFX_COUNTING_STAR);
                        });
                    }

                }
            });

            delayEachSpawn += delta;
        }


    }
}

// sequence.OnComplete(() =>
// {
//     var curveHelper = newstar.AddComponent<CurveHelper>();
//     curveHelper.CurveMove(_destination.position, Random.Range(0.4f, 0.6f), index % 2 == 0 ? 1 : -1, () =>
//     {
//         _starRewardBox.DOKill();
//         _starRewardBox.DOScale(1.2f, 0.15f).OnComplete(() => { _starRewardBox.DOScale(1f, 0.15f); });
//         Destroy(newstar.gameObject);
//     });
// });