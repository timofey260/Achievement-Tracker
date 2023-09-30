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
	internal class AchievementDisplay : MenuObject
	{
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

		public AchievementDisplay(Menu.Menu menu, MenuObject owner, Achid id) : base(menu, owner)
		{
			achievementHud = menu as Hud;

			text = CustomIds.achievementdata[id].name;
			achievementID = id;
			delete = false;
			lifespan = 0;
			size = new(achievementHud.hudsize - sizedecrease, achivementheight);
			pos = achievementHud.NewAchievementpos;
			pos.x += sizedecreasehalf;
			pos.y -= sizedecreasehalf;
			spawnpos = achievementHud.NewAchievementpos;

			rect = new(achievementHud, owner, pos, size, false);
			image = new FSprite(id.ToString())
			{
				width = 90,
				height = 90
			};
			image.SetAnchor(new Vector2(0, .5f));
			label = new(Custom.GetFont(), text) { alignment = FLabelAlignment.Left };
			description = new(Custom.GetFont(), CustomIds.achievementdata[id].description) { alignment = FLabelAlignment.Left, color = Color.gray };
			Container.AddChild(image);
			Container.AddChild(label);
			Container.AddChild(description);
			owner.subObjects.Add(rect);
			Update();
			GrafUpdate(0f);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
			Vector2 newpos = pos + new Vector2(sizedecreasehalf, -sizedecreasehalf);
			rect.pos = newpos;
			image.SetPosition(newpos + new Vector2(5, rect.size.y / 2f));
			label.SetPosition(image.GetPosition() + new Vector2(image.width, 0));
			description.SetPosition(label.GetPosition() - new Vector2(0, 15));
		}

		public override void Update()
		{
			lifespan++;
			float anim = Mathf.Clamp(lifespan, 0f, 100f) / 100f;

			float alpha = Custom.LerpExpEaseOut(0f, 1f, anim);
			rect.fillAlpha = alpha;
			image.alpha = alpha;
			label.alpha = alpha;
			description.alpha = alpha;
			pos.x = Custom.LerpBackEaseOut(spawnpos.x + 500, spawnpos.x, anim);
		}

		public virtual void ClearSprites()
		{
			image?.RemoveFromContainer();
			label?.RemoveFromContainer();
			description?.RemoveFromContainer();
		}

		public void Destroy()
		{
			delete = true;
		}
	}
}
