using Harmony;
using MelonLoader;
//using System.Collections.Generic;

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
            // based on Fire_RV mod by Deus131
            // will allow use tinder as starting fuel
            [HarmonyPatch(typeof(Panel_FireStart))]
            [HarmonyPatch("RefreshList")]
            static class PatchPanel_FireStart_RefreshList
            {
                private static void Prefix(Panel_FeedFire __instance)
                {
                    MelonLogger.Log("onstartfire");
                    if (!GameManager.GetSkillFireStarting().TinderRequired())
                    {
                        MelonLogger.Log("tinder not required");
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
                                        MelonLogger.Log("Tinder:"+ gearItem);
                                        FireAddons.ModifyTinder(gearItem);

                                    }
                                }
                            }
                        }
                    }
                }
            }


            // from Fire_RV mod by Deus131
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
                                    gearItem.m_FuelSourceItem.m_IsTinder = false;
                                }
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
        }
    }

}