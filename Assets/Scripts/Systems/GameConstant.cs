using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstant
{

    public static string FLYING_AIR_DROP = "FLYING_AIR_DROP";
    public static string SCENE_MAIN_MENU = "HomeScene";
    public static string SCENE_GAME_PLAY = "GameCoreClone";
    public static string SCENE_LOADING = "LoadingScene";

    public static string ICON_GOLD = "icon_gold";
    public static string ICON_TOKEN = "icon_token";
    public static string ICON_PILL = "icon_pill";
    public static string ICON_SCRAP = "icon_scrap";

    public static int EQUIP_TYPE_WEAPON = 0;
    public static int EQUIP_TYPE_ARMOUR = 1;

    public static string IDLE_REWARD_PREFIX = "IDLE_REWARD_";
    public static string CAMPAIGN_REWARD_PREFIX = "CAMPAIGN_REWARD_";

    public static int MAXIMUM_RANK = 6;
    public static string CHEST_RARE_ID = "Chest_Rare";
    public static string CHEST_LEGENDARY_ID = "Chest_Legendary";
    public static string CHEST_LEGENDARY_TEN_ID = "Chest_Legendary_Ten";
    public static string NONE = "NONE";
    public static string MANUAL_HERO = "MANUAL_HERO";
    public static int MAX_HERO_SLOT = 4;
    public static string ADD_ON_SPEED_UP = "ADD_ON_SPEED_UP";
    public static string ADD_ON_AUTO_MANUAL_HERO = "ADD_ON_AUTO_MANUAL_HERO";
    public static string ADD_ON_AIR_DROP = "ADD_ON_AIR_DROP";

    public static string ADD_ON_MEDIC = "ADD_ON_MEDIC";
    public static string ADD_ON_RADIO_CALL = "ADD_ON_RADIO_CALL";
    public static string MAIL_SUPPORT_ADDRESS = "nhnpro.nguyenhoang@gmail.com";

    public static string MAX_COMPLETE_LEVEL_X2_REWARD_PER_DAY = "MAX_COMPLETE_LEVEL_X2_REWARD_PER_DAY";

    public static float SPAWN_ZOM_SPEED_MULTIPLIER = 2.2f;

    public static string IDLE_MODE_PILL_BASE = "IDLE_MODE_PILL_BASE";
    public static string IDLE_MODE_TOKEN_RETURN_PERCENT = "IDLE_MODE_TOKEN_RETURN_PERCENT";
    public static string ERROR_IAP_COST = "$???";
    public static string STEP_TO_SKIP_IDLE_LEVEL = "STEP_TO_SKIP_IDLE_LEVEL";

    public static string MAX_LEVEL_IDLE_MODE = "MAX_LEVEL_IDLE_MODE";

    public static string REVIVE_GEM_PRICE = "PRICE_REVIVE_DIAMOND";

    public static string MIN_COUNT_ZOMBIE_ANGRY = "MIN_COUNT_ZOMBIE_ANGRY";
    public static string MAX_COUNT_ZOMBIE_ANGRY = "MAX_COUNT_ZOMBIE_ANGRY";

    public static string POWER_SAVING_IDLE_DURATION = "POWER_SAVING_IDLE_DURATION";
}

public static class IAPConstant
{
    public static string idle_bundle = "idle_bundle";
    public static string remove_ads = "remove_ads";
    public static string hero_chest_x1 = "hero_chest_x1";
    public static string hero_chest_x5 = "hero_chest_x5";
}

public static class LOCALIZE_ID_PREF
{
    public static string BY_RARITY = "BY_RARITY";
    public static string BY_EQUIPMENT = "BY_EQUIPMENT";
    public static string BY_DPS = "BY_DPS";
    public static string PURCHASE_FAIL = "PURCHASE_FAIL";
    public static string WATCH_ADS_FAIL = "WATCH_ADS_FAIL";
    public static string EQUIP_MAX_RANK = "EQUIP_MAX_RANK";
    public static string UNLOCK_IN_LV = "UNLOCK_IN_LV";
    public static string WATCH_ADS_FAIL_INTERVAL = "WATCH_ADS_FAIL_INTERVAL";
    public static string NOT_AVAILABLE_NOW = "NOT_AVAILABLE_NOW";
    public static string MAX = "MAX";
    public static string FUSION_TO_UP_LV = "FUSION_TO_UP_LV";
    public static string PROMO_TO_UP_LV = "PROMO_TO_UP_LV";
    public static string NOT_ENOUGH_RESOURCES = "NOT_ENOUGH_RESOURCES";
    public static string GET_MORE_NOW = "GET_MORE_NOW";
    public static string UNLOCK_IN_LV_HERO = "UNLOCK_IN_LV_HERO";
    public static string EQUIPPED = "EQUIPPED";
    public static string MAX_LV = "MAX_LV";
    public static string NEW_ATTRIBUTE = "NEW_ATTRIBUTE";
    public static string UNLOCK_IN_RANK = "UNLOCK_IN_RANK";
    public static string FREE = "FREE";
    public static string GET_FREE = "GET_FREE";
    public static string FREE_DRAW = "FREE_DRAW";
    public static string LEVEL = "LEVEL";
    public static string NOT_READY = "NOT_READY";
    public static string ACTION_NOT_AVAILABLE = "ACTION_NOT_AVAILABLE";
    public static string COLLECT_MORE_TO_USE = "COLLECT_MORE_TO_USE";
    public static string BOOST_HERO_BASE_DMG = "BOOST_HERO_BASE_DMG";
    public static string NOT_ENOUGH_SHARD = "NOT_ENOUGH_SHARD";
    public static string FULL_HP = "FULL_HP";
    public static string COMING_SOON = "COMING_SOON";
    public static string WAVE = "WAVE";
    public static string THREE_STAR_REWARD = "THREE_STAR_REWARD";
    public static string LOVE_IDLE_ZOMBIE = "LOVE_IDLE_ZOMBIE";
    public static string GIVE_US_FEEDBACK = "GIVE_US_FEEDBACK";
    public static string GIVE_US_RATING = "GIVE_US_RATING";
    public static string THANK_YOU = "THANK_YOU";
}

public static class AnalyticsConstant
{
    public static int MAX_TRACKING_LEVEL = 41;
    public enum ANALYTICS_ENUM
    {
        NONE,
        TOUCH_CAMPAIGN_MODE,
        TOUCH_HOME,
        TOUCH_INVENTORY,
        TOUCH_SHOP,
        TOUCH_TALENT,
        TOUCH_EVENT,
        TOUCH_INGAME_TAB_HEROS,
        TOUCH_INGAME_TAB_WEAPON,
        TOUCH_INGAME_TAB_ADD_ON,
        TOUCH_INGAME_TAB_TOWER,
        USE_ADDON,
        TOUCH_INGAME_HEROSLOT,
        TOUCH_INGAME_HERO_AVATAR,
        TOUCH_FINISH_LEVEL_WATCH_ADS,
        TOUCH_FINISH_LEVEL_RETURN_HOME,
        TOUCH_INGAME_BTN_CHANGE_WEAPON,
        TOUCH_INGAME_BTN_NEW_WEAPON,
        TOUCH_HOME_BTN_TIME_REWARD,
        TOUCH_NEXT_LEVEL_BTN,


        //TUTORIAL TRACKING 
        TUTORIAL_TOUCH_MEDIC = 200,
        TUTORIAL_TOUCH_RADIO_CALL,


        //IAP EVENTS,
        IAP_PURCHASE_BEGIN = 300,
        IAP_PURCHASE_SUCCESS,
        IAP_PURCHASE_FAIL,
        PURCHASE_CHEST,
        PURCHASE_SHOP_ITEM,


        //PLAY SESSION TRACKING
        NEW_REGISTER_USER = 400,
        DAY_LOGIN,
        AVG_PLAY_TIME,
        DAY_LOGIN_CAMPAIGN_LEVEL


    }

    public static string getEventName(ANALYTICS_ENUM enumAnalytic)
    {
        string result = "";
        switch (enumAnalytic)
        {
            case ANALYTICS_ENUM.NONE:
                break;
            case ANALYTICS_ENUM.TOUCH_CAMPAIGN_MODE:
                result = "touch_btn_campaign_mode";
                break;
            case ANALYTICS_ENUM.TOUCH_HOME:
                result = "touch_btn_home";
                break;
            case ANALYTICS_ENUM.TOUCH_INVENTORY:
                result = "touch_btn_inventory";
                break;
            case ANALYTICS_ENUM.TOUCH_SHOP:
                result = "touch_btn_shop";
                break;
            case ANALYTICS_ENUM.TOUCH_TALENT:
                result = "touch_btn_talent";
                break;
            case ANALYTICS_ENUM.TOUCH_EVENT:
                result = "touch_btn_event";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_TAB_HEROS:
                result = "touch_ingame_tab_heros";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_TAB_WEAPON:
                result = "touch_ingame_tab_weapon";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_TAB_ADD_ON:
                result = "touch_ingame_tab_addon";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_TAB_TOWER:
                result = "touch_ingame_tab_tower";
                break;
            case ANALYTICS_ENUM.USE_ADDON:
                result = "use_addon";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_HEROSLOT:
                result = "touch_ingame_hero_slot";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_HERO_AVATAR:
                result = "touch_ingame_hero_avatar";
                break;
            case ANALYTICS_ENUM.TOUCH_FINISH_LEVEL_WATCH_ADS:
                result = "touch_finish_level_watch_ads";
                break;
            case ANALYTICS_ENUM.TOUCH_FINISH_LEVEL_RETURN_HOME:
                result = "touch_finish_level_return_home";
                break;
            case ANALYTICS_ENUM.TUTORIAL_TOUCH_MEDIC:
                result = "tutorial_touch_medic";
                break;
            case ANALYTICS_ENUM.TUTORIAL_TOUCH_RADIO_CALL:
                result = "tutorial_touch_radio_call";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_BTN_CHANGE_WEAPON:
                result = "touch_ingame_btn_change_weapon";
                break;
            case ANALYTICS_ENUM.TOUCH_INGAME_BTN_NEW_WEAPON:
                result = "touch_ingame_btn_weapon";
                break;
            case ANALYTICS_ENUM.TOUCH_HOME_BTN_TIME_REWARD:
                result = "touch_btn_time_reward";
                break;
            case ANALYTICS_ENUM.TOUCH_NEXT_LEVEL_BTN:
                result = "touch_next_level_btn";
                break;
            case ANALYTICS_ENUM.IAP_PURCHASE_BEGIN:
                result = "iap_purchase_begin";
                break;
            case ANALYTICS_ENUM.IAP_PURCHASE_SUCCESS:
                result = "iap_purchase_success";
                break;
            case ANALYTICS_ENUM.IAP_PURCHASE_FAIL:
                result = "iap_purchase_fail";
                break;
            case ANALYTICS_ENUM.NEW_REGISTER_USER:
                result = "new_register_user";
                break;
            case ANALYTICS_ENUM.DAY_LOGIN:
                result = "day_login";
                break;
            case ANALYTICS_ENUM.AVG_PLAY_TIME:
                result = "avg_play_time";
                break;
            case ANALYTICS_ENUM.PURCHASE_CHEST:
                result = "purchase_chest";
                break;
            case ANALYTICS_ENUM.PURCHASE_SHOP_ITEM:
                result = "purchase_shop_item";
                break;
            case ANALYTICS_ENUM.DAY_LOGIN_CAMPAIGN_LEVEL:
                result = "daylevel";
                break;
            default:
                Debug.LogError($"Cant get analytic event name {enumAnalytic}");
                break;
        }

        return result;
    }

    public static string getEventNameByLevel(ANALYTICS_ENUM enumAnalytic, int level)
    {
        return getEventName(enumAnalytic) + $"_level_{level}";
    }

}

public static class UnlockRequireId
{
    public static string ANGLE_UPGRADE_BUTTON = "ANGLE_UPGRADE_BUTTON";

    // public static string SHOW_SPECIAL_ADD_ON = "SHOW_SPECIAL_ADD_ON";
    // public static string ENABLE_SPECIAL_ADD_ON = "ENABLE_SPECIAL_ADD_ON";
    public static string TAB_ADD_ON = "TAB_ADD_ON";
    public static string SHOP_HERO_CHEST = "SHOP_HERO_CHEST";

}

public static class PlayerPrefKey
{
}

public static class AchieveConstant
{
    public static string HAS_PURCHASED_DIAMOND = "HAS_PURCHASED_DIAMOND";
    public static string HAS_PURCHASED_IAP = "HAS_PURCHASED_IAP";
}

public static class PLAYER_PREF
{
    public static string COUNT_LOGIN = "COUNT_LOGIN";
}

public static class RankConstant
{
    public static int COMMON = 1;
    public static int RARE = 2;
    public static int EPIC = 3;
    public static int LEGENDARY = 4;
    public static int IMMORTAL = 5;
    public static int UNIQUE = 6;
}

public static class TagConstant
{
    public static string TAG_ZOMBIE = "zombie";
    public static string TAG_BOSS = "boss";
    public static string TAG_ATTACKABLE_TRIGGEER = "attackableTrigger";
    public static string TAG_GROUND = "ground";
    public static string TAG_CLICKABLE = "clickable";
    public static string TAG_ZOM_OBJECT = "zombieObject";
    public static string TAG_PLAYER = "Player";
}

public static class NotificationID
{
    public static int reminder = 1;
    public static int chestRare = 2;
    public static int chestLegendary = 3;
    public static int freeStuff = 4;
}

public static class NotificationIconID
{
    public static string reminder_small_icon = "icon_0";

    public static string reminder_large_icon = "icon_1";
}

public static class POOLY_PREF
{
    //HUD
    public static string FLOATING_TEXT = "UIFloatingText";
    public static string FLOATING_TEXT_NOTIFY = "UIFloatingTextNotify";
    public static string FLOATING_SPRITE = "UIFlyingSprite";
    public static string COIN_3D = "Coin3D";
    public static string TOKEN_3D = "Token3D";
    public static string PLAYER_HEALTH = "PlayerHealth";
    public static string CASTLE_HEALTH = "CastleHealth";
    public static string REWARD_ITEM_VIEW = "RewardItemView";
    public static string REWARD_UI_CAMPAIGN = "RewardUICampaign";
    public static string ADD_ON_BTN_VIEW = "AddOnButtonView";
    public static string ADD_ON_AIR_DROP_BTN_VIEW = "AddOnAirDropButtonView";
    public static string EQUIPMENT_UI_VARIANT = "HUDEquipment EquipmentUI Variant";
    public static string HERO_BUTTON_UI = "HeroButtonUI";
    public static string SKILL_UI = "SkillUI";

    //INGAME
    public static string AIMING_DRAG = "DragAiming";
    public static string AIMING_PRICE_SHOT = "PierceShotAiming";
    public static string AIMING_PRICE_SHOT_3D = "PierceShotAiming3D";
    public static string SINGLE_GRENADE = "SingleGrenade";
    public static string APPLY_EFFECT_ZONE = "ApplyEffectZone";
    public static string LIGHTING_LINE = "LightningLine";
    public static string PIERCE_BULLET = "PierceBulet";
    public static string BOOST_VIEW_ICON = "BoostIconView";
    public static string HERO_BOOST_CANVAS = "HeroBoostCanvas";
    public static string AIR_DROP = "AirDrop";
    public static string DEFAULT_BULLET = "BulletPrefab";
    public static string RAY_BULLET = "RayBulletPrefab";
    public static string FLYING_AIR_DROP = "FlyingAirDrop";
    public static string CHANGE_WEAPON_BUTTON = "ChangeWeaponButton";
    public static string STAR_REWARD_UI = "StarRewardUI";
    public static string MISSION_ITEM_UI = "MissionItemUI";
    public static string MISSION_FINAL_ITEM_UI = "MissionFinalItemUI";
    public static string REWARD_UI = "RewardUI";


    //PARTICLES
    public static string FX_EXPLODE = "FX_Explode";
    public static string FX_LIGHTNING_IMPACT = "LightningImpact";
    public static string COLLECT_ANIM_PREFAB = "CollectAnimPrefab";
    public static string EQUIPMENT_UI_HUD_EQUIPMENT = "EquipmentUI";
    public static string EQUIPMENT_UI_HUD_FUSION = "EquipmentFusionUI";
    public static string LEVEL_PROGRESS_ITEM = "LevelProgressItem";
    public static string SHADOW_BETWEEN_FULL_EQUIP = "ShadowBetween";

    public static string[] PRELOAD_HUD = new[]
        {
            "HUDShop",
            "HUDEquipment"
        };

    public static string GetHUDPrefabByType(EnumHUD hudType)
    {
        string result = "";
        switch (hudType)
        {
            case EnumHUD.NONE:
                break;
            case EnumHUD.HUD_MISSION:
                result = "HUDMission";
                break;
            case EnumHUD.HUD_FUSION_RESULT:
                result = "HUDFusionResult";
                break;
            case EnumHUD.HUD_REMOVE_ADS:
                result = "HUDRemoveAds";
                break;
            case EnumHUD.HUD_SELECT_CAMPAGIN_LEVEL:
                result = "HUDSelectCampaignLevel";
                break;
            case EnumHUD.HUD_SETTING:
                result = "HUDSetting";
                break;
            case EnumHUD.HUD_MAIL:
                result = "HUDMail";
                break;
            case EnumHUD.HUD_DAILY_FREE_STUFF:
                result = "HUDDailyFreeStuff";
                break;
            case EnumHUD.HUD_LOADING_GAME:
                result = "HUDGameLoading";
                break;
            case EnumHUD.HUD_HOME:
                result = "HUDHomeMenu";
                break;
            case EnumHUD.HUD_LOADING:
                result = "HUDLoading";
                break;
            case EnumHUD.HUD_SHOP:
                result = "HUDShop";
                break;
            case EnumHUD.HUD_IDLE_OFFLINE_REWARD:
                result = "HUDOfflineIncomePanel";
                break;
            case EnumHUD.HUD_EQUIPMENT:
                result = "HUDEquipment";
                break;
            case EnumHUD.HUD_TALENT:
                result = "HUDResearch";
                break;
            case EnumHUD.HUD_HERO:
                result = "HUDHero";
                break;
            case EnumHUD.HUD_COMPLETE_LEVEL_REWWARD:
                result = "HUDCompleteLevelReward";
                break;
            case EnumHUD.HUD_NOT_ENOUGH:
                result = "HUDNotEnough";
                break;
            case EnumHUD.HUD_ASCEND:
                result = "HUDAscend";
                break;
            case EnumHUD.HUD_REWARD_SIMPLE:
                result = "HUDRewardSimple";
                break;
            case EnumHUD.HUD_FUSION:
                result = "HUDFusion";
                break;
            case EnumHUD.HUD_NOTIFY:
                result = "HUDNotify";
                break;
            case EnumHUD.HUD_REVIVE_DEFEAT:
                result = "HUDReviveDefeat";
                break;
            case EnumHUD.HUD_AIR_DROP:
                result = "HUDAirDrop";
                break;
            case EnumHUD.HUD_SIMULATE_ADS:
                result = "HUDSimulateAds";
                break;
            case EnumHUD.HUD_MERGE_CONFLICT:
                result = "HUDMergeConflictData";
                break;
            case EnumHUD.HUD_NO_INTERNET:
                result = "HUDNoInternet";
                break;
            case EnumHUD.HUD_SHARD_CHAR:
                result = "HUDShardChar";
                break;
            case EnumHUD.HUD_ADD_ON_INFO:
                result = "HUDSpecialAddOnInfo";
                break;
            case EnumHUD.HUD_IDLE_REWARD:
                result = "HUDIdleReward";
                break;
            case EnumHUD.HUD_ASK_EXIT:
                result = "HUDAskExit";
                break;
            case EnumHUD.HUD_PURCHASE_HERO_INGAME:
                result = "HUDPurchaseHeroInGame";
                break;
            case EnumHUD.HUD_HERO_UNLOCK:
                result = "HUDHeroUnlock";
                break;
            case EnumHUD.HUD_STAR_REWARD:
                result = "HUDStarReward";
                break;
            case EnumHUD.HUD_IDLE_SKIP_LEVEL:
                result = "HUDIdleSkipLevel";
                break;
            case EnumHUD.HUD_PREVIEW_CAMPAIGN_LEVEL:
                result = "HUDLevelPreview";
                break;
            case EnumHUD.HUD_POWER_SAVING_MODE:
                result = "HUDPowerSaving";
                break;
            case EnumHUD.HUD_RATING:
                result = "HUDRating";
                break;
            default:
                break;
        }

        return result;
    }
}

public static class EVENT_NAME
{
    public static string GAME_STARTED = "GAME_STARTED";
    public static string ZOMBIE_KILLED = "ZOMBIE_KILLED";
    public static string ZOMBIE_IN_ATTACK_ZONE = "ZOMBIE_IN_ATTACK_ZONE";
    public static string ZOMBIE_LEAVE_ATTACK_ZONE = "ZOMBIE_LEAVE_ATTACK_ZONE";
    public static string LEVEL_FAILED = "BOSS_LOOSE";
    public static string LEVEL_COMPLETE = "BOSS_WIN";
    public static string EXP_GROW = "EXP_GROW";
    public static string EXP_FALL = "EXP_FALL";
    public static string WAVE_STARTED = "WAVE_STARTED";

    public static string ZOMBIE_WAVE_STARTED = "ZOMBIE_WAVE_STARTED";
    public static string LEVEL_READY_UP = "LEVEL_READY_UP";
    public static string LEVEL_UP = "LEVEL_UP";
    public static string UNLOAD_LEVEL = "UNLOAD_LEVEL";
    public static string LEVEL_LOADED = "LEVEL_LOADED";
    public static string LEVEL_UP_BTN_CLICKED = "LEVEL_UP_BTN_CLICKED";
    public static string CHOOSE_LOCATION_CLICKED = "CHOOSE_LOCATION_CLICKED";
    public static string CHARACTERS_CHANGED = "CHARACTERS_CHANGED";
    public static string LOCATION_CHANGED = "LOCATION_CHANGED";
    public static string LOADING_FINISHED = "LOADING_FINISHED";

    public static string RESET_GAME_HUD = "RESET_GAME_HUD";

    public static string HERO_UPGRADED = "HERO_UPGRADED";
    public static string RESET_TEAM_SLOTS = "RESET_TEAM_SLOTS";

    public static string ON_TOUCH_GROUND = "ON_TOUCH_GROUND";
    public static string ON_TOUCH_ZOMBIE = "ON_TOUCH_ZOMBIE";

    public static string CASTLE_DEFEATED = "CAMPAIGN_MODE_DEFEAT";

    public static string UPDATE_CASTLE_HP = "UPDATE_CASTLE_HP";
    public static string UPDATE_IDLE_HUD = "UPDATE_IDLE_HUD";
    public static string UPDATE_CURRENCY = "UPDATE_CURRENCY";
    public static string CLAIM_MISSION = "CLAIM_MISSION";


    public static string SET_ULTIMATE_BUTTON_REACHED_LIMIT = "RESET_ULTIMATE_BUTTON_REACHED_LIMIT";
    public static string SET_ULTIMATE_BUTTON_COUNTDOWN = "RESET_ULTIMATE_BUTTON";
    public static string REVIVE_CAMPAIGN_BATTLE = "REVIVE_CAMPAIGN_BATTLE";
    public static string REPLAY_CAMPAIGN_BATTLE = "REPLAY_CAMPAIGN_BATTLE";
    public static string RESET_UPGRADE_BUTTON_VIEW = "RESET_UPGRADE_BUTTON_VIEW";

    public static string RESET_CAMPAIGN_DATA = "RESET_CAMPAIGN_DATA";

    public static string SET_ULTIMATE_BUTTON_TIMER = "SET_ULTIMATE_BUTTON_TIMER";
    public static string SET_ULTIMATE_BUTTON_READY_USE = "SET_ULTIMATE_BUTTON_READY_USE";

    public static string UPDATE_USER_INFO_VIEW = "UPDATE_USER_INFO_VIEW";

    public static string UPDATE_USER_INFO_ID = "UPDATE_USER_INFO_ID";
    public static string RESET_HERO_INFO_VIEW = "RESET_HERO_INFO_VIEW";

    public static string CHEAT_DISABLE_OBJECT = "CHEAT_DISABLE_OBJECT";

    //GAMEPLAY VIEW UPDATE HUD!!!!
    public static string UPDATE_GP_ZOM_HPBAR = "UPDATE_GP_ZOM_HPBAR";
    public static string RESET_HUD_HOME = "RESET_HUD_HOME";

    public static string RESET_HERO_PREVIEW = "RESET_HERO_PREVIEW";
    public static string GAME_LOADED = "GAME_LOADED";
    public static string PURCHASE_HERO = "PURCHASE_HERO";

    public static string RESET_IDLE_REWARD_ITEM_DATA = "RESET_IDLE_REWARD_ITEM_DATA";
    public static string ADD_ADON = "ADD_ADON";
    public static string ON_REMOVE_ADS = "ON_REMOVE_ADS";
    public static string CHANGE_DAILY_FREE_STUFF = "CHANGE_DAILY_FREE_STUFF";
    public static string CLAIM_STAR_REWARD = "CLAIM_STAR_REWARD";
    public static string ADD_SHARD_HERO = "ADD_SHARD_HERO";
    public static string ON_PROMOTE_HERO = "ON_PROMOTE_HERO";
    public static string ENABLE_DEBUG_HERO_PANEL = "ENABLE_DEBUG_HERO_PANEL";
}

public enum COMMON_FX
{
    NONE = 0,
    FX_BLOOD,
    FX_ZOMBIE_EXPLODE,
    FX_EXPLODE_BOOM_01,
    FX_BLOOD_FOR_BOSS,
    FX_EXPLODE_METAL,
    FX_EXPLODE_GREEN,


    //hit effects
    FX_STUNNED = 100,
    FX_PARASHUTE_SLAM,
    FX_MUSIC_NOTE,
    FX_BURNING_AREA_MINI,

    //SKILL EFFECTS
    FX_LIGHTNING_STRIKE = 300,

}
