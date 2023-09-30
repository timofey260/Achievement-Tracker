using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procid = ProcessManager.ProcessID;
using Achid = RainWorld.AchievementID;

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
			achievementdata = new()
			{
				{Achid.PassageSurvivor, new("The Survivor", "This land has become your home", false)},
				{Achid.PassageHunter, new("The Hunter", "Path of the carnivore", false)},
				{Achid.PassageMonk, new("The Monk", "Path of the vegetarian", false)},
				{Achid.PassageSaint, new("The Saint", "Path of the pacifist", false)},
				{Achid.PassageOutlaw, new("The Outlaw", "Path of aggression", false)},
				{Achid.PassageDragonSlayer, new("The Dragon Slayer", "Predator has become prey", false)},
				{Achid.PassageChieftain, new("The Chieftain", "Embraced by a new tribe", false)},
				{Achid.PassageTraveller, new("The Wanderer", "Your travels have taken you far", false)},
				{Achid.PassageScholar, new("The Scholar", "History lessons", false)},
				{Achid.PassageFriend, new("The Friend", "One need not travel alone", false)},
				{Achid.GhostCC, new("Nineteen Spades, Endless Reflections", "", true)},
				{Achid.GhostSI, new("Droplets upon Five Large Droplets", "", true)},
				{Achid.GhostLF, new("A Bell, Eighteen Amber Beads", "", true)},
				{Achid.GhostSH, new("Four Needles under Plentiful Leaves", "", true)},
				{Achid.GhostUW, new("Six Grains of Gravel, Mountains Abound", "", true)},
				{Achid.GhostSB, new("Two Sprouts, Twelve Brackets", "", true)},
				{Achid.AllGhostsEncountered, new("Pilgrimage", "", true)},
				{Achid.MoonEncounterBad, new("Stolen Enlightenment", "", true)},
				{Achid.MoonEncounterGood, new("A New Friend", "", true)},
				{Achid.PebblesEncounter, new("The Journey", "", true)},
				{Achid.Win, new("Ascension", "", true)},
				{Achid.HunterPayload, new("A Helping Hand", "", true)},
				{Achid.HunterWin, new("Within Time", "", true)},
				{Achid.GourmandEnding, new("Migration", "", true)},
				{Achid.ArtificerEnding, new("Closure", "", true)},
				{Achid.RivuletEnding, new("An Old Friend", "", true)},
				{Achid.SpearmasterEnding, new("Messenger", "", true)},
				{Achid.SaintEnding, new("The Cycle", "", true)},
				{Achid.ChallengeMode, new("Champion", "", true)},
				{Achid.Quests, new("Expedition Leader", "", true)},
				{Achid.PassageMartyr, new("The Martyr", "", true)},
				{Achid.PassageNomad, new("The Nomad", "", true)},
				{Achid.PassagePilgrim, new("The Pilgrim", "", true)},
				{Achid.PassageMother, new("The Mother", "", true)},
			};
		}
		internal static Dictionary<Achid, AchievementData> achievementdata;
	}
}
