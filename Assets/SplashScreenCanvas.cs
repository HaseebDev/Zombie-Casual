using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;
using UnityEngine.UI;
using DG.Tweening;

public class SplashScreenCanvas : MonoBehaviour
{
    public CanvasGroup _splashScreenImage;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _splashScreenImage.gameObject.SetActiveIfNot(true);
        _splashScreenImage.alpha = 1;

        GoToLoadingScene();
    }

    private void GoToLoadingScene()
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        LoadScene(GameConstant.SCENE_LOADING, (success) =>
        {
            Timing.CallDelayed(0.5f, () =>
            {
                _splashScreenImage.DOFade(0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _splashScreenImage.gameObject.SetActiveIfNot(false);
                    Destroy(this.gameObject);
                });
            });
        });
    }

    #region Scene Utils

    public void LoadScene(string sceneName, Action<bool> callback)
    {
        StartCoroutine(LoadSceneAsync(sceneName, callback));
    }

    IEnumerator LoadSceneAsync(string sceneName, Action<bool> callback)
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (asyncOperation.isDone)
            callback?.Invoke(true);
    }

    #endregion

}
