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
		[Description("use tinder as fuel when firestarting level 3 is reach. Suggested yes!")]
		public bool tinderAsFuel = true;

		[Name("FORCE Tinder as Fuel")]
		[Description("In case You're lev3 and new items are still tinder")]
		public bool tinderAsFuelForced = false;

		[Name("Tinder fuel value")]
		[Description("In minutes, suggested 5")]
		[Slider(1, 60)]
		public int tinderFuel = 5;

		[Name("Tinder base fire start offset")]
		[Description("Suggested value 10 - this is to compensate lack of proper fuel. Book is 30, softwood is 10. This will be at level 3 so your base is higher.")]
		[Slider(0, 30)]
		public int tinderBonusOffset = 10;

		[Name("Tinder Plug: fire start Modification")]
		[Description("Suggested value 3")]
		[Slider(-20, 10)]
		public int tinderBonusPlug = 3;

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


		[Section("Other")]
		[Name("Flint degredation")]
		[Description("Suggested value 0.2. Can be crafted from prybar, coal and whetstone.")]
		[Slider(0f, 20f)]
		public float flintDegredation = 0.2f;




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
				SetFieldVisible(nameof(tinderBonusPlug), true);
				SetFieldVisible(nameof(tinderBonusPaper), true);
				SetFieldVisible(nameof(tinderBonusOffset), true);
				SetFieldVisible(nameof(tinderBonusNewsprintRoll), true);
				SetFieldVisible(nameof(tinderBonusNewsprint), true);
				SetFieldVisible(nameof(tinderBonusCattail), true);
				SetFieldVisible(nameof(tinderBonusCash), true);
				SetFieldVisible(nameof(tinderBonusBark), true);
			}
			else
            {
				SetFieldVisible(nameof(tinderFuel), false);
				SetFieldVisible(nameof(tinderBonusPlug), false);
				SetFieldVisible(nameof(tinderBonusPaper), false);
				SetFieldVisible(nameof(tinderBonusOffset), false);
				SetFieldVisible(nameof(tinderBonusNewsprintRoll), false);
				SetFieldVisible(nameof(tinderBonusNewsprint), false);
				SetFieldVisible(nameof(tinderBonusCattail), false);
				SetFieldVisible(nameof(tinderBonusCash), false);
				SetFieldVisible(nameof(tinderBonusBark), false);

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