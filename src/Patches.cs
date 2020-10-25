using Harmony;
using MelonLoader;
using UnityEngine;


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
        }
    }

}