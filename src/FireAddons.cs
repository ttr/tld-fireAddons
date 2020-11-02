using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;


namespace FireAddons
{
	internal class FireAddons : MelonMod
	{
		public override void OnApplicationStart()
		{
			Debug.Log($"[{InfoAttribute.Name}] Version {InfoAttribute.Version} loaded!");
			Settings.OnLoad();
		}

		internal static void MyApplyChanges(GearItem gi) {

			// Lamp as firestarter
			if (Settings.options.lanternUse && gi.name.Contains("GEAR_KeroseneLamp"))
			{
				if (!gi.m_FireStarterItem)
				{
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConsumeOnUse = false;
				gi.m_FireStarterItem.m_FireStartDurationModifier = 60;
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.lanternDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.lanternStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.lanternStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.lanternPenalty;
				gi.m_FireStarterItem.m_OnUseSoundEvent = "Play_SNDACTIONFIREFLINTLOOP";

			}

			// Flint
			if (gi.name.Contains("GEAR_FlintAndSteel"))
			{
				if (!gi.m_FireStarterItem)
				{
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.flintDegredation;
				gi.m_StartCondition = GearStartCondition.Perfect;
				gi.m_WeightKG = 1.5f; // combined weight of components it's made.
            }
		}

		private static float GetModifiedFireStartSkillModifier(FuelSourceItem fs)
		{

			if (fs.name.Contains("GEAR_NewsprintRoll"))
			{
				return Settings.options.tinderBonusNewsprintRoll;
			}
			if (fs.name.Contains("GEAR_PaperStack"))
			{
				return Settings.options.tinderBonusPaper;
			}
			if (fs.name.Contains("GEAR_Newsprint"))
			{
				return Settings.options.tinderBonusNewsprint;
			}
			if (fs.name.Contains("GEAR_CashBundle"))
			{
				return Settings.options.tinderBonusCash;
			}
			if (fs.name.Contains("GEAR_BarkTinder"))
			{
				return Settings.options.tinderBonusBark;
			}
			if (fs.name.Contains("GEAR_Tinder"))
			{
				return Settings.options.tinderBonusPlug;
			}
			if (fs.name.Contains("GEAR_CattailTinder"))
			{
				return Settings.options.tinderBonusCattail;
			}

			MelonLogger.LogWarning("MISSING TINDER " + fs.name);
			return 0;
		}
		internal static bool IsNamedTinder(GearItem gi)
        {
			if (gi.name.Contains("GEAR_CattailTinder") ||
				gi.name.Contains("GEAR_Tinder") ||
				gi.name.Contains("GEAR_BarkTinder") ||
				gi.name.Contains("GEAR_NewsprintRoll") ||
				gi.name.Contains("GEAR_PaperStack") ||
				gi.name.Contains("GEAR_Newsprint") ||
				gi.name.Contains("GEAR_CashBundle"))
            {
				return true;
            }
			return false;
		}

		internal static void ModifyTinder(GearItem gi)
		{
			// if we modified it already, skip, otherwise You will have infinite burn time ;)
			if (gi.m_FuelSourceItem.m_IsTinder)
			{
				float value = Settings.options.tinderFuel / 60f;
				if (!gi.m_FuelSourceItem)
				{
					gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
				}
				gi.m_FuelSourceItem.m_HeatIncrease = Settings.options.tinderFueldeg; // deg C
				gi.m_FuelSourceItem.m_BurnDurationHours = value;
				gi.m_FuelSourceItem.m_FireStartDurationModifier = value;
				gi.m_FuelSourceItem.m_FireStartSkillModifier = GetModifiedFireStartSkillModifier(gi.m_FuelSourceItem);
				gi.m_FuelSourceItem.m_FireStartSkillModifier += Settings.options.tinderBonusOffset;
				gi.m_FuelSourceItem.m_IsTinder = false;
			}
		}
		internal static void ModifyWater(GearItem gi, bool state)
        {
			if (state)
			{
				gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
				gi.m_FuelSourceItem.m_HeatIncrease = 0;
				gi.m_FuelSourceItem.m_BurnDurationHours = 0;
				gi.m_FuelSourceItem.m_IsTinder = false;
				gi.m_FuelSourceItem.m_FireStartDurationModifier = 0;
				gi.m_FuelSourceItem.m_FireStartSkillModifier = 0;
			}
			else
            {
				gi.m_FuelSourceItem = null;
            }

		}
		internal static void Blueprints()
        {
			// Buy 'zeobviouslyfakeacc' a beer as i've used his tincan improvements code
			if (Settings.options.flintEnable) {
				// create bp

				BlueprintItem bp_flint= GameManager.GetBlueprints().AddComponent<BlueprintItem>();
				// Inputs
				bp_flint.m_RequiredGear = new Il2CppReferenceArray<GearItem>(3) { [0] = GetGearItemPrefab("GEAR_Prybar"), [1] = GetGearItemPrefab("GEAR_SharpeningStone"), [2] = GetGearItemPrefab("GEAR_Coal") };
				bp_flint.m_RequiredGearUnits = new Il2CppStructArray<int>(3) { [0] = 1, [1] = 1, [2] = 1 };
				bp_flint.m_KeroseneLitersRequired = 0f;
				bp_flint.m_GunpowderKGRequired = 0f;
				bp_flint.m_RequiredTool = GetToolItemPrefab("GEAR_Hammer");
				bp_flint.m_OptionalTools = new Il2CppReferenceArray<ToolsItem>(0);

				// Outputs
				bp_flint.m_CraftedResult = GetGearItemPrefab("GEAR_FlintAndSteel");
				bp_flint.m_CraftedResultCount = 1;

				// Process
				bp_flint.m_Locked = false;
				bp_flint.m_AppearsInStoryOnly = false;
				bp_flint.m_RequiresLight = false;
				bp_flint.m_RequiresLitFire = true;
				bp_flint.m_RequiredCraftingLocation = CraftingLocation.Forge;
				bp_flint.m_DurationMinutes = 360;
				bp_flint.m_CraftingAudio = "PLAY_CRAFTINGGENERIC";
				bp_flint.m_AppliedSkill = SkillType.ToolRepair;
				bp_flint.m_ImprovedSkill = SkillType.None;


					if (Settings.options.flintSmeltEnable) {
					// re-smelt one
					BlueprintItem bp_flint2 = GameManager.GetBlueprints().AddComponent<BlueprintItem>();
					// Inputs
					bp_flint2.m_RequiredGear = new Il2CppReferenceArray<GearItem>(3) { [0] = GetGearItemPrefab("GEAR_FlintAndSteel"), [1] = GetGearItemPrefab("GEAR_ScrapMetal"), [2] = GetGearItemPrefab("GEAR_Coal") };
					bp_flint2.m_RequiredGearUnits = new Il2CppStructArray<int>(3) { [0] = 1, [1] = 3, [2] = 2 };
					bp_flint2.m_KeroseneLitersRequired = 0f;
					bp_flint2.m_GunpowderKGRequired = 0f;
					bp_flint2.m_RequiredTool =  GetToolItemPrefab("GEAR_Hammer");
					bp_flint2.m_OptionalTools = new Il2CppReferenceArray<ToolsItem>(0);

					// Outputs
					bp_flint2.m_CraftedResult = GetGearItemPrefab("GEAR_FlintAndSteel");
					bp_flint2.m_CraftedResultCount = 1;

					// Process
					bp_flint2.m_Locked = false;
					bp_flint2.m_AppearsInStoryOnly = false;
					bp_flint2.m_RequiresLight = false;
					bp_flint2.m_RequiresLitFire = true;
					bp_flint2.m_RequiredCraftingLocation = CraftingLocation.Forge;
					bp_flint2.m_DurationMinutes = 180;
					bp_flint2.m_CraftingAudio = "PLAY_CRAFTINGGENERIC";
					bp_flint2.m_AppliedSkill = SkillType.ToolRepair;
					bp_flint2.m_ImprovedSkill = SkillType.None;
				}
			}
		}
		private static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
		private static ToolsItem GetToolItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<ToolsItem>();

		internal static void CalculateEmbers(Fire __instance)
        {
			if (__instance.m_HeatSource.IsTurnedOn() && __instance.GetFireState() != FireState.Off && (!__instance.m_IsPerpetual))
			{

				float currTemp = __instance.GetCurrentTempIncrease();
				float remSec = __instance.m_MaxOnTODSeconds - __instance.m_ElapsedOnTODSeconds;

				// aways use embers state
				if (!__instance.m_UseEmbers)
				{
					__instance.m_UseEmbers = true;
					__instance.m_DurationSecondsToReduceToEmbers = 0;
				}

				// reset embers timer if burning
				if (__instance.m_EmberTimer != 0 && remSec > 0 )
                {
					__instance.m_EmberTimer = 0;
				}

				if (currTemp > Settings.options.embersBunoutTemp && __instance.m_EmberDurationSecondsTOD > 0f)
				{
					float deltaTime = GameManager.GetTimeOfDayComponent().GetTODSeconds(Time.deltaTime);
					float burnRatio = Mathf.InverseLerp(Settings.options.embersBunoutTemp, 80f, currTemp);

					float maxBurnHour = Settings.options.embersBunoutRatio * Settings.options.embersTime;
					float variedBurnSec = (maxBurnHour * burnRatio) * 3600;

					// remove embers per tick
					__instance.m_EmberDurationSecondsTOD -= variedBurnSec * deltaTime * Settings.options.embersFuelEx;
					if ( __instance.m_EmberDurationSecondsTOD < 0f)
                    {
						__instance.m_EmberDurationSecondsTOD = 0;
					}
					else
                    {
						__instance.m_MaxOnTODSeconds += variedBurnSec * deltaTime;
					}
				}

				//	GameObject obj = __instance.transform.GetParent()?.gameObject;
				//if (obj && (obj.name.ToLower().Contains("woodstove") || obj.name.ToLower().Contains("potbellystove")))

			}
		}
		internal static void FeedFire(Panel_FeedFire __instance)
		{

			Fire _fire = __instance.m_Fire;
			GearItem fuel = __instance.GetSelectedFuelSource();
			// cool off fire with water

			if (fuel.name.ToLower().Contains("recycledcan") || fuel.name.ToLower().Contains("cookingpot"))
			{

				GearItem gi_wt = GameManager.GetInventoryComponent().GetPotableWaterSupply();
				if (gi_wt.m_WaterSupply.m_VolumeInLiters >= 0.25f && _fire.m_FuelHeatIncrease > Settings.options.waterTempRemoveDeg) {
					gi_wt.m_WaterSupply.m_VolumeInLiters -= 0.25f;
					_fire.m_HeatSource.m_MaxTempIncrease -= Settings.options.waterTempRemoveDeg;
					_fire.m_HeatSource.m_TempIncrease -= Settings.options.waterTempRemoveDeg;
					MelonLogger.Log(gi_wt.m_WaterSupply.m_VolumeInLiters + " " + _fire.m_HeatSource.m_MaxTempIncrease + " " + _fire.m_HeatSource.m_TempIncrease);
				} else if (gi_wt.m_WaterSupply.m_VolumeInLiters <  0.25f)
				{
					HUDMessage.AddMessage("Need water in inventory.", false);
				} else
                {
					HUDMessage.AddMessage("Temperature is too low.", false);
				}

			}
			else if (fuel.name.ToLower().Contains("wood") || fuel.name.ToLower().Contains("coal"))
			{

				if (_fire.m_EmberDurationSecondsTOD < (Settings.options.embersTime * 3600))
				{
					float fuelmove = fuel.m_FuelSourceItem.m_BurnDurationHours * Settings.options.embersFuelRatio;
					_fire.m_MaxOnTODSeconds -= fuelmove * 3600;
					_fire.m_EmberDurationSecondsTOD += (fuelmove * 3600) * Settings.options.embersFuelEx;
					// _fire.m_BurningTimeTODHours -= fuelmove;
					MelonLogger.Log(fuel.name + " " + _fire.m_EmberDurationSecondsTOD + " " + fuel.m_FuelSourceItem.m_BurnDurationHours * 3600 + " " + (fuelmove * 3600) * Settings.options.embersFuelEx);
				}
				else
				{
					MelonLogger.Log(fuel.name + " " + _fire.m_EmberDurationSecondsTOD + " ");
				}
			}
		}
	}
}
