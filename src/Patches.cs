using HarmonyLib;
using UnityEngine;
using Il2Cpp;
using Il2CppTLD.Gear;
using MelonLoader;
using Il2CppEpic.OnlineServices;
using static FireAddons.Patches;


namespace FireAddons
{
    internal static class Patches
    {


        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
        internal class GameManager_Awake
        {
            public static void Prefix()
            {
                if (!InterfaceManager.IsMainMenuEnabled())
                {
                    FireAddons.LoadData();
                }
            }
        }


        [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake), null)]
        public class GearItem_Awake
        {
            private static void Postfix(GearItem __instance)
            {
                FireAddons.ModifyFirestarters(__instance);
            }
        }

        [HarmonyPatch(typeof(Panel_FeedFire), nameof(Panel_FeedFire.OnFeedFire))]
        internal class Panel_FeedFire_OnFeedFire
        {
            private static void Postfix(Panel_FeedFire __instance)
            {
                if (Settings.options.embersSystem) { FireAddons.FeedFireEmbers(__instance); };
                if (Settings.options.burnCharcoal) { FireAddons.FeedFireCharoal(__instance); };
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.Update))]
        internal class Fire_Update_Prefix
        {
            private static void Prefix(Fire __instance)
            {
                // the GetTimeof a day is neede as during scene chnage update is firing before full load
                if (!GameManager.m_IsPaused && Settings.options.embersSystem && (GameManager.GetTimeOfDayComponent().GetSecondsPlayedUnscaled() != 0f))
                {
                    FireAddons.CalculateEmbers(__instance);

                }
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.TurnOff))]
        internal class Fire_Turnoff_Postfix
        {
            private static void Postfix(Fire __instance)
            {
                if (!GameManager.m_IsPaused && Settings.options.burnCharcoal)
                {
                    FireAddons.CalculateCharcoal(__instance);
                }
            }

        }

        [HarmonyPatch(typeof(FireManager), nameof(FireManager.CalculateFireStartSuccess))]
        static class FireManager_CalculateFireStartSuccess
        {
            private static void Postfix(FireManager __instance, float __result)
            {
                FuelSourceItem tinder = InterfaceManager.GetPanel<Panel_FireStart>().GetSelectedTinder();
                if (Settings.options.tinderMatters && tinder)
                {
                    __result += FireAddons.GetModifiedFireStartSkillModifier(tinder);
                    __result = Mathf.Clamp(__result, 0f, 100f);
                }
            }
        }

        [HarmonyPatch(typeof(Panel_FireStart), nameof(Panel_FireStart.RefreshChanceOfSuccessLabel))]
        static class Panel_FireStart_RefreshChanceOfSuccessLabel
        {
            private static void Postfix(Panel_FireStart __instance)
            {
                if (Settings.options.tinderMatters)
                {
                    FuelSourceItem tinder = __instance.GetSelectedTinder();
                    if (tinder)
                    {
                        float num = float.Parse(__instance.m_Label_ChanceSuccess.text.Replace("%", ""));
                        num += FireAddons.GetModifiedFireStartSkillModifier(tinder);
                        num = Mathf.Clamp(num, 0f, 100f);
                        __instance.m_Label_ChanceSuccess.text = num.ToString("F0") + "%";
                        __instance.m_DirtyLabels = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.ExitFireStarting))]
        static class Fire_ExitFireStarting
        {
            private static void Postfix(Fire __instance, ref bool success)
            {
                if (success)
                {
                    // if tinder matters is on, add fuelvalue to burntime on start
                    // if not, but embers is enabled, add vanilla 500s
                    if (Settings.options.tinderMatters)
                    {
                        __instance.m_MaxOnTODSeconds += Settings.options.tinderFuel * 60f;
                    }
                    else if (Settings.options.embersSystem)
                    {
                        __instance.m_MaxOnTODSeconds += 500f;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Panel_FireStart), nameof(Panel_FireStart.RefreshList))]
        static class PatchPanel_FireStart_RefreshList
        {
            private static void Prefix(Panel_FeedFire __instance)
            {

                Inventory inventoryComponent = GameManager.GetInventoryComponent();
                foreach (GearItemObject item in inventoryComponent.m_Items)
                {
                    GearItem gearItem = item;
                    if ((bool)gearItem)
                    {
                        if (Settings.options.burnCharcoal && gearItem.name == "GEAR_Charcoal")
                        {
                            FireAddons.ModifyCharcoal(gearItem);
                        }
                        FuelSourceItem fuelSourceItem = gearItem.m_FuelSourceItem;
                        if ((bool)fuelSourceItem)
                        {
                            if (Settings.options.embersSystem && (gearItem.name.ToLower().Contains("recycledcan") || gearItem.name.ToLower().Contains("cookingpot")))
                            {
                                FireAddons.ModifyWater(gearItem, false);
                            }
                        }
                    }
                }

            }
        }
        // based on Fire_RV mod by Deus131
        [HarmonyPatch(typeof(Panel_FeedFire), nameof(Panel_FeedFire.RefreshFuelSources))]
        internal static class Panel_FeedFire_RefreshFuelSources
        {
            private static void Prefix(Panel_FeedFire __instance)
            {
                Inventory inventoryComponent = GameManager.GetInventoryComponent();
                foreach (GearItemObject item in inventoryComponent.m_Items)
                {
                    GearItem gearItem = item;
                    if ((bool)gearItem)
                    {
                        FuelSourceItem fuelSourceItem = gearItem.m_FuelSourceItem;
                        if ((bool)fuelSourceItem)
                        {
                            if (Settings.options.tinderAsFuel && FireAddons.IsNamedTinder(gearItem))
                            {
                                FireAddons.ModifyTinder(gearItem);
                            }
                        }
                        if (Settings.options.burnCharcoal && gearItem.name == "GEAR_Charcoal")
                        {
                            FireAddons.ModifyCharcoal(gearItem);
                        }
                    }
                }
            }

            private static void Postfix(Panel_FeedFire __instance)
            {

                Inventory inventoryComponent = GameManager.GetInventoryComponent();
                foreach (GearItemObject item in inventoryComponent.m_Items)
                {
                    GearItem gearItem = item;
                    if ((bool)gearItem)
                    {
                        FuelSourceItem fuelSourceItem = gearItem.m_FuelSourceItem;
                        if ((bool)fuelSourceItem)
                        {
                            if (FireAddons.IsNamedTinder(gearItem))
                            {
                                gearItem.m_FuelSourceItem.m_IsTinder = true;
                            }
                        }
                    }

                }
            }

        }

        [HarmonyPatch(typeof(Panel_FireStart), nameof(Panel_FireStart.FilterItemFuelSource))]
        internal static class Panel_FireStart_FilterItemFuelSource
        {
            private static bool Prefix(Panel_FireStart __instance, GearItem gi, ref bool __result)
            {
                FuelSourceItem fuelSourceItem = gi.GetComponent<FuelSourceItem>();
                bool temp = !fuelSourceItem.m_IsTinder;
                if (Settings.options.tinderAsFuel)
                {
                    temp = true;
                }
                __result = !(fuelSourceItem == null) && temp;
                return false;
            }
        }
        [HarmonyPatch(typeof(Panel_FeedFire), nameof(Panel_FeedFire.FilterItemFuelSource))]
        internal static class Panel_FeedFire_FilterItemFuelSource
        {
            private static bool Prefix(Panel_FeedFire __instance, GameObject go, ref bool __result)
            {
                FuelSourceItem fuelSourceItem = go.GetComponent<FuelSourceItem>();
                bool temp = !fuelSourceItem.m_IsTinder;
                if (Settings.options.tinderAsFuel)
                {
                    temp = true;
                }
                __result = !(fuelSourceItem == null) && temp;
                return false;
            }
        }

        [HarmonyPatch(typeof(FireplaceInteraction), nameof(FireplaceInteraction.GetHoverText))]
        internal static class FireplaceInteraction_GetHoverText
        {
            public static void Postfix(WoodStove __instance, ref string __result)
            {
                if (!__instance.Fire.m_IsPerpetual && __result != null && Settings.options.embersSystem && __instance.Fire.GetFireState() == FireState.FullBurn && __instance.Fire.m_EmberDurationSecondsTOD > 0 && __instance.Fire.m_EmberTimer >= 0)
                {

                    float emberDiff = __instance.Fire.m_EmberDurationSecondsTOD - __instance.Fire.m_EmberTimer;
                    int emberH = (int)Mathf.Floor(emberDiff / 3600);
                    int emberM = (int)((emberDiff - (emberH * 3600)) / 60);

                    // those spaces are needed for 'canvas' as it's calculated based on 1st line lenght
                    string[] input = __result.Split('\n');
                    if (__instance.Fire.m_EmberTimer == 0)
                    {

                        __result = "      " + input[0] + "      \n" + input[1] + " & " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + input[2] + ")";
                    } else
                    {
                        __result = "      " + input[0] + "      \n" + Localization.Get("GAMEPLAY_Embers") + ": " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + __instance.Fire.GetHeatIncreaseText() + ")";
                    }
                }

            }
        }

        [HarmonyPatch(typeof(CookingPotItem), nameof(CookingPotItem.AttachedFireIsBurning))]
        internal static class CookingPotItem_AttachedFireIsBurning
        {
            public static void Postfix(CookingPotItem __instance, ref bool __result)
            {
                if (__instance.m_FireBeingUsed && __instance.m_FireBeingUsed.m_IsPerpetual) { return; } 
                if (Settings.options.embersSystem && Settings.options.embersSystemNoCooking && __result && __instance.m_FireBeingUsed?.m_EmberTimer > 0)
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(Fire), nameof(Fire.IsBurning))]
        internal static class Fire_IsBurning
        {
            public static void Postfix(Fire __instance, ref bool __result)
            {
                if (__instance.m_IsPerpetual) { return; }
                if (Settings.options.embersSystem && Settings.options.embersSystemNoCooking && __result && __instance?.m_EmberTimer > 0)
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(CookingPotItem), nameof(CookingPotItem.UpdateCookingTimeAndState))]
        internal static class CookingPotItem_UpdateCookingTimeAndState {
            public static void Prefix(CookingPotItem __instance, ref float cookTimeMinutes, ref float readyTimeMinutes)
            {
                if (Settings.options.cookingSystem && __instance.m_FireBeingUsed)
                {
                    cookTimeMinutes *= FireAddons.CookSpeedFromTemp(__instance.m_FireBeingUsed.GetCurrentTempIncrease());
                    readyTimeMinutes *= FireAddons.CookSpeedFromTemp(__instance.m_FireBeingUsed.GetCurrentTempIncrease());
                    //MelonLogger.Msg("Cooking speed2: " + cookTimeMinutes + " " + readyTimeMinutes + " " + FireAddons.CookSpeedFromTemp(__instance.m_FireBeingUsed.GetCurrentTempIncrease()));
                }
            }

        }
    }
}