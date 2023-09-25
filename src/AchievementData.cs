using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementTracker
{
	internal class AchievementData
	{
		public string name;
		public string description;
		public bool hidden;

		public AchievementData(string name, string description, bool hidden)
		{
			this.name = name;
			this.description = description;
			this.hidden = hidden;
		}
	}
}
