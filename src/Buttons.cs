using UnityEngine;
using HarmonyLib;
using Il2Cpp;
using static Il2CppAK.SWITCHES;
using MelonLoader;

namespace FireAddons
{
    internal class FireAddonsButton
    {
        internal static GameObject coolFireBtnObj;

        internal static void Initialize(Panel_FeedFire panel_FeedFire)
        {
            if (panel_FeedFire == null) return;


            coolFireBtnObj = GameObject.Instantiate<GameObject>(panel_FeedFire.m_ActionButtonObject, panel_FeedFire.m_ActionButtonObject.transform.parent, true);
            coolFireBtnObj.transform.Translate(0.20f, 0.18f, 0);
            Utils.GetComponentInChildren<UILabel>(coolFireBtnObj).text = "Cool fire";

            // Pass null there, because will try to get from last interacted.
            Action act = new Action(() => CoolFire());
            AddAction(coolFireBtnObj, act);

            NGUITools.SetActive(coolFireBtnObj, true);
        }
        private static void AddAction(GameObject button, System.Action action)
        {
            Il2CppSystem.Collections.Generic.List<EventDelegate> placeHolderList = new Il2CppSystem.Collections.Generic.List<EventDelegate>();
            placeHolderList.Add(new EventDelegate(action));
            Utils.GetComponentInChildren<UIButton>(button).onClick = placeHolderList;
        }
        internal static void SetActive(bool active)
        {
            NGUITools.SetActive(coolFireBtnObj, active);
        }
        internal static void CoolFire()
        {
            Fire activeFire = InterfaceManager.GetPanel<Panel_FeedFire>().m_FireplaceInteraction.Fire;
            if (activeFire.m_HeatSource.m_MaxTempIncrease > Settings.options.waterTempRemoveDeg)
            {
                InterfaceManager.GetPanel<Panel_FeedFire>().m_FireplaceInteraction.Fire.ReduceHeatByDegrees(Settings.options.waterTempRemoveDeg);
            } 
        }
    }

    [HarmonyPatch(typeof(Panel_FeedFire), "Initialize")]
    internal class Panel_FeedFire_Initialize
    {
        private static void Postfix(Panel_FeedFire __instance)
        {
            //MelonLoader.MelonLogger.Msg("FeedFire_Initialize");
            FireAddonsButton.Initialize(__instance);
        }
    }

    [HarmonyPatch(typeof(Panel_FeedFire), "Enable")]
    internal class Panel_FeedFire_Enable
    {
        private static void Postfix(Panel_FeedFire __instance, bool enable)
        {
            //MelonLoader.MelonLogger.Msg("FeedFire_Enable");
            if (!enable) return;
            FireAddonsButton.SetActive(true);
        }
    }

}
