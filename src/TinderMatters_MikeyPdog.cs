/* canibalised version from MikeyPdog TLD 1.65 mod */

using Harmony;
using MelonLoader;
using UnityEngine;

namespace FireAddons
{
    [HarmonyPatch(typeof(Panel_FireStart))]
    [HarmonyPatch("OnStartFire")]
    static class PatchPanel_FireStart_OnStartFire
    {
        static bool Prefix(Panel_FireStart __instance, Campfire ___m_CampFireInstance, FireStarterItem ___m_KeroseneAccelerant, FireManager ___m_FireManager, bool skipResearchItemCheck = false)
        {
            var traverseInstance = Traverse.Create(__instance);
            traverseInstance.Field("m_FireManager").SetValue(GameManager.GetFireManagerComponent());

            var selectedFireStarter = traverseInstance.Method("GetSelectedFireStarter").GetValue<FireStarterItem>();
            var selectedTinder = traverseInstance.Method("GetSelectedTinder").GetValue<FuelSourceItem>();
            var selectedFuelSource = traverseInstance.Method("GetSelectedFuelSource").GetValue<FuelSourceItem>();
            var selectedAccelerant = traverseInstance.Method("GetSelectedAccelerant").GetValue<FireStarterItem>();

            if (___m_CampFireInstance && ___m_CampFireInstance.TooWindyToStart())
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Itstoowindytostartafirethere"), false);
                return false;
            }
            if (selectedFireStarter == null || (GameManager.GetSkillFireStarting().TinderRequired() && selectedTinder == null) || selectedFuelSource == null)
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Insufficientmaterialstocreatefire"), false);
                return false;
            }

            if (selectedFireStarter.m_RequiresSunLight && !traverseInstance.Method("HasDirectSunlight").GetValue<bool>())
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Magnifyinglensrequiresdirectsunlight"), false);
                return false;
            }
            if (!skipResearchItemCheck)
            {
                ResearchItem component = selectedFuelSource.GetComponent<ResearchItem>();
                if (component != null && !component.IsResearchComplete())
                {
                    // @ttr: this code give me greif so for now, you can't use unreaded books as fuel, ok ? Good.
                    /* Panel_Confirmation.CallbackDelegate forceBurnResearchItem = () => traverseInstance.Method("ForceBurnResearchItem").GetValue();
                    InterfaceManager.m_Panel_Confirmation.ShowBurnResearchNotification(forceBurnResearchItem); */
                    HUDMessage.AddMessage("FireAddons: Unread books not allowed.", false); 
                    return false;
                }
            }
            if (selectedAccelerant != null && selectedAccelerant == ___m_KeroseneAccelerant)
            {
                GameManager.GetPlayerManagerComponent().DeductLiquidFromInventory(GameManager.GetFireManagerComponent().m_KeroseneLitersAccelerant, GearLiquidTypeEnum.Kerosene);
            }
            float percentChanceSuccess = TinderHelper.CalculateFireStartSuccess(selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant);
            MelonLogger.Log("Fire start 1");
            ___m_FireManager.PlayerStartFire(___m_CampFireInstance, selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant, percentChanceSuccess);
            MelonLogger.Log("Fire start 2");
            __instance.Enable(false);
            MelonLogger.Log("Fire start 3");
            GameAudioManager.PlayGuiConfirm();
            return false;
        }

        /* ORIGINAL METHOD AS OF 1.47 2nd March 2019
         * // Token: 0x06004671 RID: 18033 RVA: 0x001626FC File Offset: 0x00160AFC
	    public void OnStartFire(bool skipResearchItemCheck = false)
	    {
		    this.m_FireManager = GameManager.GetFireManagerComponent();
		    FireStarterItem selectedFireStarter = this.GetSelectedFireStarter();
		    FuelSourceItem selectedTinder = this.GetSelectedTinder();
		    FuelSourceItem selectedFuelSource = this.GetSelectedFuelSource();
		    FireStarterItem selectedAccelerant = this.GetSelectedAccelerant();
		    if (this.m_CampFireInstance && this.m_CampFireInstance.TooWindyToStart())
		    {
			    GameAudioManager.PlayGUIError();
			    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Itstoowindytostartafirethere"), false);
			    return;
		    }
		    if (selectedFireStarter == null || (GameManager.GetSkillFireStarting().TinderRequired() && selectedTinder == null) || selectedFuelSource == null)
		    {
			    GameAudioManager.PlayGUIError();
			    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Insufficientmaterialstocreatefire"), false);
			    return;
		    }
		    if (selectedFireStarter.m_RequiresSunLight && !this.HasDirectSunlight())
		    {
			    GameAudioManager.PlayGUIError();
			    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_Magnifyinglensrequiresdirectsunlight"), false);
			    return;
		    }
		    if (!skipResearchItemCheck)
		    {
			    ResearchItem component = selectedFuelSource.GetComponent<ResearchItem>();
			    if (component != null && !component.IsResearchComplete())
			    {
				    InterfaceManager.m_Panel_Confirmation.ShowBurnResearchNotification(new Panel_Confirmation.CallbackDelegate(this.ForceBurnResearchItem));
				    return;
			    }
		    }
		    if (selectedAccelerant != null && selectedAccelerant == this.m_KeroseneAccelerant)
		    {
			    GameManager.GetPlayerManagerComponent().DeductLiquidFromInventory(GameManager.GetFireManagerComponent().m_KeroseneLitersAccelerant, GearLiquidTypeEnum.Kerosene);
		    }
		    float percentChanceSuccess = this.m_FireManager.CalclateFireStartSuccess(selectedFireStarter, selectedFuelSource, selectedAccelerant);
		    this.m_FireManager.PlayerStartFire(this.m_CampFireInstance, selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant, percentChanceSuccess);
		    this.Enable(false);
		    GameAudioManager.PlayGuiConfirm();
	    }*/
    }

    [HarmonyPatch(typeof(Panel_FireStart))]
    [HarmonyPatch("RefreshChanceOfSuccessLabel")]
    static class PatchPanel_FireStart_RefreshChanceOfSuccessLabel
    {
        [HarmonyPatch(typeof(Panel_FireStart))]
        [HarmonyPatch("RefreshChanceOfSuccessLabel")]
        static bool Prefix(Panel_FireStart __instance)
        {
            var traverseInstance = Traverse.Create(__instance);
            traverseInstance.Field("m_FireManager").SetValue(GameManager.GetFireManagerComponent());

            var selectedFireStarter = traverseInstance.Method("GetSelectedFireStarter").GetValue<FireStarterItem>();
            var selectedTinder = traverseInstance.Method("GetSelectedTinder").GetValue<FuelSourceItem>();
            var selectedFuelSource = traverseInstance.Method("GetSelectedFuelSource").GetValue<FuelSourceItem>();
            var selectedAccelerant = traverseInstance.Method("GetSelectedAccelerant").GetValue<FireStarterItem>();

            float num = TinderHelper.CalculateFireStartSuccess(selectedFireStarter, selectedTinder, selectedFuelSource, selectedAccelerant);
            if (selectedFireStarter.m_RequiresSunLight && !traverseInstance.Method("HasDirectSunlight").GetValue<bool>())
            {
                num = 0f;
            }

            __instance.m_Label_ChanceSuccess.text = num.ToString("F0") + "%";
            return false;
        }

        /* ORIGINAL METHOD AS OF 1.47 2nd March 2019
	    // Token: 0x06004664 RID: 18020 RVA: 0x00162194 File Offset: 0x00160594
	    private void RefreshChanceOfSuccessLabel()
	    {
		    this.m_FireManager = GameManager.GetFireManagerComponent();
		    FireStarterItem selectedFireStarter = this.GetSelectedFireStarter();
		    FuelSourceItem selectedFuelSource = this.GetSelectedFuelSource();
		    FireStarterItem selectedAccelerant = this.GetSelectedAccelerant();
		    float num = 0f;
		    if (selectedFireStarter && selectedFuelSource)
		    {
			    num = this.m_FireManager.CalclateFireStartSuccess(selectedFireStarter, selectedFuelSource, selectedAccelerant);
		    }
		    if (selectedFireStarter == null)
		    {
			    num = 0f;
		    }
		    else if (selectedFireStarter.m_RequiresSunLight && !this.HasDirectSunlight())
		    {
			    num = 0f;
		    }
		    this.m_Label_ChanceSuccess.text = num.ToString("F0") + "%";
	    }*/
    }

    public class TinderHelper
    {
        // COPY of CalclateFireStartSuccess with tinder added
        public static float CalculateFireStartSuccess(FireStarterItem starter, FuelSourceItem tinder, FuelSourceItem fuel, FireStarterItem accelerant)
        {
            if (starter == null || fuel == null)
            {
                return 0f;
            }
            float successChance = GameManager.GetSkillFireStarting().GetBaseChanceSuccess();
            successChance += starter.m_FireStartSkillModifier;
            successChance += GetModifiedFireStartSkillModifier(tinder);
            successChance += fuel.m_FireStartSkillModifier;
            if (accelerant)
            {
                successChance += accelerant.m_FireStartSkillModifier;
            }
            return Mathf.Clamp(successChance, 0f, 100f);
        }

        public static float GetModifiedFireStartSkillModifier(FuelSourceItem __instance)
        {
            if (__instance == null) // No tinder at all (level 3+)
            {
                return Settings.options.tinderBonusNone;
            }
            if (__instance.name == "GEAR_NewsprintRoll")
            {
                return Settings.options.tinderBonusNone;
                            }
            if (__instance.name == "GEAR_PaperStack")
            {
                return Settings.options.tinderBonusNone;
            }
            if (__instance.name == "GEAR_Newsprint")
            {
                return Settings.options.tinderBonusNone;
            }
            if (__instance.name == "GEAR_CashBundle")
            {
                return Settings.options.tinderBonusNone;
            }
            if (__instance.name == "GEAR_BarkTinder")
            {
                return Settings.options.tinderBonusNone;
            }
            if (__instance.name == "GEAR_Tinder")
            {
                return Settings.options.tinderBonusPlug;
            }
            if (__instance.name == "GEAR_CattailTinder")
            {
                return Settings.options.tinderBonusNone;
            }

            Debug.LogWarning("[TinderMatters] MISSING TINDER " + __instance.name);
            return 0;
        }
    }
}
