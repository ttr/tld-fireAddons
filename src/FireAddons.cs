using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.Collections.Generic;


namespace FireAddons
{
	internal class FireAddons : MelonMod
	{
		private const string SAVE_NAME = "fireAddons";
		private static List<string> fireFixed = new List<string>();
		private static int FADSchema = 2;
		private static bool FADForceReload = false;
		private static Dictionary<string, FireAddonsData> FAD = new Dictionary<string, FireAddonsData>();
		public override void OnApplicationStart()
		{
			Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
			Settings.OnLoad();
		}


		internal static void LoadData(string name)
		{
			FAD.Clear();
			fireFixed.Clear();
			string data = SaveGameSlots.LoadDataFromSlot(name, SAVE_NAME);
			if (!string.IsNullOrEmpty(data))
			{
				MelonLogger.Log("JSON loaded " + data);
				var foo = JSON.Load(data);
				foreach (var entry in foo as ProxyObject)
                {
					FireAddonsData lFAD = new FireAddonsData();
					entry.Value.Populate(lFAD);
					FAD.Add(entry.Key, lFAD);
				}
			}
		}

		internal static void SaveData(SaveSlotType gameMode, string name)
		{
			string data = JSON.Dump(FAD, EncodeOptions.NoTypeHints);
			SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, SAVE_NAME, data);
		}

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

			// Flint
			if (gi.name.Contains("GEAR_FlintAndSteel"))
			{
				if (!gi.m_FireStarterItem)
				{
					gi.m_FireStarterItem = gi.gameObject.AddComponent<FireStarterItem>();
				}
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.flintDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.flintStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.flintStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.flintPenalty;
				gi.m_StartCondition = GearStartCondition.Perfect;
				gi.m_WeightKG = 1.5f; // combined weight of components it's made.

			}
			if (gi.name.Contains("GEAR_Firestriker"))
            {
				gi.m_FireStarterItem.m_ConditionDegradeOnUse = Settings.options.firestrikerDegredation;
				gi.m_FireStarterItem.m_SecondsToIgniteTinder = Settings.options.firestrikerStartFire;
				gi.m_FireStarterItem.m_SecondsToIgniteTorch = Settings.options.firestrikerStartTorch;
				gi.m_FireStarterItem.m_FireStartSkillModifier = Settings.options.firestrikerPenalty;
			}
		}

		internal static void Blueprints()
		{
			// Buy 'zeobviouslyfakeacc' a beer as i've used his tincan improvements code
			if (Settings.options.flintEnable)
			{
				// create bp

				BlueprintItem bp_flint = GameManager.GetBlueprints().AddComponent<BlueprintItem>();
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


				if (Settings.options.flintSmeltEnable)
				{
					// re-smelt one
					BlueprintItem bp_flint2 = GameManager.GetBlueprints().AddComponent<BlueprintItem>();
					// Inputs
					bp_flint2.m_RequiredGear = new Il2CppReferenceArray<GearItem>(3) { [0] = GetGearItemPrefab("GEAR_FlintAndSteel"), [1] = GetGearItemPrefab("GEAR_ScrapMetal"), [2] = GetGearItemPrefab("GEAR_Coal") };
					bp_flint2.m_RequiredGearUnits = new Il2CppStructArray<int>(3) { [0] = 1, [1] = 3, [2] = 2 };
					bp_flint2.m_KeroseneLitersRequired = 0f;
					bp_flint2.m_GunpowderKGRequired = 0f;
					bp_flint2.m_RequiredTool = GetToolItemPrefab("GEAR_Hammer");
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
				CalculateFireTimers(__instance, timeDiff, __instance.m_MaxOnTODSeconds - __instance.m_ElapsedOnTODSeconds, true);
				//
				if (__instance.m_Campfire && (__instance.m_MaxOnTODSeconds > __instance.m_ElapsedOnTODSeconds || __instance.m_EmberDurationSecondsTOD > __instance.m_EmberTimer))
				{
					__instance.m_Campfire.m_State = CampfireState.Lit;
				}
				//MelonLogger.Log("restore " + guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString());
			}

		}

		private static void RemoveFireData(Fire __instance, string guid)
        {
			FAD.Remove(guid);
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
				 * however deltatime is in seconds, so whole ember store would be burned in second, not hour and doing 2 converstion (delta time and varitBurnSec) just to look clear is insane
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
			if (!__instance.m_IsPerpetual)
			{
				float deltaTime = GameManager.GetTimeOfDayComponent().GetTODSeconds(Time.deltaTime);
				float remSec = __instance.m_MaxOnTODSeconds - __instance.m_ElapsedOnTODSeconds;
				string guid = Utils.GetGuidFromGameObject(__instance.gameObject);

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
							//MelonLogger.Log("rm " + guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString());
							__instance.m_UseEmbers = false;
							__instance.m_EmberDurationSecondsTOD = 0;
							__instance.m_EmberTimer = 0;
							RemoveFireData(__instance, guid);
						}
					}
					/*
					if (__instance.GetFireState().ToString() != "Off")
					{
						MelonLogger.Log(guid + " burn:" + __instance.m_ElapsedOnTODSeconds + " max:" + __instance.m_MaxOnTODSeconds + " embers:" + __instance.m_EmberDurationSecondsTOD + " ember timer:" + __instance.m_EmberTimer + " reduce2:" + __instance.m_DurationSecondsToReduceToEmbers + " state:" + __instance.GetFireState().ToString() + " " + __instance.m_HeatSource.IsTurnedOn());
					}
					*/
				}
			}
		}
		internal static void CalculateCharcoal(Fire __instance)
        {
			string guid = Utils.GetGuidFromGameObject(__instance.gameObject);
			MelonLogger.Log(guid + " charcoal " + __instance.m_NumGeneratedCharcoalPieces);
			if (FAD.ContainsKey(guid) && __instance.m_NumGeneratedCharcoalPieces <0)
            {
				float tmp = __instance.m_NumGeneratedCharcoalPieces;
				__instance.m_NumGeneratedCharcoalPieces = 0;
				__instance.m_BurningTimeTODHours = 0;
				MelonLogger.Log(guid + " capping charcoal to 0 from " + tmp);
			}
		}
		internal static void FeedFire(Panel_FeedFire __instance)
		{

			Fire _fire = __instance.m_Fire;
			string guid = Utils.GetGuidFromGameObject(_fire.gameObject);
			GearItem fuel = __instance.GetSelectedFuelSource();
			// cool off fire with water
			if (fuel.name.ToLower().Contains("recycledcan") || fuel.name.ToLower().Contains("cookingpot"))
			{

				GearItem gi_wt = GameManager.GetInventoryComponent().GetPotableWaterSupply();
				if (gi_wt.m_WaterSupply.m_VolumeInLiters >= 0.25f && _fire.m_HeatSource.m_MaxTempIncrease > Settings.options.waterTempRemoveDeg)
				{

					GameAudioManager.StopAllSoundsFromGameObject(InterfaceManager.GetSoundEmitter());
					GameAudioManager.PlaySound(gi_wt.m_PickUpAudio, InterfaceManager.GetSoundEmitter());
					gi_wt.m_WaterSupply.m_VolumeInLiters -= 0.25f;
					_fire.ReduceHeatByDegrees(Settings.options.waterTempRemoveDeg);
				}
				else if (gi_wt.m_WaterSupply.m_VolumeInLiters < 0.25f)
				{
					HUDMessage.AddMessage("Need water in inventory.", false);
					GameAudioManager.StopAllSoundsFromGameObject(InterfaceManager.GetSoundEmitter());
					GameAudioManager.PlayGUIError();

				}
				else
				{
					HUDMessage.AddMessage("Temperature is too low.", false);
					GameAudioManager.StopAllSoundsFromGameObject(InterfaceManager.GetSoundEmitter());
					GameAudioManager.PlayGUIError();
				}
				/* we consume can/pot so recreate it
				 * we could hack it to not consume but after some time (minutes) of feed fire dialog, said object is getting corrupted in such way that it's lagging game
				 * on each interaction with said item
				 */
				GearItem clone = Utils.InstantiateGearFromPrefabName(fuel.name);
				clone.m_CurrentHP = fuel.m_CurrentHP;
				GameManager.GetInventoryComponent().AddGear(clone.gameObject);
				GameManager.GetInventoryComponent().DestroyGear(fuel.gameObject);
			}
			// added fuel while embers
			else if (_fire.m_EmberTimer > 0f )
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
			// If charcoal was added, hack charcoal values
			if (fuel.name.ToLower().StartsWith("gear_charcoal"))
            {
				_fire.m_NumGeneratedCharcoalPieces -= 1;
			}

		}
	}
	internal class FireAddonsData
    {
		public int ver { get; set; }
		public float timestamp { get; set; }
		public string fireState { get; set; }
		public float embersSeconds { get; set; }
		public float emberTimer { get; set; }
		public float burnSeconds { get; set; }
		public float burnMaxSeconds { get; set; }
		public float heatTemp { get; set; }
	}
}
