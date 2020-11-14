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
                FireAddons.MyApplyChanges(__instance);
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

        // based on Fire_RV mod by Deus131
        // will allow use tinder as starting fuel
        [HarmonyPatch(typeof(Panel_FireStart))]
        [HarmonyPatch("RefreshList")]
        static class PatchPanel_FireStart_RefreshList
        {
            private static void Prefix(Panel_FeedFire __instance)
            {

                if (!GameManager.GetSkillFireStarting().TinderRequired())
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

                                    FireAddons.ModifyTinder(gearItem);

                                }
                                if (Settings.options.embersSystem && (gearItem.name.ToLower().Contains("recycledcan") || gearItem.name.ToLower().Contains("cookingpot")))
                                {
                                    FireAddons.ModifyWater(gearItem, false);
                                }
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
                            if (FireAddons.IsNamedTinder(gearItem))
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
                if (__instance.m_Fire.GetFireState() != FireState.Off && __instance.m_Fire.m_EmberDurationSecondsTOD > 0 && __instance.m_Fire.m_EmberTimer >= 0)
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
                if (__instance.m_Fire.GetFireState() != FireState.Off && __instance.m_Fire.m_EmberDurationSecondsTOD > 0 && __instance.m_Fire.m_EmberTimer >= 0)
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
    }
}