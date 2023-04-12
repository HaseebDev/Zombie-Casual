using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class HUDRating : BaseHUD
{
    public enum RATING_STATE
    {
        NONE,
        RATING,
        FEEDBACK,
        THANK_YOU
    }

    public TextMeshProUGUI _txtTitle;


    [Header("Rating")]
    public CanvasGroup _rectRating;
    public List<StarView> _listStarViews;

    [Header("Feedback")]
    public CanvasGroup _rectFeedback;
    public TMP_InputField _feedbackInput;

    [Header("Thanks View")]
    public CanvasGroup _rectThank;

    public static string HTTP_SHEET = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeKRh5YKoYKfdmPWr_9-uoTbcCrnnGxUKnDvBU5sal_osxEKQ/formResponse";

    public static string GOOGLE_RATING_URL = "https://play.google.com/store/apps/details?id=com.namnh.idlezombie";
    public static string APPLE_RATING_URL = "";


    private RATING_STATE _state = RATING_STATE.NONE;
    private CoroutineHandle _taskFeedback;

    private int _starCount = 0;

    public void SetState(RATING_STATE _state)
    {
        this._state = _state;
        _rectRating.gameObject.SetActiveIfNot(false);
        _rectFeedback.gameObject.SetActiveIfNot(false);
        _rectThank.gameObject.SetActiveIfNot(false);
        switch (_state)
        {
            case RATING_STATE.NONE:
                break;
            case RATING_STATE.RATING:
                _txtTitle.text = LOCALIZE_ID_PREF.LOVE_IDLE_ZOMBIE.AsLocalizeString();
                _starCount = 0;
                foreach (var item in _listStarViews)
                {
                    item.OnStarClick = OnStarClicked;
                    item.EnableStar(false);
                }

                _rectRating.gameObject.SetActiveIfNot(true);
                break;
            case RATING_STATE.FEEDBACK:
                _txtTitle.text = LOCALIZE_ID_PREF.GIVE_US_FEEDBACK.AsLocalizeString();
                _rectFeedback.gameObject.SetActiveIfNot(true);
                _feedbackInput.text = "";
                break;
            case RATING_STATE.THANK_YOU:
                _txtTitle.text = LOCALIZE_ID_PREF.THANK_YOU.AsLocalizeString();
                _rectThank.gameObject.SetActiveIfNot(true);
                SaveManager.Instance.Data.DayTrackingData.IsDoneRating = true;
                Timing.CallDelayed(1.0f, () =>
                {
                    Hide();
                });
                break;
            default:
                break;
        }
    }

    private void OnStarClicked(StarView star)
    {
        int starIndex = _listStarViews.IndexOf(star);
        for (int i = 0; i < _listStarViews.Count; i++)
        {
            bool enable = (i <= starIndex);
            _listStarViews[i].EnableStar(enable, enable);
        }

        if (_taskFeedback != null)
        {
            Timing.KillCoroutines(_taskFeedback);
            _taskFeedback = Timing.CallDelayed(0.3f, () =>
            {
                if ((_starCount + 1) >= 5)
                {
                    DoLinkToStore();
                }
                else
                {
                    SetState(RATING_STATE.FEEDBACK);
                }
            });
        }

        _starCount = starIndex;
    }

    public void OnButtonOkay()
    {
        switch (_state)
        {
            case RATING_STATE.NONE:
                break;
            case RATING_STATE.RATING:
                OnButtonRating();
                break;
            case RATING_STATE.FEEDBACK:
                OnButtonFeedback();
                break;
            default:
                break;
        }
    }

    public void OnButtonRating()
    {
        if ((_starCount + 1) >= 5)
        {
            DoLinkToStore();
        }
        else
        {
            SetState(RATING_STATE.FEEDBACK);
        }

    }

    private void DoLinkToStore()
    {
        string storeURL = "";
#if UNITY_ANDROID
        storeURL = GOOGLE_RATING_URL;
#elif UNITY_IOS
        storeURL = APPLE_RATING_URL;
#endif
        Application.OpenURL(storeURL);
        SetState(RATING_STATE.THANK_YOU);
    }

    public void OnButtonFeedback()
    {
        var feedback = _feedbackInput.text;
        if (!string.IsNullOrEmpty(feedback))
        {
            StartCoroutine(PostFeedback(HTTP_SHEET, feedback));
        }
        SetState(RATING_STATE.THANK_YOU);
    }

    IEnumerator PostFeedback(string URL, string feedback)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1669322313", SaveManager.Instance.Data.MetaData.UserID);
        form.AddField("entry.1918437853", SaveManager.Instance.Data.MetaData.UserName);
        form.AddField("entry.256694632", SaveManager.Instance.Data.GetPlayProgress(GameMode.CAMPAIGN_MODE).MaxLevel);
        form.AddField("entry.1052380910", SaveManager.Instance.Data.GetPlayProgress(GameMode.IDLE_MODE).MaxLevel);
        form.AddField("entry.1155093333", _starCount + 1);
        form.AddField("entry.1894633070", feedback);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (!www.isDone)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }



        //UnityWebRequest www = UnityWebRequest.Post(URL, form);
        //yield return www.SendWebRequest();

        //if (www.isDone)
        //{
        //    Debug.LogError($"PostFeedback successfully!");
        //}
        //else if (www.isHttpError)
        //{
        //    Debug.LogError($"PostFeedback error!: {www.responseCode} ");
        //}



    }

    public override void PreInit(EnumHUD type, IParentHud _parent, params object[] args)
    {
        base.PreInit(type, _parent, args);
        SetState(RATING_STATE.RATING);
    }


}
