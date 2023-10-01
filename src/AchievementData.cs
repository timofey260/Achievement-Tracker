using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Achid = RainWorld.AchievementID;

namespace AchievementTracker
{
	internal class AchievementData
	{
		public string name;
		public string description;
		public bool hidden;
		public Achid id;

		public bool Achieved => Plugin.achievements.Contains(id);

		public AchievementData(string name, string description, bool hidden, Achid id)
		{
			this.name = name;
			this.description = description;
			this.hidden = hidden;
			this.id = id;
		}
	}
}
