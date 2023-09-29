using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;
using Achid = RainWorld.AchievementID;

namespace AchievementTracker
{
	internal class AchievementDisplay
	{
		public static Dictionary<Achid, AchievementData> achievementdata = new()
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
		public const float sizedecrease = 10;
		public const float sizedecreasehalf = 5;
		public const float achivementheight = 100;

		public Hud achievementHud;
		public Achid achievementID;

		public Vector2 size;
		public Vector2 pos;
		public Vector2 spawnpos;

		public FSprite image;
		public RoundedRect rect;
		public FLabel label;
		public FLabel description;

		public int lifespan;

		public bool delete;

		public string text;
		
		public FContainer Container { get { return achievementHud.pages[0].Container; } }
		public Page Page { get { return achievementHud.pages[0] ; } }

		public AchievementDisplay(Hud hud, Achid id)
		{
			achievementHud = hud;

			text = achievementdata[id].name;
			achievementID = id;
			delete = false;
			lifespan = 0;
			size = new(achievementHud.hudsize - sizedecrease, achivementheight);
			pos = hud.NewAchievementpos;
			pos.x += sizedecreasehalf;
			pos.y -= sizedecreasehalf;
			spawnpos = hud.NewAchievementpos;

			rect = new(achievementHud, Page, pos, size, false);
			image = new FSprite(id.ToString())
			{
				width = 90,
				height = 90
			};
			image.SetAnchor(new Vector2(0, .5f));
			label = new(Custom.GetFont(), text) { alignment = FLabelAlignment.Left };
			description = new(Custom.GetFont(), achievementdata[id].description) { alignment = FLabelAlignment.Left, color = Color.gray };
			AddToContainer();
			Update();
			DrawSprites();
		}

		public virtual void DrawSprites()
		{
			Vector2 newpos = pos + new Vector2(sizedecreasehalf, -sizedecreasehalf);
			Page.subObjects.Add(rect);
			rect.pos = newpos;
			image.SetPosition(newpos + new Vector2(5, rect.size.y / 2f));
			label.SetPosition(image.GetPosition() + new Vector2(image.width, 0));
			description.SetPosition(label.GetPosition() - new Vector2(0, 15));
		}

		public virtual void Update()
		{
			lifespan++;
			float anim = Mathf.Clamp(lifespan, 0f, 100f) / 100f;

			float alpha = Custom.LerpExpEaseOut(0f, 1f, anim);
			rect.fillAlpha = alpha;
			image.alpha = alpha;
			label.alpha = alpha;
			pos.x = Custom.LerpBackEaseOut(spawnpos.x + 500, spawnpos.x, anim);
		}

		public virtual void ClearSprites()
		{
			image?.RemoveFromContainer();
			label?.RemoveFromContainer();
			description?.RemoveFromContainer();
		}
		public virtual void AddToContainer()
		{
			Container.AddChild(image);
			Container.AddChild(label);
			Container.AddChild(description);
		}

		public void Destroy()
		{
			delete = true;
		}
	}
}
