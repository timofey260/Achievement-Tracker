using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUD;
using Menu.Remix.MixedUI;

namespace AchievementTracker
{
	internal class AchievementHud : HudPart
	{
		public DyeableRect rect;
		public AchievementHud(HUD.HUD hud) : base(hud)
		{
			rect = new(hud.fContainers[1], new(10, 10), new(200, 200));
		}

		public override void Draw(float timeStacker)
		{
			base.Draw(timeStacker);
			rect.GrafUpdate(timeStacker);
		}

		public override void Update()
		{
			base.Update();
			rect.Update();
		}
		public override void ClearSprites()
		{
			base.ClearSprites();
			if (rect != null)
			{
				for (int i = 0; i < rect.sprites.Length; i++)
				{
					rect.sprites[i]?.RemoveFromContainer();
				}
			}
			rect ??= null;
		}
	}
}
