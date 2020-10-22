using MelonLoader;
using UnityEngine;
using Harmony;


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
			
			// Tinders
			// MelonLogger.Log("all: " + gi.name);
            if ((bool)(Settings.options.tinderAsFuel && (gi.name.Contains("GEAR_CattailTinder") ||
				gi.name.Contains("GEAR_Tinder") ||
				gi.name.Contains("GEAR_BarkTinder") ||
                gi.name.Contains("GEAR_NewsprintRoll") ||
                gi.name.Contains("GEAR_PaperStack") ||
                gi.name.Contains("GEAR_Newsprint") ||
                gi.name.Contains("GEAR_CashBundle")))
                )
            {
				ModifyTinder(gi);
				// MelonLogger.Log("tinder: " + gi.name);
			}
           
			// Lamp as firestarter
			if ((bool)(Settings.options.lanternUse && gi.name.Contains("GEAR_KeroseneLamp")))
			{
				// MelonLogger.Log("lamp: " + gi.name);
				if (!gi.m_FireStarterItem)
				{	
					//MelonLogger.Log("lamp: add firestarter");
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConsumeOnUse = false;
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.lanternDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.lanternStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.lanternStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.lanternPenalty; 
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
			}
			if (gi.m_FireStarterItem)
            {
				
				//MelonLogger.Log("FS stat: " + gi.name + " " + gi.m_FireStarterItem.m_SecondsToIgniteTinder + " " + gi.m_FireStarterItem.m_SecondsToIgniteTorch + " " + gi.m_FireStarterItem.m_FireStartSkillModifier + " " + gi.m_FireStarterItem.m_ConditionDegradeOnUse);
            }
			if (gi.m_FuelSourceItem)
			{

				//MelonLogger.Log("FU stat: " + gi.name + " " + gi.m_FuelSourceItem.m_FireStartSkillModifier + " " + gi.m_FuelSourceItem.m_FireStartDurationModifier + " " + gi.m_FuelSourceItem.m_HeatIncrease);
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
		private static void ModifyTinder(GearItem gi)
		{
			float value = Settings.options.tinderFuel / 60;
			if (!gi.m_FuelSourceItem)
			{
				//MelonLogger.Log(gi.name + ": add fuelSource: " + value);
				gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
				gi.m_FuelSourceItem.m_BurnDurationHours = value;
				gi.m_FuelSourceItem.m_FireStartDurationModifier = value;
			}
			//MelonLogger.Log(gi.name + ": add fuel: " + value);
			gi.m_FuelSourceItem.m_BurnDurationHours += value;
			gi.m_FuelSourceItem.m_FireStartDurationModifier += value;
			gi.m_FuelSourceItem.m_FireStartSkillModifier = GetModifiedFireStartSkillModifier(gi.m_FuelSourceItem);
			gi.m_FuelSourceItem.m_FireStartSkillModifier += Settings.options.tinderBonusOffset;

			if (!GameManager.GetSkillFireStarting().TinderRequired())
            {
				gi.m_FuelSourceItem.m_IsTinder = false;
			}
		}
	}
}
