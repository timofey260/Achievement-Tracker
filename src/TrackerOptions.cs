using BepInEx;
using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementTracker
{
	internal class TrackerOptions : OptionInterface
	{
		public TrackerOptions(BaseUnityPlugin plugin)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
			OpTab tab1 = new(this, "options");
			Tabs = new OpTab[] { tab1 };
		}
	}
}
