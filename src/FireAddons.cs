using MelonLoader;
using Il2Cpp;
using UnityEngine;
using MelonLoader.TinyJSON;
using Il2CppTLD.Gear;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ModData;



namespace FireAddons
{
	internal class FireAddons : MelonMod
	{
		private static List<string> fireFixed = new List<string>();
		private static int FADSchema = 3;
		private static bool FADForceReload = false;
		private static Dictionary<string, FireAddonsData> FAD = new Dictionary<string, FireAddonsData>();
    internal static ModDataManager SaveMgr = new ModDataManager("FireAddons", false);       

    public override void OnInitializeMelon()
		{
			Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
			Settings.OnLoad();
		}
        
    internal static void LoadData()
		{
			FAD.Clear();
			fireFixed.Clear();

			string? data = SaveMgr.Load("FAD");

            if (!string.IsNullOrEmpty(data))
			{
				MelonLogger.Msg("JSON loaded " + data);
                fireFixed.Clear();
                FAD = JSON.Load(data).Make<Dictionary<string, FireAddonsData>>();
			}
		}
/*
		internal static void SaveData()
		{
			string data = JSON.Dump(FAD, EncodeOptions.NoTypeHints);
			SaveMgr.Save(data, "FAD");
		}
*/
		internal static float GetModifiedFireStartSkillModifier(FuelSourceItem fs)
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

			MelonLogger.Warning("MISSING TINDER " + fs.name);
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
				if (!gi.m_FuelSourceItem)
				{
					gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
				}
				gi.m_FuelSourceItem.m_HeatIncrease = Settings.options.tinderFueldeg; // deg C
				gi.m_FuelSourceItem.m_BurnDurationHours = Settings.options.tinderFuel / 60f;
				gi.m_FuelSourceItem.m_IsTinder = false;
				gi.m_FuelSourceItem.m_HeatInnerRadius = 1;
				gi.m_FuelSourceItem.m_HeatInnerRadius = 1;
			}
		}
		internal static void ModifyCharcoal(GearItem gi)
        {
			if (!gi.m_FuelSourceItem)
			{
				gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
			}
			gi.m_FuelSourceItem.m_HeatIncrease = Settings.options.burnCharcoalTemp; // deg C
			gi.m_FuelSourceItem.m_BurnDurationHours = Settings.options.burnCharcoalTime / 60f;
			gi.m_FuelSourceItem.m_IsTinder = false;
			gi.m_FuelSourceItem.m_HeatInnerRadius = 5;
			gi.m_FuelSourceItem.m_HeatInnerRadius = 10;
		}
		internal static void ModifyWater(GearItem gi, bool state)
		{
			if (state)
			{
				if (!gi.m_FuelSourceItem)
				{
					gi.m_FuelSourceItem = gi.gameObject.AddComponent<FuelSourceItem>();
					gi.m_FuelSourceItem.m_HeatIncrease = 0;
					gi.m_FuelSourceItem.m_BurnDurationHours = 0;
					gi.m_FuelSourceItem.m_IsTinder = false;
					gi.m_FuelSourceItem.m_FireStartDurationModifier = 0;
					gi.m_FuelSourceItem.m_FireStartSkillModifier = -100;
				}

			} else
            {
				if (gi.m_FuelSourceItem)
                {
					gi.m_FuelSourceItem = null;
				}
			}
		}

		internal static void ModifyFirestarters(GearItem gi)
		{

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
				gi.m_FireStarterItem.m_OnUseSoundEvent = "PLAY_LANTERNLIGHT";

			}

			// Lenses
			if (gi.name.Contains("GEAR_MagnifyingLens") || gi.name.Contains("GEAR_Binoculars"))
            {
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.lensesDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.lensesStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.lensesStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.lensesPenalty;
            }

			if (gi.name.Contains("GEAR_Firestriker"))
            {
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.firestrikerDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.firestrikerStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.firestrikerStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.firestrikerPenalty;
			}
		}


		private static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
		private static ToolsItem GetToolItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<ToolsItem>();
    public static Il2CppAK.Wwise.Event? MakeAudioEvent(string eventName)
    {
      if (eventName == null)
      {
        return null;
      }
      uint eventId = AkSoundEngine.GetIDFromString(eventName);
      if (eventId <= 0 || eventId >= 4294967295)
      {
        return null;
      }

      Il2CppAK.Wwise.Event newEvent = new();
      newEvent.WwiseObjectReference = new WwiseEventReference();
      newEvent.WwiseObjectReference.objectName = eventName;
      newEvent.WwiseObjectReference.id = eventId;
      return newEvent;
    }

    private static void WriteFireData(Fire __instance, string guid)
    {
      // create new instance if needed
      if (!FAD.ContainsKey(guid))
      {
        FireAddonsData lFAD = new FireAddonsData();
        FAD.Add(guid, lFAD);
      }
      FAD[guid].timestamp = GameManager.GetTimeOfDayComponent().GetTODSeconds(GameManager.GetTimeOfDayComponent().GetSecondsPlayedUnscaled());
      FAD[guid].ver = FADSchema;
      FAD[guid].fireState = __instance.GetFireState().ToString();
      FAD[guid].embersSeconds = __instance.m_EmberDurationSecondsTOD;
      FAD[guid].emberTimer = __instance.m_EmberTimer;
      FAD[guid].burnSeconds = __instance.m_ElapsedOnTODSeconds;
      FAD[guid].burnMaxSeconds = __instance.m_MaxOnTODSeconds;
      FAD[guid].heatTemp = __instance.m_HeatSource.m_MaxTempIncrease;
      FAD[guid].infinite = __instance.m_IsPerpetual;
      SaveMgr.Save(JSON.Dump(FAD, EncodeOptions.NoTypeHints), "FAD");
    }

		private static void LoadFireData(Fire __instance, string guid)
        {
			if (FAD.ContainsKey(guid))
			{
				float timeDiff = GameManager.GetTimeOfDayComponent().GetTODSeconds(GameManager.GetTimeOfDayComponent().GetSecondsPlayedUnscaled()) - FAD[guid].timestamp;
				__instance.m_EmberTimer = FAD[guid].emberTimer;
				__instance.m_ElapsedOnTODSeconds = FAD[guid].burnSeconds;
				__instance.m_MaxOnTODSeconds = FAD[guid].burnMaxSeconds;
				__instance.m_EmberDurationSecondsTOD = FAD[guid].embersSeconds;
				__instance.m_FuelHeatIncrease = FAD[guid].heatTemp;
				__instance.m_HeatSource.m_MaxTempIncrease = FAD[guid].heatTemp;
				__instance.m_HeatSource.m_TempIncrease = FAD[guid].heatTemp;
				__instance.m_IsPerpetual = FAD[guid].infinite;
				CalculateFireTimers(__instance, timeDiff, __instance.m_MaxOnTODSeconds - __instance.m_ElapsedOnTODSeconds, true);
				
				if (__instance.m_Campfire && (__instance.m_MaxOnTODSeconds > __instance.m_ElapsedOnTODSeconds || __instance.m_EmberDurationSecondsTOD > __instance.m_EmberTimer))
				{
					__instance.m_Campfire.m_State = CampfireState.Lit;
				}
				MelonLogger.Msg("restore " + guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString() + " tdiff:" + timeDiff);
			}
		}

		private static void RemoveFireData(Fire __instance, string guid)
    {
			FAD.Remove(guid);
      SaveMgr.Save(JSON.Dump(FAD, EncodeOptions.NoTypeHints), "FAD");
    }
		private static void ResetEmbersOnRestart(Fire __instance)
        {
			if ( __instance.m_EmberDurationSecondsTOD > __instance.m_EmberTimer)
            {
				__instance.m_EmberDurationSecondsTOD -= __instance.m_EmberTimer;
				//__instance.m_MaxOnTODSeconds += __instance.m_EmberTimer;

			}
			__instance.m_EmberTimer = 0;
			__instance.m_HeatSource.m_TurnedOn = true;

		}
		private static void CalculateFireTimers(Fire __instance, float timeDiff, float remSec, bool reload=false)
        {
			float currTemp = __instance.GetCurrentTempIncrease();
			if (timeDiff > remSec)
			{
				timeDiff -= remSec;
				__instance.m_ElapsedOnTODSeconds = __instance.m_MaxOnTODSeconds;
				__instance.m_EmberTimer += timeDiff;
			}
			else
			{
				if (reload)
				{
					__instance.m_ElapsedOnTODSeconds += timeDiff;
				}
			}

			if (__instance.m_EmberDurationSecondsTOD < 0)
			{
				__instance.m_EmberDurationSecondsTOD = 0;
			}

			if ( currTemp > Settings.options.embersBunoutTemp && __instance.m_EmberDurationSecondsTOD > 0f)
			{
				float burnRatio = Mathf.InverseLerp(Settings.options.embersBunoutTemp, 80f, currTemp);
				float maxBurnRatio = Settings.options.embersBunoutRatio * Settings.options.embersTime;
				float variedBurnSec = (maxBurnRatio * burnRatio);

				/* Small word of explanation
				 * this looks like it's underpowered as next one is ratio in hours and nowhere is converted to seconds
				 * however deltatime is in seconds, so whole ember store would be burned in second, not hour and doing 2 converstion (delta time and varitBurnSec) just to look clear, feels insane
				 */

				// remove embers per timeDiff
				__instance.m_EmberDurationSecondsTOD -= variedBurnSec * timeDiff;
				if (__instance.m_EmberDurationSecondsTOD < 0f)
				{
					__instance.m_EmberDurationSecondsTOD = 0;
				}
				else
				{
					__instance.m_MaxOnTODSeconds += variedBurnSec * timeDiff / Settings.options.embersFuelEx;
				}
			}
		}

		private static void ApplyStoredFAD(Fire __instance, string guid)
		{
			if (FAD.ContainsKey(guid))
			{
				if (FAD[guid].ver == FADSchema)
				{
					FireState foo = (FireState)System.Enum.Parse(typeof(FireState), FAD[guid].fireState);
					__instance.FireStateSet(foo);
					LoadFireData(__instance, guid);
					// always on load, enable embers -> this way we should have embers when fire burned out off-scene
					__instance.m_UseEmbers = true;
				}
				else
				{
					RemoveFireData(__instance, guid);
				}
				fireFixed.Add(guid);
			}

		}
		internal static void CalculateEmbers(Fire __instance)
		{
            string guid = ObjectGuid.GetGuidFromGameObject(__instance.gameObject);
			if (guid == null)
			{
				// if you move forge with SC+, it's GUID is gone.
				ObjectGuid.MaybeAttachObjectGuidAddOrReplace(__instance.gameObject, Guid.NewGuid().ToString());
				guid = ObjectGuid.GetGuidFromGameObject(__instance.gameObject);
				MelonLogger.Msg("New Guid generated " + guid);
			}
			
            if (__instance.m_IsPerpetual)
			{
				if (FAD.ContainsKey(guid)) {
					WriteFireData(__instance, guid);
					FAD.Remove(guid);
					MelonLogger.Msg("Fire was switched to infinite - storing it's data: " + guid);
				}
            }
			else
			{
				float deltaTime = GameManager.GetTimeOfDayComponent().GetTODSeconds(Time.deltaTime);
				float remSec = __instance.m_MaxOnTODSeconds - __instance.m_ElapsedOnTODSeconds;

				/* when time acceleration is active, all Update() functions are called, but in end thre is TodMaterial.UpdateAll()
				 * which will calculate fire time twice.
				 */

				if (GameManager.GetTimeOfDayComponent().IsTimeLapseActive())
				{
					if (!FADForceReload)
					{
						fireFixed.Clear();
						FADForceReload = true;
					}
				}
				else
				{
					if (FADForceReload)
					{
						FADForceReload = false;
					}

					// apply stored configs to detected fires if needed
					if (!fireFixed.Contains(guid))
					{
						ApplyStoredFAD(__instance, guid);
					}

					if (__instance.GetFireState() == FireState.FullBurn)
					{
						// if remSec is negative, sync them (no burn)
						if (remSec < 0 )
						{
							__instance.m_ElapsedOnTODSeconds = __instance.m_MaxOnTODSeconds;
						}

						// reset embers timer if burning, reduce embers of burned value
						if (__instance.m_EmberTimer != 0 && remSec > 0)
						{
							ResetEmbersOnRestart(__instance);
						}
						if (remSec > 0)
						{
							CalculateFireTimers(__instance, deltaTime, remSec);
						}

						// for future, perhaps, maybe...
						//	GameObject obj = __instance.transform.GetParent()?.gameObject;
						//if (obj && (obj.name.ToLower().Contains("woodstove") || obj.name.ToLower().Contains("potbellystove")))
						//if (__instance.m_Campfire) { ...

						if (__instance.m_EmberTimer > 0)
						{
							__instance.m_UseEmbers = true;
							__instance.m_HeatSource.m_MaxTempIncrease = 5;
						}
						WriteFireData(__instance, guid);
						// orginally this is 7 mins before end of fuel time, lets change this to 1s
						__instance.m_DurationSecondsToReduceToEmbers = 1;
					}
					else if (__instance.GetFireState() == FireState.Off)
					{

						if (FAD.ContainsKey(guid))
						{
							//MelonLogger.Msg("rm " + guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString());
							__instance.m_UseEmbers = false;
							__instance.m_EmberDurationSecondsTOD = 0;
							__instance.m_EmberTimer = 0;
							RemoveFireData(__instance, guid);
						}
					}
					
					if (__instance.GetFireState().ToString() != "Off" && ((int)(deltaTime % 10) == 0))
					{
						//MelonLogger.Msg(guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString() + " " + __instance.m_HeatSource.IsTurnedOn());
					}
					
				}
			}
		}
		internal static void CalculateCharcoal(Fire __instance)
        {
			string guid = ObjectGuid.GetGuidFromGameObject(__instance.gameObject);
			//MelonLogger.Msg(guid + " charcoal " + __instance.m_NumGeneratedCharcoalPieces);
			if (FAD.ContainsKey(guid) && __instance.m_NumGeneratedCharcoalPieces <0)
            {
				float tmp = __instance.m_NumGeneratedCharcoalPieces;
				__instance.m_NumGeneratedCharcoalPieces = 0;
				__instance.m_BurningTimeTODHours = 0;
				//MelonLogger.Msg(guid + " capping charcoal to 0 from " + tmp);
			}
		}
		internal static void FeedFireEmbers(Panel_FeedFire __instance)
		{

			Fire _fire = __instance.m_FireplaceInteraction.Fire;
			string guid = ObjectGuid.GetGuidFromGameObject(_fire.gameObject);
			GearItem fuel = __instance.GetSelectedFuelSource();

			// added fuel while embers
			if (_fire.m_EmberTimer > 0f)
			{
				ResetEmbersOnRestart(_fire);
			}
			// try add fuel to embers unless it wasn't comming out from ember state; only for certian fuels
			else if (fuel.name.ToLower().Contains("wood") || fuel.name.ToLower().StartsWith("gear_coal") || fuel.name.ToLower().StartsWith("gear_charcoal"))
			{
				if (_fire.GetRemainingLifeTimeSeconds() < 39600 && (_fire.m_EmberDurationSecondsTOD < (Settings.options.embersTime * 3600)))
				{
					float fuelmove = fuel.m_FuelSourceItem.m_BurnDurationHours * Settings.options.embersFuelRatio;
					_fire.m_MaxOnTODSeconds -= fuelmove * 3600;
					_fire.m_EmberDurationSecondsTOD += (fuelmove * 3600) * Settings.options.embersFuelEx;
					_fire.m_BurningTimeTODHours -= fuelmove;

				}
			}
		}
        internal static void FeedFireCharoal(Panel_FeedFire __instance)
        {

            Fire _fire = __instance.m_FireplaceInteraction.Fire;
            GearItem fuel = __instance.GetSelectedFuelSource();

            // If charcoal was added, hack charcoal values
            if (fuel.name.ToLower().StartsWith("gear_charcoal"))
            {
				_fire.m_NumGeneratedCharcoalPieces -= 1;
			}
		}

		internal static float CookSpeedFromTemp(float temp)
		{
			// no complex conditions needed due to early returns.
			if (!Settings.options.cookingSystem) { return 1; }
			if (temp < Settings.options.cookingSystemTempMin) { return float.PositiveInfinity; }
			if (temp < Settings.options.cookingSystemTempLow) { return Mathf.Lerp(Settings.options.cookingSystemTimeLow, 1f, Mathf.InverseLerp(Settings.options.cookingSystemTempMin, Settings.options.cookingSystemTempLow, temp)); }
			if (temp <= Settings.options.cookingSystemTempHigh) { return 1; }
			if (temp <= Settings.options.cookingSystemTempMax) { return Mathf.Lerp(1f, Settings.options.cookingSystemTimeHigh, Mathf.InverseLerp(Settings.options.cookingSystemTempHigh, Settings.options.cookingSystemTempMax, temp)); }
			if (temp > Settings.options.cookingSystemTempMax) { return Settings.options.cookingSystemTimeHigh; }
            return 1;
		}
	}
	internal class FireAddonsData
    {
		public int ver;
		public float timestamp;
		public string fireState;
		public float embersSeconds;
		public float emberTimer;
		public float burnSeconds;
		public float burnMaxSeconds;
		public float heatTemp;
		public bool infinite;
	}
}
