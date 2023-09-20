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

		public readonly AchievementHud achievementHud;
		public RainWorld.AchievementID achievementID;

		public Vector2 size;
		public Vector2 pos;

		public FSprite image;
		public RoundedRect rect;
		public FLabel label;

		public int lifespan;

		public bool delete;

		public string text;
		
		public FContainer Container { get { return achievementHud.pages[0].Container; } }
		public Page Page { get { return achievementHud.pages[0] ; } }

		public AchievementDisplay(AchievementHud hud)
		{
			achievementHud = hud;

			text = "";
			delete = false;
			lifespan = 0;
			size = new(achievementHud.hudsize - sizedecrease, achivementheight);
			pos = hud.rect.pos;
			pos.x += sizedecrease / 2f;
			pos.y = hud.rect.size.y - achivementheight - sizedecrease;

			rect = new(achievementHud, Page, pos, size, false);
			image = new FSprite("GhostSB")
			{
				width = 90,
				height = 90
			};
			image.SetAnchor(new Vector2(0, .5f));
			label = new(Custom.GetFont(), text) { alignment = FLabelAlignment.Left };
			AddToContainer();
		}
		public AchievementDisplay(AchievementHud hud, RainWorld.AchievementID id) : this(hud)
		{
			achievementID = id;
			image.element = Futile.atlasManager.GetElementWithName(id.ToString());
			text = id.ToString();
			label.text = text;
		}

		public virtual void DrawSprites()
		{
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
