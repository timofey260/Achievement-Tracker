using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procid = ProcessManager.ProcessID;

namespace AchievementTracker
{
	public static class CustomIds
	{
		public static Procid menu;
		public static Procid hud;
		public static Procid loop;

		public static void Initialize()
		{
			loop = new("AchievementMenuLoop", true);
			menu = new("AchievementMenu", true);
			hud = new("AchievementHud", true);
		}
	}
}
