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
				{Achid.PassageSurvivor, new("The Survivor", "This land has become your home", false, Achid.PassageSurvivor)},
				{Achid.PassageHunter, new("The Hunter", "Path of the carnivore", false, Achid.PassageHunter)},
				{Achid.PassageMonk, new("The Monk", "Path of the vegetarian", false, Achid.PassageMonk)},
				{Achid.PassageSaint, new("The Saint", "Path of the pacifist", false, Achid.PassageSaint)},
				{Achid.PassageOutlaw, new("The Outlaw", "Path of aggression", false, Achid.PassageOutlaw)},
				{Achid.PassageDragonSlayer, new("The Dragon Slayer", "Predator has become prey", false, Achid.PassageDragonSlayer)},
				{Achid.PassageChieftain, new("The Chieftain", "Embraced by a new tribe", false, Achid.PassageChieftain)},
				{Achid.PassageTraveller, new("The Wanderer", "Your travels have taken you far", false, Achid.PassageTraveller)},
				{Achid.PassageScholar, new("The Scholar", "History lessons", false, Achid.PassageScholar)},
				{Achid.PassageFriend, new("The Friend", "One need not travel alone", false, Achid.PassageFriend)},
				{Achid.GhostCC, new("Nineteen Spades, Endless Reflections", "", true, Achid.GhostCC)},
				{Achid.GhostSI, new("Droplets upon Five Large Droplets", "", true, Achid.GhostSI)},
				{Achid.GhostLF, new("A Bell, Eighteen Amber Beads", "", true, Achid.GhostLF)},
				{Achid.GhostSH, new("Four Needles under Plentiful Leaves", "", true, Achid.GhostSH)},
				{Achid.GhostUW, new("Six Grains of Gravel, Mountains Abound", "", true, Achid.GhostUW)},
				{Achid.GhostSB, new("Two Sprouts, Twelve Brackets", "", true, Achid.GhostSB)},
				{Achid.AllGhostsEncountered, new("Pilgrimage", "", true, Achid.AllGhostsEncountered)},
				{Achid.MoonEncounterBad, new("Stolen Enlightenment", "", true, Achid.MoonEncounterBad)},
				{Achid.MoonEncounterGood, new("A New Friend", "", true, Achid.MoonEncounterGood)},
				{Achid.PebblesEncounter, new("The Journey", "", true, Achid.PebblesEncounter)},
				{Achid.Win, new("Ascension", "", true, Achid.Win)},
				{Achid.HunterPayload, new("A Helping Hand", "", true, Achid.HunterPayload)},
				{Achid.HunterWin, new("Within Time", "", true, Achid.HunterWin)},
				{Achid.GourmandEnding, new("Migration", "", true, Achid.GourmandEnding)},
				{Achid.ArtificerEnding, new("Closure", "", true, Achid.ArtificerEnding)},
				{Achid.RivuletEnding, new("An Old Friend", "", true, Achid.RivuletEnding)},
				{Achid.SpearmasterEnding, new("Messenger", "", true, Achid.SpearmasterEnding)},
				{Achid.SaintEnding, new("The Cycle", "", true, Achid.SaintEnding)},
				{Achid.ChallengeMode, new("Champion", "", true, Achid.ChallengeMode)},
				{Achid.Quests, new("Expedition Leader", "", true, Achid.Quests)},
				{Achid.PassageMartyr, new("The Martyr", "", true, Achid.PassageMartyr)},
				{Achid.PassageNomad, new("The Nomad", "", true, Achid.PassageNomad)},
				{Achid.PassagePilgrim, new("The Pilgrim", "", true, Achid.PassagePilgrim)},
				{Achid.PassageMother, new("The Mother", "", true, Achid.PassageMother)},
			};
		}
		internal static Dictionary<Achid, AchievementData> achievementdata;
	}
}
