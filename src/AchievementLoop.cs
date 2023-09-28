using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Menu;
using RWCustom;
using Menu.Remix.MixedUI;

namespace AchievementTracker
{
	internal class AchievementLoop : Menu.Menu
	{
		public AchievementMenu menu;
		public SimpleButton backButton;
		public RoundedRect roundedRect;
		//public FSprite sily;
		public AchievementLoop(ProcessManager manager) : base(manager, CustomIds.loop)
		{
			pages = new() { new(this, null, "", 0) };

			roundedRect = new(this, pages[0], new Vector2(0, 0), new(500, 900), true);

			//sily = new("atlases/god");
			//sily.SetAnchor(0, 0);

			menu = new(manager, this);
			backButton = new(this, pages[0], Translate("Back"), "BACK", new Vector2(manager.rainWorld.options.SafeScreenOffset.x + 15f, Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)), new Vector2(110f, 30f));
			pages[0].subObjects.Add(backButton);
			//container.AddChild(sily);
		}

		public override void GrafUpdate(float timeStacker)
		{
			base.GrafUpdate(timeStacker);
			menu.GrafUpdate(timeStacker);
			//sily.SetPosition(0, 0);
		}

		public override void Update()
		{
			base.Update();
			menu.Update();
		}
		public override void ShutDownProcess()
		{
			base.ShutDownProcess();
			menu?.ShutDownProcess();
			//sily?.RemoveFromContainer();
		}
		public override void Singal(MenuObject sender, string message)
		{
			base.Singal(sender, message);
			if (message == "BACK")
			{
				manager.RequestMainProcessSwitch(ProcessManager.ProcessID.MainMenu);
			}
		}
	}
}
