using ModSettings;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FireAddons
{
	internal class FireAddonsSettings : JsonModSettings
	{
		[Section("Lantern")]
		[Name("Use Lantern to start fire")]
		[Description("Suggested value: yes")]
		public bool lanternUse = true;

		[Name("Lantern: time to light fire")]
		[Description("Suggested value 5 for 'mechanical'; 0 for 'accelerated'")]
		[Slider(0, 10)]
		public int lanternStartFire = 5;

		[Name("Lantern: time to light torch")]
		[Description("Suggested value 5 for 'mechanical'; 0 for 'accelerated'")]
		[Slider(0, 10)]
		public int lanternStartTorch = 5;

		[Name("Lantern: fire start modification")]
		[Description("Suggested value -10 for 'mechanical'; +20 for 'accelerated'")]
		[Slider(-20, 40)]
		public int lanternPenalty = -10;

		[Name("Lantern: degredation")]
		[Description("Suggested value 0 for 'mechanical'; any positive for 'accelerated'")]
		[Slider(0, 20)]
		public int lanternDegredation = 0;

		[Section("Tinder")]
		[Name("Tinder as Fuel")]
		[Description("Use tinder as fuel (not during starting fire). Suggested yes")]
		public bool tinderAsFuel = true;

		[Name("Tinder fuel time")]
		[Description("In minutes, suggested 5")]
		[Slider(1, 60)]
		public int tinderFuel = 5;

		[Name("Tinder fuel temperature")]
		[Description("Increase of temp in Celsius, suggested 5")]
		[Slider(1, 60)]
		public int tinderFueldeg = 5;

		[Name("Tinder fire start modifications")]
		[Description("Different tinder chnage chance of fire start. Suggested yes")]
		public bool tinderMatters = true;

		[Name("Tinder Plug: fire start Modification")]
		[Description("Suggested value 1")]
		[Slider(-20, 10)]
		public int tinderBonusPlug = 1;

		[Name("Newsprint: fire start Modification")]
		[Description("Suggested value 2")]
		[Slider(-20, 10)]
		public int tinderBonusNewsprint = 2;

		[Name("Newsprint Roll: fire start Modification")]
		[Description("Suggested value 3")]
		[Slider(-20, 10)]
		public int tinderBonusNewsprintRoll = 3;

		[Name("Paper Stack: fire start Modification")]
		[Description("Suggested value 2")]
		[Slider(-20, 10)]
		public int tinderBonusPaper = 2;

		[Name("Cash: fire start Modification")]
		[Description("Suggested value 2")]
		[Slider(-20, 10)]
		public int tinderBonusCash = 2;

		[Name("Bark: fire start Modification")]
		[Description("Suggested value 5")]
		[Slider(-20, 10)]
		public int tinderBonusBark = 5;

		[Name("Cattail Head: fire start Modification")]
		[Description("Suggested value 1")]
		[Slider(-20, 10)]
		public int tinderBonusCattail = 1;


		[Section("Flint")]
		[Name("Flint crafting enabled")]
		public bool flintEnable = true;

		[Name("Flint resmelt enabled")]
		[Description("allow re-smelting flints steel as repair process")]
		public bool flintSmeltEnable = true;

		[Name("Flint degredation")]
		[Description("Suggested value 0.2. Can be crafted from prybar, coal and whetstone.")]
		[Slider(0f, 20f)]
		public float flintDegredation = 0.2f;

		[Section("Embers")]
		[Name("Reworked embers")]
		[Description("Enable reworked embers system - see Readme")]
		public bool embersSystem = true;

		[Name("Embers max time")]
		[Description("Maximum time embers will last, in hours")]
		[Slider(0f, 12f)]
		public float embersTime = 8f;

		[Name("Burnout temperature")]
		[Description("Temperature above which fuel will be burning out from ember store. See next setting")]
		[Slider(20, 50)]
		public int embersBunoutTemp = 30;

		[Name("Burnout ratio")]
		[Description("Ratio of ember (max) time is removed (burnt) in one hour, at 80C. Burnt embers will extend burning time of fire.")]
		[Slider(0.1f, 1f, NumberFormat = "{0:F2}")]
		public float embersBunoutRatio = 0.5f;

		[Name("Fuel to embers ratio")]
		[Description("Ratio of fuel is transfered to embers on fuel add")]
		[Slider(0.05f, 0.5f, NumberFormat = "{0:F2}")]
		public float embersFuelRatio = 0.2f;

		[Name("Fuel to embers exchange")]
		[Description("Ratio of fuel is transfered to embers on fuel add")]
		[Slider(1f, 5f)]
		public float embersFuelEx = 2f;

		[Name("Water fire cooldown")]
		[Description("Amount of deg, fire will cooldown per 250ml of water. NOTE: in 'Add Fuel' it will show as can/pot as water directly can't be used.")]
		[Slider(1, 30)]
		public int waterTempRemoveDeg = 15;





		protected override void OnChange(FieldInfo field, object oldValue, object newValue)
		{
			RefreshFields();
		}

		internal void RefreshFields()
		{
			if (lanternUse)
			{

				SetFieldVisible(nameof(lanternStartFire), true);
				SetFieldVisible(nameof(lanternStartTorch), true);
				SetFieldVisible(nameof(lanternPenalty), true);
				SetFieldVisible(nameof(lanternDegredation), true);

			}
			else
			{
				SetFieldVisible(nameof(lanternStartFire), false);
				SetFieldVisible(nameof(lanternStartTorch), false);
				SetFieldVisible(nameof(lanternPenalty), false);
				SetFieldVisible(nameof(lanternDegredation), false);
			}
			if (tinderAsFuel)
			{
				SetFieldVisible(nameof(tinderFuel), true);
				SetFieldVisible(nameof(tinderFueldeg), true);
			}
			else
			{
				SetFieldVisible(nameof(tinderFuel), false);
				SetFieldVisible(nameof(tinderFueldeg), false);
			}
			if (tinderMatters)
            {
				SetFieldVisible(nameof(tinderBonusPlug), true);
				SetFieldVisible(nameof(tinderBonusPaper), true);
				SetFieldVisible(nameof(tinderBonusNewsprintRoll), true);
				SetFieldVisible(nameof(tinderBonusNewsprint), true);
				SetFieldVisible(nameof(tinderBonusCattail), true);
				SetFieldVisible(nameof(tinderBonusCash), true);
				SetFieldVisible(nameof(tinderBonusBark), true);
			}
			else
			{
				SetFieldVisible(nameof(tinderBonusPlug), false);
				SetFieldVisible(nameof(tinderBonusPaper), false);
				SetFieldVisible(nameof(tinderBonusNewsprintRoll), false);
				SetFieldVisible(nameof(tinderBonusNewsprint), false);
				SetFieldVisible(nameof(tinderBonusCattail), false);
				SetFieldVisible(nameof(tinderBonusCash), false);
				SetFieldVisible(nameof(tinderBonusBark), false);

			}
			if (flintEnable)
            {
				SetFieldVisible(nameof(flintSmeltEnable), true);
				SetFieldVisible(nameof(flintDegredation), true);
			}
			else
            {
				SetFieldVisible(nameof(flintSmeltEnable), false);
				SetFieldVisible(nameof(flintDegredation), false);
			}
		}
	}
	internal static class Settings
	{
		public static FireAddonsSettings options;
		public static void OnLoad()
		{
			options = new FireAddonsSettings();
			options.RefreshFields();
			options.AddToModSettings("Fire Addons Settings");
		}
	}
}