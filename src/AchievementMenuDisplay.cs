﻿using System;
using Menu;
using RWCustom;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using Achid = RainWorld.AchievementID;

namespace AchievementTracker
{
	internal class AchievementMenuDisplay : MenuObject
	{
		public FSprite image;
		public RoundedRect rect;
		public FLabel label;
		public FLabel description;

		public readonly AchMenu achMenu;
		private readonly Achid id;

		public Vector2 spawnpos;
		public Vector2 pos;
		public int lifespan;
		public bool deleteFromList;
		public bool delete;
		public int deletecount;
		public bool colorize;
		public Color lastcolor;

		public AchievementMenuDisplay(Menu.Menu menu, MenuObject owner, Achid ID, bool hidden) : base(menu, owner)
		{
			achMenu = menu as AchMenu;
			spawnpos = achMenu.NewAchievementpos;
			pos = spawnpos;
			lifespan = 0;
			deletecount = 50;
			delete = false;
			deleteFromList = false;
			colorize = Plugin.colorize;
			lastcolor = Color.white;

			rect = new(menu, owner, spawnpos, new(achMenu.roundedRect.size.x - 10, 100), true);
			owner.subObjects.Add(rect);
			id = ID;
			image = new FSprite((hidden ? "gray" : "") + id.ToString())
			{
				width = 90,
				height = 90
			};
			string text = achMenu.Translate(CustomIds.achievementdata[ID].name);
			image.SetAnchor(new Vector2(0, .5f));
			label = new(Custom.GetFont(), text) { alignment = FLabelAlignment.Left };
			description = new(Custom.GetFont(), achMenu.Translate(CustomIds.achievementdata[id].description)) { alignment = FLabelAlignment.Left, color = Color.gray };
			rect.Container.AddChild(label);
			rect.Container.AddChild(description);
			rect.Container.AddChild(image);
			owner.subObjects.Add(rect);/*
			foreach (var sprite in achMenu.roundedRect.)
			{
				label.MoveBehindOtherNode(sprite);
				description.MoveBehindOtherNode(sprite);
				image.MoveBehindOtherNode(sprite);
			}*/
			Update();
			GrafUpdate(0f);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
			Vector2 newpos = pos + new Vector2(5, -5);
			rect.pos = newpos;
			float anim = Mathf.Clamp(lifespan, 0, 50) / 50f;
			float alpha = Custom.LerpExpEaseOut(0f, 1f, anim);
			float alphamul = 1f;
			if (pos.y < 0)
			{
				alphamul = 1f + (pos.y / rect.size.y);
			}
			else if (pos.y + rect.size.y > achMenu.manager.rainWorld.options.ScreenSize.y)
			{
				alphamul = -((pos.y - achMenu.manager.rainWorld.options.ScreenSize.y) / rect.size.y);
			}
			alphamul = Custom.LerpCircEaseIn(0f, 1f, alphamul);
			alpha *= alphamul;
			alpha *= achMenu.shuttingcounter / 10f;
			if (deleteFromList)
			{
				deletecount--;
				if (deletecount <= 0)
				{
					delete = true;
				}
			}
			alpha *= deletecount / 50f;
			rect.fillAlpha = alpha;
			Color bordercolor = Color.white;
			if (colorize)
			{
				if (Plugin.achievements.Contains(id)) bordercolor = Color.green; else bordercolor = Color.red;
				bordercolor = Color.Lerp(bordercolor, lastcolor, .5f);
			}
			Vector3 color = Custom.RGB2HSL(bordercolor);
			rect.borderColor = new HSLColor(color.x, color.y, color.z);
			lastcolor = bordercolor;
			foreach (var item in rect.sprites)
			{
				item.alpha = alpha;
			}
			image.alpha = alpha;
			label.alpha = alpha;
			description.alpha = alpha;
			image.SetPosition(newpos + new Vector2(5, rect.size.y / 2f));
			label.SetPosition(image.GetPosition() + new Vector2(image.width, 0));
			description.SetPosition(label.GetPosition() - new Vector2(0, 15));
		}

		public override void Update()
		{
			base.Update();
			lifespan++;
			float anim = Mathf.Clamp(lifespan, 0, 50) / 50f;
			pos = new Vector2(spawnpos.x, Custom.LerpCircEaseOut(spawnpos.y - 100, spawnpos.y, anim)) + achMenu.achoffset;
		}

		public override void RemoveSprites()
		{
			base.RemoveSprites();
			image?.RemoveFromContainer();
			label?.RemoveFromContainer();
			description?.RemoveFromContainer();
		}
	}
}
