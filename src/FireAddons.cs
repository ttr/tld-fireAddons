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
				// MelonLogger.Log("lamp: " + gi.name);
				if (!gi.m_FireStarterItem)
				{
					//MelonLogger.Log("lamp: add firestarter");
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConsumeOnUse = false;
				gi.m_FireStarterItem.m_FireStartDurationModifier = 60;
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.lanternDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.lanternStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.lanternStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.lanternPenalty;
				gi.m_FireStarterItem.m_OnUseSoundEvent = "Play_SNDACTIONFIREFLINTLOOP";
				// gi.m_Type = GearTypeEnum.Firestarting;

			}

			// Flint
			if (gi.name.Contains("GEAR_FlintAndSteel"))
			{
				//MelonLogger.Log("flint: " + gi.name);
				if (!gi.m_FireStarterItem)
				{
					//MelonLogger.Log("Flint: add firestarter");
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.flintDegredation;
				gi.m_StartCondition = GearStartCondition.Perfect;
				gi.m_WeightKG = 1.5f;
            }
			if (gi.m_FireStarterItem)
            {

				//MelonLogger.Log("FS stat: " + gi.name + " " + gi.m_FireStarterItem.m_SecondsToIgniteTinder + " " + gi.m_FireStarterItem.m_SecondsToIgniteTorch + " " + gi.m_FireStarterItem.m_FireStartSkillModifier + " " + gi.m_FireStarterItem.m_ConditionDegradeOnUse + " " + gi.m_FireStarterItem.m_FireStartDurationModifier);
            }
			if (gi.m_FuelSourceItem)
			{

				//MelonLogger.Log("FU stat: " + gi.name + " " + gi.m_FuelSourceItem.m_FireStartSkillModifier + " " + gi.m_FuelSourceItem.m_FireStartDurationModifier + " " + gi.m_FuelSourceItem.m_BurnDurationHours + " " + gi.m_FuelSourceItem.m_HeatIncrease + " " + gi.m_FuelSourceItem.m_IsTinder + " " + gi.m_FuelSourceItem.m_FireAgeMinutesBeforeAdding);
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
	}
}
