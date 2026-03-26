using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public class Slime : Monster
	{
		public Slime() : base(
			"Slime Nhớt",
			MonsterRank.Normal,
			new CharacterStats(atk: 800, def: 100, hp: 3000, spd: 80)
		)
		{
			GoldDrop  = 500;
			DropTable.Add( MaterialType.SlimeJelly , 3);
			Element = Element.Water;
		}
	}




	// --- 2. GOBLIN (Quái thường - Tốc cao) ---
	public class Goblin : Monster
	{
		public Skill S_Ambush { get; private set; }
		public Goblin() : base(
			"Goblin Trộm",
			MonsterRank.Normal,
			new CharacterStats(atk: 1200, def: 200, hp: 5000, spd: 180, critRate: 0.3f)
		)
		{
			// Có thể thêm skill thụ động hoặc đánh lén ở đây nếu muốn
			S_Ambush = new Skill("Đánh Lén", 2);
			Skills.Add(S_Ambush);
			 
			Element = Element.Earth;
			GoldDrop  = 800;
			DropTable.Add( MaterialType.GoblinDagger , 2);
		}
		public void CastAmbush(List<Character> Allfighter)
		{
			if (!S_Ambush.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_Ambush.CurrentCD} lượt ");
				return;
			}
			var heroes = Allfighter.FindAll(c => c.CurrentHP > 0 && !(c is Monster));
			if (heroes.Count == 0) return;
			Character target = heroes[Random.Shared.Next(heroes.Count)];
			GD.Print($"{Name} tấn công lén {target.Name}!");
			DealDamage(target, 1.4f);
			var defDown = new StatusEffect(Debuff.DEFDown, 2, 0.5f);
			target.ApplyStatusEffect(defDown, this);
			S_Ambush.TriggerCooldown();
		}
	}

	// --- 3. ORC (Tinh anh - Có Skill) ---
	public class Orc : Monster
	{
		public Skill S_SBash { get; private set; }
		public Orc() : base(
			"Orc Chiến Binh",
			MonsterRank.Elite,
			new CharacterStats(atk: 1500, def: 500, hp: 12000, spd: 100)
		)
		{
			S_SBash = new Skill("Đấm Mạnh", 3);
			Skills.Add(S_SBash);
			Element = Element.Earth;
			GoldDrop  = 1500;
			DropTable.Add( MaterialType.OrcBadge , 2);
		}
		public void SmashBash(Character target)
		{
			if (!S_SBash.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_SBash.CurrentCD} lượt ");
				return;
			}
			GD.Print($"{Name} dùng Đấm Mạnh vào {target.Name}!");
			DealDamage(target, 1.5f);
			var stun = new StatusEffect(CCType.Stun, 1);
			target.ApplyStatusEffect(stun, this);
			S_SBash.TriggerCooldown();
		}
		public override void ExecuteTurn(List<Character> allFighters)
		{
			// Tìm mục tiêu
			var heroes = allFighters.FindAll(c => c.CurrentHP > 0 && !(c is Monster));
			if (heroes.Count == 0) return;
			Character target = heroes[Random.Shared.Next(heroes.Count)];
			if (S_SBash.IsReady())
			{
				SmashBash(target);
			}
			else
			{
				Attack(target);
			}
		}
	}

	// --- 4. RỒNG (BOSS - Nhiều Skill + Nội tại) ---
	public class Dragon : Monster
	{
		// Khai báo Skill riêng (giống Saint)
		public Skill S_FireBreath { get; private set; }
		public Skill S_TerrorRoar { get; private set; }

		public Dragon() : base(
			"Hắc Long",
			MonsterRank.Boss,
			new CharacterStats(atk: 3500, def: 1200, hp: 40000, spd: 160, res: 0.4f)
		)
		{
			// Khởi tạo skill
			S_FireBreath = new Skill("Phun Lửa", 3);
			S_TerrorRoar = new Skill("Tiếng Gầm", 5);

			// Add vào list chung để hệ thống tự trừ Cooldown ở OnturnStart
			Skills.Add(S_FireBreath);
			Skills.Add(S_TerrorRoar);
			Element = Element.Fire;
			GoldDrop  = 5000;
			DropTable.Add( MaterialType.DragonScale , 1);
		}

		// --- CÁC HÀM SKILL CỤ THỂ (Code logic tại đây) ---
		private void CastFireBreath(List<Character> allFighters)
		{
			if (!S_FireBreath.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_FireBreath.CurrentCD} lượt ");
				return;
			}
			AoeApplyDot("Fire Breath" , allFighters, Dot.Burn, 3 , 2);
			AoeAttack("Fire Breath", allFighters, t => !(t is Monster) && t.CurrentHP > 0, 1.8f);
			S_FireBreath.TriggerCooldown();
		}

		private void CastTerrorRoar(List<Character> allFighters)
		{
			if (!S_TerrorRoar.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_TerrorRoar.CurrentCD} lượt ");
				return;
			}

			// Logic gầm: AOE + Giảm công
			AoeApplyDebuff("Terror Roar", allFighters, Debuff.ATKDown, 2);
			AoeApplyDebuff("Terror Roar", allFighters, Debuff.DEFDown, 2);
			AoeAttack("Terror Roar", allFighters, t => !(t is Monster) && t.CurrentHP > 0, 1.4f);

			
			S_TerrorRoar.TriggerCooldown();
		}
		public override void PerformEnrage()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"\n💢 {Name} ĐÃ HÓA ĐIÊN!");
			Console.ResetColor();
			// Buff sức mạnh
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.ATKUp, 99, 0), this);
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.DEFUp, 99, 0), this);
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.SPDUp, 99, 0), this);


			int heal = (int)(SheetHP * 0.3f);
			CombatSystem.ApplyHeal(this, heal);
		}

		public override void ExecuteTurn(List<Character> allFighters)
		{
			GD.Print($"\n👿 {Name} đang suy tính...");

			// AI: Ưu tiên Gầm -> Phun Lửa -> Đánh thường
			if (S_TerrorRoar.IsReady())
			{
				CastTerrorRoar(allFighters);
			}
			else if (S_FireBreath.IsReady())
			{
				CastFireBreath(allFighters);
			}
			else
			{
				// Gọi hàm đánh thường của AI mặc định (Random mục tiêu)
				base.ExecuteTurn(allFighters);
			}
		}
	}

	// --- 5. QUỶ VƯƠNG (BOSS Mới) ---
	public class DemonKing : Monster
	{
		public Skill S_SoulSteal { get; private set; }
		public Skill S_HellStorm { get; private set; }
		public DemonKing() : base(
			"Quỷ Vương Diablo",
			MonsterRank.Boss,
			new CharacterStats(atk: 4000, def: 1400, hp: 45000, spd: 180, res: 0.5f)
		)
		{
			S_SoulSteal = new Skill("Đoạt Hồn", 3);
			S_HellStorm = new Skill("Bão Địa Ngục", 5);
			Skills.Add(S_SoulSteal);
			Skills.Add(S_HellStorm);
			Element = Element.Dark;
			GoldDrop  = 8000;
			DropTable.Add( MaterialType.DarkSoul , 1);
		}
		public void CastSoulSteal(Character target)
		{
			if (!S_SoulSteal.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_SoulSteal.CurrentCD} lượt ");
				return;
			}
			GD.Print($"{Name} dùng Đoạt Hồn vào {target.Name}!");
			var atkDown = new StatusEffect(Debuff.ATKDown, 3, 0);
			var atkbuff = new StatusEffect(Buff.ATKUp, 3, 0);
			target.ApplyStatusEffect(atkDown, this);
			this.ApplyStatusEffect(atkbuff, this);
			DealDamage(target, 1.6f);
			S_SoulSteal.TriggerCooldown();
		}
		public void CastHellStorm(List<Character> allFighters)
		{
			if (!S_HellStorm.IsReady())
			{
				GD.Print($"🚫 Kỹ năng chưa sẵn sàng! Còn chờ {S_HellStorm.CurrentCD} lượt ");
				return;
			}
			AoeApplyDot("Hell Storm", allFighters, Dot.Burn, 3, 3);
			AoeAttack("Hell Storm", allFighters, t => !(t is Monster) && t.CurrentHP > 0, 2.0f);
			S_HellStorm.TriggerCooldown();
		}
		public override void ExecuteTurn(List<Character> allFighters)
		{
			GD.Print($"\n👿 {Name} đang suy tính...");

			var heroes = allFighters.FindAll(c => c.CurrentHP > 0 && !(c is Monster));
			if (heroes.Count == 0) return; 

			if (S_HellStorm.IsReady())
			{
				CastHellStorm(allFighters);
			}
			else if (S_SoulSteal.IsReady())
			{
				Character victim = heroes[Random.Shared.Next(heroes.Count)];
				CastSoulSteal(victim);
			}
			else
			{
				base.ExecuteTurn(allFighters);
			}
		}
		public override void PerformEnrage()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"\n💢 {Name} ĐÃ HÓA ĐIÊN!");
			Console.ResetColor();
			// Buff sức mạnh
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.ATKUp, 99, 0), this);
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.DEFUp, 99, 0), this);
			StatusManager.ApplyEffect(this, new StatusEffect(Buff.SPDUp, 99, 0), this);
			

			int heal = (int)(SheetHP * 0.3f);
			CombatSystem.ApplyHeal(this, heal);
		}
	}
}
