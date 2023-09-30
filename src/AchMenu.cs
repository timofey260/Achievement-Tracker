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
		public List<Achid> achievements;
		public Vector2 achoffset;
		public int count;
		public int lifespan;
		public AchSlider slider;
		public CheckBox OnlyAchievedCheckbox;
		public CheckBox colorizeCheckbox;
		public SimpleButton backButton;
		public bool delete;

		public float scrollval;
		public bool colorize;
		public bool onlyAchieved;

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
			displays = new();
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
				alpha = menu is AchievementLoop ? 0f : .5f
			};
			pages[0].Container.AddChild(blackSprite);

			achoffset = Vector2.zero;

			int amount = CustomIds.achievementdata.Count;
			int collected = Plugin.achievements.Count;
			Vector2 screensize = manager.rainWorld.options.ScreenSize;
			Vector2 saveoffset = manager.rainWorld.options.SafeScreenOffset;
			label = new(this, pages[0], string.Format("{0} out of {1} achievements" + (amount == collected ? ". Great Job!" : ""), new object[] { collected, amount }), new(0, screensize.y - 20), new(screensize.x / 3f, -20), false);
			roundedRect = new(this, pages[0], new Vector2(screensize.x / 3f, 0), new Vector2(screensize.x / 3f - saveoffset.x, screensize.y - saveoffset.y), true);
			slider = new(this, pages[0], "text", new(roundedRect.pos.x - 30, 10), new(15, screensize.y - 40));
			colorizeCheckbox = new(this, pages[0], this, new(saveoffset.x + 10, screensize.y - saveoffset.y - 50), 40f, "Colors", "ACHT_COLORS", true);
			backButton = new SimpleButton(this, pages[0], Translate("Back"), "BACK", new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
			SliderSetValue(slider, 1f);
			PlaySound(SoundID.HUD_Pause_Game);
			MutualHorizontalButtonBind(backButton, slider);
			MutualHorizontalButtonBind(colorizeCheckbox, slider);
			MutualVerticalButtonBind(backButton, colorizeCheckbox);

			backObject = backButton;
			pages[0].subObjects.Add(label);
			pages[0].subObjects.Add(roundedRect);
			pages[0].subObjects.Add(slider);
			pages[0].subObjects.Add(colorizeCheckbox);
			pages[0].subObjects.Add(backButton);

            for (int i = 0; i < CustomIds.achievementdata.Count; i++)
            {
				Achid id = CustomIds.achievementdata.Keys.ToList()[i];
				achievements.Add(id);
            }
        }
		public override void Singal(MenuObject sender, string message)
		{
			base.Singal(sender, message);
			if (message == "BACK")
			{
				if (manager.currentMainLoop.ID == CustomIds.loop)
				{
					manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
				} else
				{
					delete = true;
					ShutDownProcess();
				}
			}
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
		}

		public override void Update()
		{
			if (connectedMenu is PauseMenu menu)
			{
				if (menu.wantToContinue)
				{
					ShutDownProcess();
				}
			} else if (connectedMenu == null)
			{
				ShutDownProcess();
			}
			lifespan++;
			float alpha = Custom.LerpExpEaseOut(0f, 1f, Mathf.Clamp(lifespan, 0, 60) / 60f);
			roundedRect.fillAlpha = alpha;
			blackSprite.alpha = connectedMenu is AchievementLoop ? 0f : alpha / 2f;
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
			achoffset.y = scrollval * (displays.Count * 105 - manager.rainWorld.options.ScreenSize.y + 5);
			/*
			if (mouseScrollWheelMovement > 1)
			{
				SliderSetValue(slider, ValueOfSlider(slider) + mouseScrollWheelMovement / 10f);
			}*/
			label.text = mouseScrollWheelMovement.ToString();
			base.Update();
		}
		public override void ShutDownProcess()
		{
			blackSprite?.RemoveFromContainer();
			foreach (var display in displays)
			{
				display?.ClearSprites();
			}
			delete = true;
			base.ShutDownProcess();
		}

		public bool GetChecked(CheckBox box)
		{
			if (box == colorizeCheckbox)
			{
				return colorize;
			}
			return false;
		}

		public void SetChecked(CheckBox box, bool c)
		{
			if (box == colorizeCheckbox)
			{
				colorize = c;
			}
		}

		internal class AchSlider : VerticalSlider
		{
			public AchSlider(Menu.Menu menu, MenuObject owner, string text, Vector2 pos, Vector2 size) : base(menu, owner, text, pos, size, new("AchSlider"), true)
			{
			}
		}
	}
}
