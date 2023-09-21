using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;

namespace AchievementTracker
{
	internal class AchievementDisplay
	{
		public const float sizedecrease = 10;
		public const float achivementheight = 100;

		public AchievementHud achievementHud;
		public RainWorld.AchievementID achievementID;

		public Vector2 size;
		public Vector2 pos;
		public Vector2 spawnpos;

		public FSprite image;
		public RoundedRect rect;
		public FLabel label;

		public int lifespan;

		public bool delete;

		public string text;
		
		public FContainer Container { get { return achievementHud.pages[0].Container; } }
		public Page Page { get { return achievementHud.pages[0] ; } }

		public AchievementDisplay(AchievementHud hud, RainWorld.AchievementID id)
		{
			achievementHud = hud;

			text = id.ToString();
			achievementID = id;
			delete = false;
			lifespan = 0;
			size = new(achievementHud.hudsize - sizedecrease, achivementheight);
			spawnpos = hud.NewAchievementpos;
			Debug.Log("tis data");
			Debug.Log(spawnpos);
			pos = spawnpos;
			pos.x += sizedecrease / 2f;
			pos.y -= sizedecrease / 2f;

			rect = new(achievementHud, Page, pos, size, false);
			image = new FSprite(id.ToString())
			{
				width = 90,
				height = 90
			};
			image.SetAnchor(new Vector2(0, .5f));
			label = new(Custom.GetFont(), text) { alignment = FLabelAlignment.Left };
			AddToContainer();
		}

		public virtual void DrawSprites()
		{
			Page.subObjects.Add(rect);
			image.SetPosition(pos + new Vector2(5, rect.size.y / 2f));
			label.SetPosition(image.GetPosition() + new Vector2(image.width, 0));
		}

		public virtual void Update()
		{
			lifespan++;
		}

		public virtual void ClearSprites()
		{
			image?.RemoveFromContainer();
			label?.RemoveFromContainer();
		}
		public virtual void AddToContainer()
		{
			Container.AddChild(image);
			Container.AddChild(label);
		}

		public void Destroy()
		{
			delete = true;
		}
	}
}
