using ModSettings;
using System.Reflection;

namespace FireAddons
{
	internal class FireAddonsSettings : JsonModSettings
	{
		[Section("Lantern")]
		[Name("Use Lantern to start fire")]
		[Description("Suggested value: yes")]
		public bool lanternUse = true;

		[Name("Lantern: time to light fire")]
		[Description("Suggested value 5 for 'mechanical'; 0 for 'accelerated'.")]
		[Slider(0, 40)]
		public int lanternStartFire = 5;

		[Name("Lantern: time to light torch")]
		[Description("Suggested value 5 for 'mechanical'; 0 for 'accelerated'.")]
		[Slider(0, 40)]
		public int lanternStartTorch = 5;

		[Name("Lantern: fire start chance modification")]
		[Description("Suggested value -10 for 'mechanical'; +20 for 'accelerated'.")]
		[Slider(-30, 30)]
		public int lanternPenalty = -10;

		[Name("Lantern: degredation")]
		[Description("Suggested value 0 for 'mechanical'; any positive for 'accelerated'.")]
		[Slider(0, 20)]
		public int lanternDegredation = 0;


		[Section("Lenses modifiers")]
		[Name("Lenses: time to light fire")]
		[Description("Default: 10.")]
		[Slider(0, 40)]
		public int lensesStartFire = 10;

		[Name("Lenses: time to light torch")]
		[Description("Default: 10.")]
		[Slider(0, 40)]
		public int lensesStartTorch = 10;

		[Name("Lenses: fire start chance modification")]
		[Description("Default: 10.")]
		[Slider(-30, 30)]
		public int lensesPenalty = 10;

		[Name("Lenses: degredation")]
		[Description("Default: 0.")]
		[Slider(0, 20)]
		public int lensesDegredation = 0;


		[Section("Firestriker modifiers")]
		[Name("Firestriker: time to light fire")]
		[Description("Default: 3.")]
		[Slider(0, 40)]
		public int firestrikerStartFire = 3;

		[Name("Firestriker: time to light torch")]
		[Description("Default: 3.")]
		[Slider(0, 40)]
		public int firestrikerStartTorch = 3;

		[Name("Firestriker: fire start chance modification")]
		[Description("Default: 20.")]
		[Slider(-30, 30)]
		public int firestrikerPenalty = 20;

		[Name("Firestriker: degredation")]
		[Description("Default: 2.")]
		[Slider(0, 20)]
		public int firestrikerDegredation = 2;


		[Section("Tinder")]
		[Name("Tinder as Fuel")]
		[Description("Use tinder as fuel (not during starting fire). Vanilla: no, suggested yes.")]
		public bool tinderAsFuel = true;

		[Name("Tinder fuel time")]
		[Description("In minutes. Vanilla: 0, suggested 1-2.")]
		[Slider(0, 30)]
		public int tinderFuel = 2;

		[Name("Tinder fuel temperature")]
		[Description("Increase of temp in Celsius. Vanilla: 0, suggested 3-5.")]
		[Slider(0, 30)]
		public int tinderFueldeg = 5;

		[Name("Tinder fire start modifications")]
		[Description("Chane fire start values for tinders. Vanilla: no, suggested: yes.")]
		public bool tinderMatters = true;

		[Name("Tinder Plug: fire start modification")]
		[Description("Suggested value 1.")]
		[Slider(-20, 10)]
		public int tinderBonusPlug = 1;

		[Name("Newsprint: fire start modification")]
		[Description("Suggested value 2.")]
		[Slider(-20, 10)]
		public int tinderBonusNewsprint = 2;

		[Name("Newsprint Roll: fire start modification")]
		[Description("Suggested value 3.")]
		[Slider(-20, 10)]
		public int tinderBonusNewsprintRoll = 3;

		[Name("Paper Stack: fire start modification")]
		[Description("Suggested value 2.")]
		[Slider(-20, 10)]
		public int tinderBonusPaper = 2;

		[Name("Cash: fire start modification")]
		[Description("Suggested value 2.")]
		[Slider(-20, 10)]
		public int tinderBonusCash = 2;

		[Name("Bark: fire start modification")]
		[Description("Suggested value 5.")]
		[Slider(-20, 10)]
		public int tinderBonusBark = 5;

		[Name("Cattail Head: fire start modification")]
		[Description("Suggested value 1.")]
		[Slider(-20, 10)]
		public int tinderBonusCattail = 1;


		[Section("Embers")]
		[Name("Reworked embers (smoldering fuel)")]
		[Description("Enable reworked embers (smoldering fuel) system - see Readme on github for exact details.")]
		public bool embersSystem = true;

		[Name("Embers max time")]
		[Description("Maximum time embers will last, in hours.")]
		[Slider(0f, 12f)]
		public float embersTime = 8f;

		[Name("Burnout temperature")]
		[Description("Temperature above which fuel will be burning out from ember store. See next setting.")]
		[Slider(20, 50)]
		public int embersBunoutTemp = 30;

		[Name("Burnout ratio")]
		[Description("Ratio of ember (max) time is removed (burnt) in one hour, at 80C. Burnt embers will extend burning time of fire.")]
		[Slider(0.1f, 1f, NumberFormat = "{0:F2}")]
		public float embersBunoutRatio = 0.5f;

		[Name("Fuel to embers ratio")]
		[Description("Fraction of fuel is transfered to embers on fuel add. Used with next settings.")]
		[Slider(0.05f, 0.5f, NumberFormat = "{0:F2}")]
		public float embersFuelRatio = 0.2f;

		[Name("Fuel to embers exchange")]
		[Description("Multiplier of above fraction, to be added to ember timer.")]
		[Slider(1f, 5f)]
		public float embersFuelEx = 2f;

		[Name("Snow cools fire by xC")]
		[Description("Amount of deg, fire will cooldown using snow.")]
		[Slider(1, 30)]
		public int waterTempRemoveDeg = 15;

		[Name("Prevent cooking while in ember state")]
		[Description("Will disable cooking/boiling while stove/campfire is in 'ember state' when new ember/smoldering mechanic is used. Setting this to false, will make new mechanic useless and they will only 'extend burn time' (yet this ability was requiesed). Recommended: true.")]
		public bool embersSystemNoCooking = true;

		[Name("Use Charcoal as fuel")]
		[Description("Allow using charcal as fuel. Set to false if other mod is adding charcoal as fuel")]
		public bool burnCharcoal = true;

		[Name("... burn time")]
		[Description("Recommended 20 min.")]
		[Slider(5, 40)]
		public int burnCharcoalTime = 20;

		[Name("... temp increase")]
		[Description("Recommended 5 deg")]
		[Slider(0, 15)]
		public int burnCharcoalTemp = 5;

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
			if (embersSystem)
            {
				SetFieldVisible(nameof(embersTime), true);
				SetFieldVisible(nameof(embersBunoutTemp), true);
				SetFieldVisible(nameof(embersBunoutRatio), true);
				SetFieldVisible(nameof(embersFuelRatio), true);
				SetFieldVisible(nameof(embersFuelEx), true);
				SetFieldVisible(nameof(waterTempRemoveDeg), true);
				SetFieldVisible(nameof(embersSystemNoCooking), true);
			}
			else
            {
				SetFieldVisible(nameof(embersTime), false);
				SetFieldVisible(nameof(embersBunoutTemp), false);
				SetFieldVisible(nameof(embersBunoutRatio), false);
				SetFieldVisible(nameof(embersFuelRatio), false);
				SetFieldVisible(nameof(embersFuelEx), false);
				SetFieldVisible(nameof(waterTempRemoveDeg), false);
				SetFieldVisible(nameof(embersSystemNoCooking), false);

			}
			if (burnCharcoal)
            {
				SetFieldVisible(nameof(burnCharcoalTime), true);
				SetFieldVisible(nameof(burnCharcoalTemp), true);
			}
			else
            {
				SetFieldVisible(nameof(burnCharcoalTime), false);
				SetFieldVisible(nameof(burnCharcoalTemp), false);
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