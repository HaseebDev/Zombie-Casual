using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterUltimateButton : MonoBehaviour
{
    public Image _imgIcon;
    public Image _imgFilled;
    public Image _imgIcon2;
    public Image _imgBG;
    public UIEffect _uiEffectHolder;
    public UIEffect _uiEffectIcon;
    public UIEffect _uiEffectBG;

    public UIShiny _uiShiny;
    public ParticleSystem _psEffect;

    private SoliderUpgradeButton _parent;
    private string _UltimateID;

    private float timerCountDown = 0f;
    private float CountDownDuration = 0f;
    private bool startCountDown = false;

    private float timerPlayShiny = 0f;
    private float TimeCastSkill = 0f;
    public const float SHINY_DURATION = 0.8f;

    private bool isPlayedShiny = false;

    private void Awake()
    {
        EventSystemServiceStatic.AddListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN,
            new Action<string, float, float>(SetCountDown));
    }

    private void OnDestroy()
    {
        EventSystemServiceStatic.RemoveListener(this, EVENT_NAME.SET_ULTIMATE_BUTTON_COUNTDOWN,
            new Action<string, float, float>(SetCountDown));
    }

    private void DoPlayEffectShiny(bool play)
    {
        if (play)
        {
            if (!isPlayedShiny)
            {
                _psEffect.Play();
                _uiShiny.Play();
                isPlayedShiny = true;
            }
        }
        else
        {
            _psEffect.Stop();
            _uiShiny.Stop();
            isPlayedShiny = false;
        }

    }

    private void SetCountDown(string skillID, float timeCastSkill, float countDownDuration)
    {
        if (skillID == _UltimateID)
        {
            if (countDownDuration <= 0)
            {
                startCountDown = false;
                DoPlayEffectShiny(true);
                timerPlayShiny = 0f;
                _imgFilled.fillAmount = 0f;

            }
            else
            {
                CountDownDuration = countDownDuration;
                startCountDown = true;
                TimeCastSkill = timeCastSkill;
                _imgFilled.fillAmount = 1.0f;

                DoPlayEffectShiny(false);
            }

        }
    }

    public void Initialize(SoliderUpgradeButton parent, string SKILL_ID)
    {
        _parent = parent;
        _UltimateID = SKILL_ID;

        var icon = ResourceManager.instance.GetFullUltimateSprite(_UltimateID);
        if (icon != null)
        {
            // _imgIcon.sprite = icon;
            ResourceManager.instance.GetSprite(icon.spriteAddress, s =>
            {
                _imgIcon2.sprite = s;
            });

            ResourceManager.instance.GetSprite(icon.bgAddress, s =>
            {
                _imgBG.sprite = s;
                _imgBG.gameObject.SetActive(s != null);
            });
        }

        DoPlayEffectShiny(false);
        _imgFilled.fillAmount = 0f;
    }

    public void UpdateButton(float _deltaTime)
    {
        if (startCountDown)
        {
            timerCountDown = Time.time - TimeCastSkill;
            var percent = 1.0f - (timerCountDown * 1.0f / CountDownDuration * 1.0f);
            _imgFilled.fillAmount = percent;

            if (timerCountDown >= CountDownDuration)
            {
                startCountDown = false;
                DoPlayEffectShiny(true);
                timerPlayShiny = 0f;
                _imgFilled.fillAmount = 0f;
            }
        }
        else //IS READY FOR USE
        {
            DoPlayEffectShiny(true);
            //timerPlayShiny += _deltaTime;
            //if (timerPlayShiny >= SHINY_DURATION)
            //{
            //    _uiShiny.Play(false);
            //    timerPlayShiny = 0f;
            //}
        }
    }

    public void OnPointerDown()
    {
        if (startCountDown)
            return;
        //Debug.Log("OnPointerDown!!!");
        _parent._character?.PointerDownUltimate(_UltimateID, Input.mousePosition);
        transform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerUp()
    {
        if (startCountDown)
        {
            MasterCanvas.CurrentMasterCanvas.ShowFloatingTextNotify(LOCALIZE_ID_PREF.NOT_READY
                .AsLocalizeString()); // "Not ready!");
            return;
        }

        //Debug.Log("OnPointerUp!!!");
        _parent._character?.PointerUpUltimate(_UltimateID, Input.mousePosition);
        transform.localScale = Vector3.one;
    }
}