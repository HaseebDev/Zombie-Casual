using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using com.datld.data;
using QuickType.Attribute;
using QuickType.Talent;
using UnityEngine;

namespace com.datld.talent
{
    public class ModelTalent
    {
        public static float hpIncrease = 0;
        public static float powerIncrease = 0;
        public static float recoverHPGameplay = 0;
        public static float rangeBlock = 0;
        public static float meleeBlock = 0;
        public static float fireRatePercentIncrease = 0;
        public static float hpRecoverAfterKillPercent = 0;
        public static float angleRageIncreasePercent = 0;
        public static float bonusGoldPercent = 0;
        public static float bonusDiamondShopPercent = 0;
        public static float bonusCritPercent = 0;
        public static bool hasWheel = false;
    }

    public static class TalentManager
    {
        private static RecoverHPAfterKillTalent _recoverHpAfterKillTalent = null;

        public static void Init()
        {
            _recoverHpAfterKillTalent = new RecoverHPAfterKillTalent();
        }

        public static List<Tuple<EffectType, float>> GetAttributeValue()
        {
            List<Tuple<EffectType, float>> result = new List<Tuple<EffectType, float>>();
            if (ModelTalent.hpIncrease != 0)
                result.Add(new Tuple<EffectType, float>(EffectType.PASSIVE_INCREASE_HP, ModelTalent.hpIncrease));
            if (ModelTalent.powerIncrease != 0)
                result.Add(new Tuple<EffectType, float>(EffectType.PASSIVE_INCREASE_DMG, ModelTalent.powerIncrease));
            if (ModelTalent.fireRatePercentIncrease != 0)
                result.Add(new Tuple<EffectType, float>(EffectType.PASSIVE_INCREASE_FIRERATE_PERCENT,
                    ModelTalent.fireRatePercentIncrease));
            if (ModelTalent.bonusCritPercent != 0)
                result.Add(new Tuple<EffectType, float>(EffectType.PASSIVE_INCREASE_CRIT, ModelTalent.bonusCritPercent));

            return result;
        }

        public static void AddTalentBonus(List<AttributeUI> attributeUIs)
        {
            foreach (var attributeUi in attributeUIs)
            {
                if (attributeUi.AttributeId == EffectType.PASSIVE_INCREASE_CRIT.ToString())
                {
                    // var old = attributeUi.Value;
                    attributeUi.Value += ModelTalent.bonusCritPercent;
                    attributeUi.Reload();

                    // Debug.LogError($"Old{old}, new {attributeUi.Value} ");
                }
            }
        }

        public static void UpdateFromSave()
        {
            foreach (var talentData in SaveManager.Instance.Data.Inventory.ListTalentData)
            {
                TalentID talentId = (TalentID)Enum.Parse(typeof(TalentID), talentData.TalentID, true);
                switch (talentId)
                {
                    case TalentID.STRENGTH:
                        ModelTalent.hpIncrease = talentData.TalentValue;
                        break;
                    case TalentID.POWER:
                        ModelTalent.powerIncrease = talentData.TalentValue;
                        break;
                    case TalentID.RECOVER:
                        ModelTalent.recoverHPGameplay = talentData.TalentValue;
                        break;
                    case TalentID.RANGE_BLOCK:
                        ModelTalent.rangeBlock = talentData.TalentValue;
                        break;
                    case TalentID.MELEE_BLOCK:
                        ModelTalent.meleeBlock = talentData.TalentValue;
                        break;
                    case TalentID.AGILITY:
                        ModelTalent.fireRatePercentIncrease = talentData.TalentValue;
                        break;
                    case TalentID.VAMPIRE:
                        ModelTalent.hpRecoverAfterKillPercent = talentData.TalentValue;
                        _recoverHpAfterKillTalent.UpdateValue(talentData.TalentValue);
                        break;
                    case TalentID.POWER_OF_ANGLE:
                        ModelTalent.angleRageIncreasePercent = talentData.TalentValue;
                        break;
                    case TalentID.HAND_OF_MIDAS:
                        ModelTalent.bonusGoldPercent = talentData.TalentValue;
                        break;
                    case TalentID.GREEDY:
                        ModelTalent.bonusDiamondShopPercent = talentData.TalentValue;
                        break;
                    case TalentID.CRITICAL_HIT:
                        ModelTalent.bonusCritPercent = talentData.TalentValue;
                        break;
                    case TalentID.WHEEL_OF_FORTUNE:
                        ModelTalent.hasWheel = talentData.TalentLevel != 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class BaseTalent
    {
        protected float _value;

        public void UpdateValue(float value)
        {
            _value = value;
        }

        public virtual void OnApply()
        {
        }
    }

    public class RecoverHPAfterKillTalent : BaseTalent
    {
        public RecoverHPAfterKillTalent()
        {
            OnApply();
        }

        public override void OnApply()
        {
            if (_value == 0)
                return;

            EventSystemServiceStatic.AddListener(this, EVENT_NAME.ZOMBIE_KILLED,
                new Action<KilledZombieInfoStruct>(OnZombieKilled));
        }

        public void OnZombieKilled(KilledZombieInfoStruct rwdData)
        {
            string heroID = rwdData.killerID;
            if (heroID != "" && _value != 0)
            {
                HeroData heroData = SaveManager.Instance.Data.GetHeroData(heroID);
                if (heroData != null)
                {
                    float hpRecover = heroData.BaseHeroPower.Hp * _value / 100f;
                    GamePlayController.instance.RefillHPGamePlay(hpRecover);
                }

            }
        }
    }
}