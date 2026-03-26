using Godot;
using System;
using System.Collections.Generic;
namespace DemoRPGGame
{
	public class Debuffer : Character
	{
		public Skill S_CursedTouch { get; private set; } // Skill 2: Phá giáp
		public Skill S_Assassinate { get; private set; } // Skill 3: Choáng + Chảy máu

		// Chỉ số: Tốc cao (120), Eff cao (50%) để debuff trúng Boss
		public Debuffer(string name)
			: base(name, new CharacterStats(atk: 1000, def: 500, hp: 9000, spd: 120, eff: 0.5f))
		{
			S_CursedTouch = new Skill("Lời Nguyền (Def Break)", 3);
			S_Assassinate = new Skill("Ám Sát (Stun)", 5);

			Skills.Add(S_CursedTouch);
			Skills.Add(S_Assassinate);
			Element = Element.Dark;
		}

		// Skill 1: Đánh thường + 50% Gây Độc
		public override void Attack(Character target)
		{
			GD.Print($"{Name} phóng dao tẩm độc vào {target.Name}!");
			DealDamage(target);

			// 50% tỉ lệ gây Độc (2 lượt)
			if (Random.Shared.NextDouble() < 0.5)
			{
				var poison = new StatusEffect(Dot.Poison, 3, 0); //1,5%
				target.ApplyStatusEffect(poison, this);
			}
		}

		// Skill 2: Phá Giáp (Giảm thủ đối phương) - Quan trọng để đánh Boss
		public void CursedTouch(Character target)
		{
			if (!S_CursedTouch.IsReady())
			{
				GD.Print($"🚫 Skill chưa hồi ({S_CursedTouch.CurrentCD})");
				return;
			}

			GD.Print($"{Name} chạm tay vào {target.Name}, làm mềm lớp giáp!");

			// Gây 80% damage
			DealDamage(target, 0.8f);

			// 100% tỉ lệ gây DEF Down (2 lượt, Giảm 50% thủ)
			// (Vẫn phải tính qua Eff vs Res trong hàm Apply)
			var defDown = new StatusEffect(Debuff.DEFDown, 2, 0.5f);
			target.ApplyStatusEffect(defDown, this);

			S_CursedTouch.TriggerCooldown();
		}

		// Skill 3: Ám Sát (Sát thương lớn + Choáng + Chảy Máu)
		public void Assassinate(Character target)
		{
			if (!S_Assassinate.IsReady())
			{
				GD.Print($"🚫 Skill chưa hồi ({S_Assassinate.CurrentCD})");
				return;
			}

			GD.Print($"⚔️ {Name} xuất hiện sau lưng và kết liễu {target.Name}!");

			// Gây 150% damage
			DealDamage(target, 1.5f);

			// Gây Choáng (1 lượt)
			var stun = new StatusEffect(CCType.Stun, 1);
			target.ApplyStatusEffect(stun, this);

			var bleed = new StatusEffect(Dot.Bleed , 2 , 0);
			target.ApplyStatusEffect(bleed, this);

			S_Assassinate.TriggerCooldown();
		}
	}
}
