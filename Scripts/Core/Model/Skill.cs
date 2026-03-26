using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public class Skill
	{
		public string Name { get; set; }
		public int MaxCoolDown { get; set; }
		public int CurrentCD { get; set; } = 0;
		public Skill (string Name , int MaxCD)
		{
			this.Name = Name;
			MaxCoolDown = MaxCD;
		}
		public bool IsReady()
		{
			return CurrentCD <= 0;
		}

		// Dùng xong thì kích hoạt hồi chiêu
		public void TriggerCooldown()
		{
			CurrentCD = MaxCoolDown;
		}

		// Giảm hồi chiêu mỗi lượt (gọi ở OnTurnStart)
		public void ReduceCooldown()
		{
			if (CurrentCD > 0) CurrentCD--;
		}
	}
}
