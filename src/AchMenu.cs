using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using RWCustom;
using UnityEngine;
using Achid = RainWorld.AchievementID;

namespace AchievementTracker
{
	internal class AchMenu : Menu.Menu, CheckBox.IOwnCheckBox
	{
		public readonly Menu.Menu connectedMenu;
		public FSprite blackSprite;
		public MenuLabel label;
		public RoundedRect roundedRect;
		public List<AchievementMenuDisplay> displays;
		public List<AchievementMenuDisplay> hiddendisplays;
		public List<Achid> achievements;
		public Vector2 achoffset;
		public int count;
		public int lifespan;

		public VerticalSlider slider;
		public CheckBox ShowAchievedCheckbox;
		public CheckBox ShowUnAchievedCheckbox;
		public CheckBox ShowHiddenCheckbox;
		public CheckBox colorizeCheckbox;
		public CheckBox sortCheckbox;
		public SimpleButton backButton;

		public bool delete;
		public bool shutting;
		public int shuttingcounter;

		public float scrollval;
		public float scrollin;


		public Vector2 NewAchievementpos
		{
			get
			{
				if (displays.Count > 0)
				{
					return displays.Last().spawnpos - new Vector2(0, 105);
				}
				return roundedRect.pos + new Vector2(0, roundedRect.size.y - 100);
			}
		}

		public override void SliderSetValue(Slider slider, float f)
		{
			scrollval = 1F - f;
		}

		public override float ValueOfSlider(Slider slider)
		{
			return 1f - scrollval;
		}

		public AchMenu(ProcessManager manager, Menu.Menu menu) : base(manager, CustomIds.menu)
		{
			connectedMenu = menu;
			delete = false;
			shutting = false;
			shuttingcounter = 10;
			displays = new();
			hiddendisplays = new();
			achievements = new();
			count = 0;
			pages.Add(new Page(this, null, "main", 0));
			blackSprite = new("pixel", true)
			{
				color = MenuRGB(MenuColors.Black),
				scaleX = 1400f,
				scaleY = 800f,
				x = manager.rainWorld.options.ScreenSize.x / 2f,
				y = manager.rainWorld.options.ScreenSize.y / 2f,
				alpha = menu is null ? 0f : .5f
			};
			pages[0].Container.AddChild(blackSprite);

			achoffset = Vector2.zero;

			int amount = CustomIds.achievementdata.Count;
			int collected = Plugin.achievements.Count;
			Vector2 screensize = manager.rainWorld.options.ScreenSize;
			Vector2 saveoffset = manager.rainWorld.options.SafeScreenOffset;
			label = new(this, pages[0], string.Format("{0} out of {1} achievements" + (amount == collected ? ". Great Job!" : ""), new object[] { collected, amount }), new(0, screensize.y - 20), new(screensize.x / 3f, -20), false);
			roundedRect = new(this, pages[0], new Vector2(screensize.x / 3f, 0), new Vector2(screensize.x / 3f - saveoffset.x, screensize.y - saveoffset.y), true);
			slider = new(this, pages[0], "", new(roundedRect.pos.x - 30, 10), new(15, screensize.y - 40), new("AchSlider", true), false);
			colorizeCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 50), 40f, "Colors", "ACHT_COLORS", true);
			ShowAchievedCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 100), 40f, "Show Achieved", "ACHT_ACHIEVED", true);
			ShowUnAchievedCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 150), 40f, "Show Unachieved", "ACHT_UNACHIEVED", true);
			ShowHiddenCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 200), 40f, "Show Hidden", "ACHT_HIDDEN", true);
			sortCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 250), 40f, "Sort by name", "ACHT_SORT", true);

			backButton = new SimpleButton(this, pages[0], Translate("Back"), "BACK", new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
			SliderSetValue(slider, 1f);
			scrollin = 0f;
			PlaySound(SoundID.HUD_Pause_Game);
			MutualHorizontalButtonBind(backButton, slider);
			MutualHorizontalButtonBind(ShowAchievedCheckbox, slider);
			MutualHorizontalButtonBind(ShowUnAchievedCheckbox, slider);
			MutualHorizontalButtonBind(ShowHiddenCheckbox, slider);
			MutualHorizontalButtonBind(sortCheckbox, slider);
			MutualHorizontalButtonBind(colorizeCheckbox, slider);

			MutualVerticalButtonBind(ShowAchievedCheckbox, colorizeCheckbox);
			MutualVerticalButtonBind(ShowUnAchievedCheckbox, ShowAchievedCheckbox);
			MutualVerticalButtonBind(ShowHiddenCheckbox, ShowUnAchievedCheckbox);
			MutualVerticalButtonBind(sortCheckbox, ShowHiddenCheckbox);

			MutualVerticalButtonBind(backButton, sortCheckbox);
			MutualVerticalButtonBind(colorizeCheckbox, backButton);

			backObject = backButton;
			pages[0].subObjects.Add(label);
			pages[0].subObjects.Add(roundedRect);
			pages[0].subObjects.Add(slider);
			pages[0].subObjects.Add(colorizeCheckbox);
			pages[0].subObjects.Add(ShowAchievedCheckbox);
			pages[0].subObjects.Add(ShowUnAchievedCheckbox);
			pages[0].subObjects.Add(ShowHiddenCheckbox);
			pages[0].subObjects.Add(sortCheckbox);
			pages[0].subObjects.Add(backButton);

            UpdateItems();
        }
		public override void Singal(MenuObject sender, string message)
		{
			base.Singal(sender, message);
			if (message == "BACK")
			{
				if (connectedMenu is PauseMenu menu)
				{
					menu.allowSelectMove = true;
					shutting = true;
					//ShutDownProcess();
				} else
				{
					manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
				}
			}
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
		}

		public override void Update()
		{
			if (connectedMenu is PauseMenu menu && !delete)
			{
				if (menu.wantToContinue)
				{
					ShutDownProcess();
				}
				menu.allowSelectMove = false;
				menu.ResetSelection();
				//menu.selectedObject = null;
			}
			if (shutting)
			{
				shuttingcounter--;
				if (shuttingcounter <= 0)
				{
					ShutDownProcess();
				}
			}
			scrollin *= .8f;
			scrollval += scrollin;
			if (scrollval > 1) scrollval = Mathf.LerpUnclamped(scrollval, 1, .2f);
			if (scrollval < 0) scrollval = Mathf.LerpUnclamped(scrollval, 0, .2f);
			lifespan++;
			float alpha = Custom.LerpExpEaseOut(0f, 1f, Mathf.Clamp(lifespan, 0, 60) / 60f);
			alpha *= shuttingcounter / 10f;
			roundedRect.fillAlpha = alpha;
			blackSprite.alpha = connectedMenu is null ? 0f : alpha / 2f;
			label.label.alpha = alpha;
			count++;
			if (achievements.Count > 0 && count > 2)
			{
				Achid id = achievements[0];
				AchievementMenuDisplay display = new(this, pages[0], id, !Plugin.achievements.Contains(id));
				displays.Add(display);
				pages[0].subObjects.Add(display);
				achievements.Remove(id);
				count = 0;
			}
			int amount = displays.Count + achievements.Count;
			float len = amount * 105;
			achoffset.y = scrollval * (len - manager.rainWorld.options.ScreenSize.y + 5);
			if (len < manager.rainWorld.screenSize.y) scrollval = Mathf.Lerp(scrollval, 0, 0.2f);


			if (mouseScrollWheelMovement != 0)
			{
				scrollin = mouseScrollWheelMovement / (amount * 2f);
			}
            for (int i = hiddendisplays.Count - 1; i >= 0; i--)
            {
				if (hiddendisplays[i].delete)
				{
					hiddendisplays[i].RemoveSprites();
					pages[0].RemoveSubObject(hiddendisplays[i]);
				}
			}
			hiddendisplays.RemoveAll(match => match.delete);
            base.Update();
		}
		public override void ShutDownProcess()
		{
			blackSprite?.RemoveFromContainer();
            foreach (var display in displays)
            {
				display.RemoveSprites();
				pages[0].RemoveSubObject(display);
			}
			foreach (var display in hiddendisplays)
			{
				display.RemoveSprites();
				pages[0].RemoveSubObject(display);
			}

			displays.Clear();
			hiddendisplays.Clear();
			delete = true;
			base.ShutDownProcess();
		}

		public void UpdateItems()
		{
			hiddendisplays.AddRange(displays);
            foreach (var item in hiddendisplays)
            {
				item.deleteFromList = true;
            }
            displays.Clear();
			achievements.Clear();
            for (int i = 0; i < CustomIds.achievementdata.Count; i++)
            {
				Achid id = CustomIds.achievementdata.Keys.ToList()[i];
				AchievementData data = CustomIds.achievementdata[id];
				//((ShowAchieved && data.Achieved) || (ShowUnAchieved && !data.Achieved)) && (ShowHidden && data.hidden || !ShowHidden);
				bool canadd = false;
				if (data.Achieved && Plugin.ShowAchieved) canadd = true;
				if (!data.Achieved && Plugin.ShowUnAchieved) canadd = true;
				if (data.hidden && !Plugin.ShowHidden) canadd = false;
				if (canadd) achievements.Add(id);
            }
			if (Plugin.sort)
			{
				achievements.Sort(delegate (Achid x, Achid y)
				{
					return Translate(CustomIds.achievementdata[x].name).CompareTo(Translate(CustomIds.achievementdata[y].name));
				});
			}
		}

		public bool GetChecked(CheckBox box)
		{
			return box.IDString switch
			{
				"ACHT_COLORS" => Plugin.colorize,
				"ACHT_ACHIEVED" => Plugin.ShowAchieved,
				"ACHT_UNACHIEVED" => Plugin.ShowUnAchieved,
				"ACHT_HIDDEN" => Plugin.ShowHidden,
				"ACHT_SORT" => Plugin.sort,
				_ => false,
			};
		}

		public void SetChecked(CheckBox box, bool c)
		{
			switch (box.IDString)
			{
				case "ACHT_COLORS":
					Plugin.colorize = c;
                    foreach (var item in displays)
                    {
						item.colorize = c;
                    }
                    break;
				case "ACHT_ACHIEVED":
					Plugin.ShowAchieved = c;
					UpdateItems();
					break;
				case "ACHT_UNACHIEVED":
					Plugin.ShowUnAchieved = c;
					UpdateItems();
					break;
				case "ACHT_HIDDEN":
					Plugin.ShowHidden = c;
					UpdateItems();
					break;
				case "ACHT_SORT":
					Plugin.sort = c;
					UpdateItems();
					break;
			}
		}
	}
}
