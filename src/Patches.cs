using Harmony;
using UnhollowerBaseLib;


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
                // Buy 'zeobviouslyfakeacc' a beer as i've used his tincan improvements code
                BlueprintItem bp_flint = GameManager.GetBlueprints().AddComponent<BlueprintItem>();
                // Inputs
                bp_flint.m_RequiredGear = new Il2CppReferenceArray<GearItem>(3) { [0] = FireAddons.GetGearItemPrefab("GEAR_Prybar"), [1] = FireAddons.GetGearItemPrefab("GEAR_SharpeningStone"), [2] = FireAddons.GetGearItemPrefab("GEAR_Coal") };
                bp_flint.m_RequiredGearUnits = new Il2CppStructArray<int>(3) { [0] = 1, [1] = 1, [2] = 1 };
                bp_flint.m_KeroseneLitersRequired = 0f;
                bp_flint.m_GunpowderKGRequired = 0f;
                bp_flint.m_RequiredTool = null;
                bp_flint.m_OptionalTools = new Il2CppReferenceArray<ToolsItem>(0);

                // Outputs
                bp_flint.m_CraftedResult = FireAddons.GetGearItemPrefab("GEAR_FlintAndSteel");
                bp_flint.m_CraftedResultCount = 1;

                // Process
                bp_flint.m_Locked = false;
                bp_flint.m_AppearsInStoryOnly = false;
                bp_flint.m_RequiresLight = false;
                bp_flint.m_RequiresLitFire = true;
                bp_flint.m_RequiredCraftingLocation = CraftingLocation.Forge;
                bp_flint.m_DurationMinutes = 180;
                bp_flint.m_CraftingAudio = "PLAY_CRAFTINGGENERIC";
                bp_flint.m_AppliedSkill = SkillType.None;
                bp_flint.m_ImprovedSkill = SkillType.None;

            }

            [HarmonyPatch(typeof(Panel_Crafting), "ItemPassesFilter")]
            private static class RecipesInCrafting
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