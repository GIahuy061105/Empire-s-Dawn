using Godot;
using System;

namespace DemoRPGGame
{
	public class Saint : Character
	{
		public Skill S_Blessing { get; private set; }
		public Skill S_GrandHeal { get; private set; }

		// Saint: Support, Thủ/Máu ổn, Tốc cao để buff trước
		public Saint(string name)
			: base(name, new CharacterStats(atk: 800, def: 700, hp: 11000, spd: 115))
		{
			S_Blessing = new Skill("Ban Phước (Buff)", 2);
			S_GrandHeal = new Skill("Đại Hồi Phục (Heal)", 3);

			Skills.Add(S_Blessing);
			Skills.Add(S_GrandHeal);
			Element = Element.Light;
		}

		public override void Attack(Character target)
		{
			GD.Print($"{Name} dùng gậy ánh sáng đánh {target.Name}...");
			DealDamage(target);
		}

		// Skill 2: Buff ATK cho đồng đội
		public void Blessing(Character target)
		{
			if (!S_Blessing.IsReady())
			{
				GD.Print($"🚫 Skill chưa hồi ({S_Blessing.CurrentCD})");
				return;
			}

			GD.Print($"✨ {Name} ban phước cho {target.Name}! (Tăng Công)");

			// Buff ATK Up 50% trong 2 lượt
			var buff = new StatusEffect(Buff.ATKUp, 2, 0);
			target.ApplyStatusEffect(buff, this);

			S_Blessing.TriggerCooldown();
		}

		// Skill 3: Hồi máu
		public void Heal(Character target)
		{
			if (!S_GrandHeal.IsReady())
			{
				GD.Print($"🚫 Skill chưa hồi ({S_GrandHeal.CurrentCD})");
				return;
			}

			// Hồi 30% Max HP của mục tiêu
			int healAmount = (int)(target.BaseStats.MaxHP * 0.3f);
			GD.Print($"🌿 {Name} hồi phục cho {target.Name}.");
			CombatSystem.ApplyHeal(target, healAmount);

			// Xóa 1 Debuff ngẫu nhiên (Cleanse) - Bonus thêm cho xịn
			if (target.ActiveEffect.Count > 0)
			{
				var debuff = target.ActiveEffect.Find(e => e.Type == EffectType.Debuff);
				if (debuff != null)
				{
					target.ActiveEffect.Remove(debuff);
					GD.Print($"   -> Đã giải trừ hiệu ứng xấu: {debuff.Name}");
				}
			}

			S_GrandHeal.TriggerCooldown();
		}
	}
}
