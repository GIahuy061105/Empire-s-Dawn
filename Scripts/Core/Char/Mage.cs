using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public class Mage : Character
	{
		public Skill S_ManaCharge { get; private set; }
		public Skill S_MeteorRain { get; private set; }

		// Mage: Công rất cao, Máu/Thủ thấp. Eff cao (40%) để đốt cháy
		public Mage(string name)
			: base(name, new CharacterStats(atk: 1500, def: 400, hp: 8000, spd: 110, critRate: 0.2f, eff: 0.4f))
		{
			S_ManaCharge = new Skill("Tụ Pháp (Buff)", 3);
			S_MeteorRain = new Skill("Thiên Thạch (AOE)", 4);

			Skills.Add(S_ManaCharge);
			Skills.Add(S_MeteorRain);
			Element = Element.Fire;
		}
		public override void Attack(Character target)
		{
			GD.Print($"{Name} niệm chú và bắn FireBall vào {target.Name}");
			DealDamage(target);
		}
		public void ManaCharge()
		{
			if (!S_ManaCharge.IsReady()) return;

			GD.Print($"{Name} tụ năng lượng! (Tăng Tốc Độ)");

			// Buff Speed 30% trong 2 lượt
			var buff = new StatusEffect(Buff.SPDUp, 3, 0);
			ApplyStatusEffect(buff, this);

			S_ManaCharge.TriggerCooldown();
		}
		public void CastMeteor(List<Character> enemyteam)
		{
			if (!S_MeteorRain.IsReady())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_MeteorRain.CurrentCD} lượt ");
				Console.ResetColor();
				return;
			}
			var aoeBurn = new StatusEffect(Dot.Burn, 3, 0); 
			foreach (var enemy in enemyteam.Where(t => t is Monster))
			{
			   if (Random.Shared.NextDouble() < 0.6)
			   {
					enemy.ApplyStatusEffect(aoeBurn, this);
			   }
			}
			AoeAttack("CastMeteor", enemyteam, t => t is Monster && t.CurrentHP > 0, 1.3f);
			S_MeteorRain.TriggerCooldown();
		}

	}
}
