using Godot;
using System;
using System.Collections.Generic;

namespace DemoRPGGame { 
	public class Warrior : Character
	{
		// Khai báo biến skill để dễ gọi trong code
		public Skill S_BattleCry { get; private set; }
		public Skill S_Whirlwind { get; private set; }

		public Warrior(string name)
			: base(name, new CharacterStats(atk: 1200, def: 600, hp: 12000, spd: 105))
		{
			// 1. Tạo skill
			S_BattleCry = new Skill("Chiến Hống", 3); // Hồi 4 lượt
			S_Whirlwind = new Skill("Xoay Kiếm", 4);  // Hồi 3 lượt

			// 2. Bỏ vào danh sách quản lý chung của cha (để tự giảm CD)
			Skills.Add(S_BattleCry);
			Skills.Add(S_Whirlwind);

			Element = Element.Earth; 
		}

		public override void Attack(Character target)
		{
			GD.Print($"{Name} vung kiếm chém {target.Name}!");
			DealDamage(target); // Warrior đánh thường thì gọi hàm cha
		}

		public void BattleCry()
		{
			// Kiểm tra skill có sẵn sàng không
			if (!S_BattleCry.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! (Còn {S_BattleCry.CurrentCD} lượt )");
				return;
			}

			GD.Print($"{Name} gầm lên! Tăng công trong 2 lượt.");
			var buff = new StatusEffect(Buff.ATKUp, 2, 0);
			ApplyStatusEffect(buff, this);

			// Kích hoạt hồi chiêu
			S_BattleCry.TriggerCooldown();
		}

		public void Whirlwind(List<Character> enemyteam)
		{
			if (!S_Whirlwind.IsReady())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! (Còn {S_Whirlwind.CurrentCD} lượt )");
				Console.ResetColor();
				return;
			}

			AoeAttack("Whirlwind", enemyteam, t => t is Monster && t.CurrentHP > 0, 1.2f);

			// Kích hoạt hồi chiêu
			S_Whirlwind.TriggerCooldown();
		}
	}
}
