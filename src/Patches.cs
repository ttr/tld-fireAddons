using Harmony;
using System;
using UnityEngine;
using MelonLoader;

namespace FireAddons
{
    internal static class Patches
    {

        [HarmonyPatch(typeof(GearItem), "Awake", null)]
        public class GearItem_Awake
        {
            private static void Postfix(GearItem __instance)
            {
                FireAddons.ModifyFirestarters(__instance);
            }
        }
        [HarmonyPatch(typeof(GameManager), "Awake")]
        public class GameManager_Awake
        {
            private static void Postfix()
            {
                FireAddons.Blueprints();
            }
        }
        [HarmonyPatch(typeof(Panel_Crafting), "ItemPassesFilter")]
        private static class RecipesInToolsRecipes
        {
            internal static void Postfix(Panel_Crafting __instance, ref bool __result, BlueprintItem bpi)
            {
                if (bpi?.m_CraftedResult?.name == "GEAR_FlintAndSteel" && __instance.m_CurrentCategory == Panel_Crafting.Category.FireStarting)
                {
                    __result = true;
                }
            }
        }
        /*
                    [HarmonyPatch(typeof(BlueprintDisplayItem), "Setup")]
                    private static class FixScrapMetalRecipeIcon
                    {
                        internal static void Postfix(BlueprintDisplayItem __instance, BlueprintItem bpi)
                        {
                            if (bpi?.m_CraftedResult?.name == "GEAR_FlintAndSteel")
                            {
                                Texture2D scrapMetalTexture = Utils.GetCachedTexture(SCRAP_METAL_CRAFTING_ICON_NAME);
                                if (!scrapMetalTexture)
                                {
                                    scrapMetalTexture = TinCanImprovementsMod.assetBundle.LoadAsset(SCRAP_METAL_CRAFTING_ICON_NAME).Cast<Texture2D>();
                                    Utils.CacheTexture(SCRAP_METAL_CRAFTING_ICON_NAME, scrapMetalTexture);
                                }
                                __instance.m_Icon.mTexture = scrapMetalTexture;
                            }
                        }
                    }
        */

        [HarmonyPatch(typeof(Panel_FeedFire), "OnFeedFire")]
        internal class Panel_FeedFire_OnFeedFire
        {
            private static void Postfix(Panel_FeedFire __instance)
            {
                FireAddons.FeedFire(__instance);
            }
        }

        [HarmonyPatch(typeof(Fire), "Update")]
        internal class Fire_Update_Prefix
        {
            private static void Prefix(Fire __instance)
            {
                if (!GameManager.m_IsPaused && Settings.options.embersSystem)
                {
                    FireAddons.CalculateEmbers(__instance);
                }
            }

        }

        [HarmonyPatch(typeof(FireManager), "PlayerStartFire")]
        static class FireManager_PlayerStartFire
        {
            private static void Prefix(FuelSourceItem tinder, ref float percentChanceSuccess)
            {
                if (Settings.options.tinderMatters && tinder)
                {
                    percentChanceSuccess += FireAddons.GetModifiedFireStartSkillModifier(tinder);
                    percentChanceSuccess = Mathf.Clamp(percentChanceSuccess, 0f, 100f);
                }
            }
        }

        [HarmonyPatch(typeof(Panel_FireStart), "RefreshChanceOfSuccessLabel")]
        static class Panel_FireStart_RefreshChanceOfSuccessLabel
        {
            private static void Postfix(Panel_FireStart __instance)
            {
                if (Settings.options.tinderMatters)
                {
                    __instance.m_FireManager = GameManager.GetFireManagerComponent();
                    FuelSourceItem tinder = __instance.GetSelectedTinder();
                    if (tinder)
                    {
                        float num = float.Parse(__instance.m_Label_ChanceSuccess.text.Replace("%", ""));
                        num += FireAddons.GetModifiedFireStartSkillModifier(tinder);
                        num = Mathf.Clamp(num, 0f, 100f);
                        __instance.m_Label_ChanceSuccess.text = num.ToString("F0") + "%";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Fire), "ExitFireStarting")]
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

        [HarmonyPatch(typeof(Panel_FireStart))]
        [HarmonyPatch("RefreshList")]
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

        // load and save custom data
        [HarmonyPatch(typeof(SaveGameSystem), "RestoreGlobalData", new Type[] { typeof(string) })]
        internal class SaveGameSystemPatch_RestoreGlobalData
        {
            internal static void Postfix(string name)
            {
                FireAddons.LoadData(name);
            }
        }

        [HarmonyPatch(typeof(SaveGameSystem), "SaveGlobalData", new Type[] { typeof(SaveSlotType), typeof(string) })]
        internal class SaveGameSystemPatch_SaveGlobalData
        {
            public static void Postfix(SaveSlotType gameMode, string name)
            {
                FireAddons.SaveData(gameMode, name);
            }
        }


        // based on Fire_RV mod by Deus131
        [HarmonyPatch(typeof(Panel_FeedFire), "RefreshFuelSources")]
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
                        if (Settings.options.embersSystem && (gearItem.name.ToLower().Contains("recycledcan") || gearItem.name.ToLower().Contains("cookingpot")))
                        {
                            FireAddons.ModifyWater(gearItem, true);
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

        [HarmonyPatch(typeof(Campfire), "GetHoverText")]
        internal static class Campfire_GetHoverText
        {
            private static void Postfix(Campfire __instance, ref string __result)
            {

                if (Settings.options.embersSystem && __instance.m_Fire.GetFireState() != FireState.Off && __instance.m_Fire.m_EmberDurationSecondsTOD > 0 && __instance.m_Fire.m_EmberTimer >= 0)
                {
                    float emberDiff = __instance.m_Fire.m_EmberDurationSecondsTOD - __instance.m_Fire.m_EmberTimer;
                    int emberH = (int)Mathf.Floor(emberDiff / 3600);
                    int emberM = (int)((emberDiff - (emberH * 3600)) / 60);
                    // those spaces are needed for 'canvas' as it's calculateb based on 1st line lenght
                    if (__instance.m_Fire.m_EmberTimer == 0)
                    {
                        string[] input = __result.Split('\n');
                        __result = "      " + __instance.m_LocalizedDisplayName.Text() + "      \n" + input[1] + " & " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + input[2] + ")";
                    }
                    else
                    {
                        __result = "      " + __instance.m_LocalizedDisplayName.Text() + "      \n" + Localization.Get("GAMEPLAY_Embers") + ": " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + __instance.m_Fire.GetHeatIncreaseText() + ")";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(WoodStove), "GetHoverText")]
        internal static class WoodStove_GetHoverText
        {
            public static void Postfix(WoodStove __instance, ref string __result)
            {
                if (Settings.options.embersSystem && __instance.m_Fire.GetFireState() != FireState.Off && __instance.m_Fire.m_EmberDurationSecondsTOD > 0 && __instance.m_Fire.m_EmberTimer >= 0)
                {

                    float emberDiff = __instance.m_Fire.m_EmberDurationSecondsTOD - __instance.m_Fire.m_EmberTimer;
                    int emberH = (int)Mathf.Floor(emberDiff / 3600);
                    int emberM = (int)((emberDiff - (emberH * 3600)) / 60);

                    // those spaces are needed for 'canvas' as it's calculateb based on 1st line lenght
                    if (__instance.m_Fire.m_EmberTimer == 0)
                    {
                        string[] input = __result.Split('\n');
                        __result = "      " + __instance.m_LocalizedDisplayName.Text() + "      \n" + input[1] + " & " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + input[2] + ")";
                    } else
                    {
                        __result = "      " + __instance.m_LocalizedDisplayName.Text() + "      \n" + Localization.Get("GAMEPLAY_Embers") + ": " + emberH.ToString() + "h " + emberM.ToString() + "m\n(" + __instance.m_Fire.GetHeatIncreaseText() + ")";
                    }
                }

            }
        }

        [HarmonyPatch(typeof(CookingPotItem), "AttachedFireIsBurning")]
        internal static class CookingPotItem_AttachedFireIsBurning
        {
            public static void Postfix(CookingPotItem __instance, ref bool __result)
            {
                if (Settings.options.embersSystem && Settings.options.embersSystemNoCooking && __result && __instance.m_FireBeingUsed?.m_EmberTimer > 0)
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(Fire), "IsBurning")]
        internal static class Fire_IsBurning
        {
            public static void Postfix(Fire __instance, ref bool __result)
            {
                if (Settings.options.embersSystem && Settings.options.embersSystemNoCooking && __result && __instance?.m_EmberTimer > 0)
                {
                    __result = false;
                }
            }
        }

    }
}