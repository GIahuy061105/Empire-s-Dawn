using DemoRPGGame;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public class Character
	{
		//Base  Properties
		public string Name { get; set; }
		public CharacterStats BaseStats { get; set; }
		public int CurrentHP { get; set; }
		// Derived Properties
		public Element Element { get; set; }
		public CharClass Role { get; set; }

		//Skill and Status Properties
		public int BuffDuration { get; set; } = 0;
		public List<Skill> Skills { get; set; } = new List<Skill>();
		public List<StatusEffect> ActiveEffect { get; set; } = new List<StatusEffect>();
		public Dictionary<EquipmentSlot , Equipment> EquippedGear { get; set; } = new Dictionary<EquipmentSlot, Equipment>();
		public int AwakenLevel { get; set; } = 0;
		public Character(string name, CharacterStats stats)
		{
			Name = name;
			BaseStats = stats;
			CalculateTotalStats();
			CurrentHP = SheetHP;
		}
		public int SheetHP { get; protected set; }
		public int SheetATK { get; protected set; }
		public int SheetDEF { get; protected set; }
		public int SheetSPD { get; protected set; }
		public float SheetEff { get; protected set; }
		public float SheetRes { get; protected set; }
		public float SheetCritRate { get; protected set; }
		public float SheetCritDamage { get; protected set; }

		public int CurrentATK
		{
			get
			{
				float multiplier = 1.0f;
				foreach (var effect in ActiveEffect)
				{
					if (effect.BuffID == Buff.ATKUp) multiplier += effect.Magnitude;
					if (effect.DebuffID == Debuff.ATKDown) multiplier -= effect.Magnitude;
				}
				return Math.Max(1, (int)(SheetATK * multiplier));
			}
		}

		public int CurrentDEF
		{
			get
			{
				float multiplier = 1.0f;
				foreach (var effect in ActiveEffect)
				{
					if (effect.BuffID == Buff.DEFUp) multiplier += effect.Magnitude;
					if (effect.DebuffID == Debuff.DEFDown) multiplier -= effect.Magnitude;
				}
				return Math.Max(0, (int)(SheetDEF * multiplier));
			}
		}

		// Tốc độ thực tế (ảnh hưởng thanh lượt đi)
		public int CurrentSPD
		{
			get
			{
				float multiplier = 1.0f;
				foreach (var effect in ActiveEffect)
				{
					if (effect.BuffID == Buff.SPDUp) multiplier += effect.Magnitude;
					if (effect.DebuffID == Debuff.SPDDown) multiplier -= effect.Magnitude;
				}
				return Math.Max(1, (int)(SheetSPD * multiplier));
			}
		}

		public float CurrentCritRate => SheetCritRate; // Thường ít buff CritRate, nếu có thì cộng vào
		public float CurrentCritDmg => SheetCritDamage; // Thường ít buff CritDmg, nếu có thì cộng vào
		public float CurrentEff => SheetEff;
		public float CurrentRes => SheetRes;
		public Dictionary<MaterialType, int> GetAwakenRequirements()
		{
			Dictionary<MaterialType, int> reqs = new Dictionary<MaterialType, int>();

			// 1. Luôn tốn Vàng
			int costGold = (AwakenLevel + 1) * 1000;
			reqs.Add(MaterialType.Gold, costGold);

			// 2. Nguyên liệu theo cấp
			switch (AwakenLevel)
			{
				case 0: // 1 Sao
					reqs.Add(MaterialType.SlimeJelly, 3);
					break;
				case 1: // 2 Sao
					reqs.Add(MaterialType.SlimeJelly, 5);
					reqs.Add(MaterialType.GoblinDagger, 2);
					break;
				case 2: // 3 Sao
					reqs.Add(MaterialType.GoblinDagger, 5);
					reqs.Add(MaterialType.OrcBadge, 2);
					break;
				case 3: // 4 Sao
					reqs.Add(MaterialType.OrcBadge, 5);
					reqs.Add(MaterialType.DragonScale, 1);
					break;
				case 4: // 5 Sao
					reqs.Add(MaterialType.DragonScale, 2);
					reqs.Add(MaterialType.DarkSoul, 1);
					break;
				default: // Cấp cao
					reqs.Add(MaterialType.DragonScale, 3);
					reqs.Add(MaterialType.DarkSoul, 2);
					break;
			}

			return reqs;
		}

		// Hàm thực hiện tăng chỉ số sau khi đã trừ đồ xong
		public void ApplyAwakenStats(bool silent = false)
		{
			AwakenLevel++;

			// Tăng 15% chỉ số gốc
			int bonusHP = (int)(BaseStats.MaxHP * 0.15f);
			int bonusATK = (int)(BaseStats.Attack * 0.15f);
			int bonusDEF = (int)(BaseStats.Defense * 0.15f);

			BaseStats.MaxHP += bonusHP;
			BaseStats.Attack += bonusATK;
			BaseStats.Defense += bonusDEF;
			CurrentHP = SheetHP; // Hồi máu

			if (!silent)
			{
				GD.Print($"   (Đã cộng chỉ số: HP+{bonusHP}, ATK+{bonusATK}, DEF+{bonusDEF})");
			}
		}
		public void  EquipItem (Equipment item)
		{
			EquippedGear[item.Slot] = item;
			CalculateTotalStats();
		}
		public void UnequipItem (EquipmentSlot slot)
		{
			if (EquippedGear.ContainsKey(slot))
			{
				EquippedGear.Remove(slot);
				CalculateTotalStats();
			}
		}
		public void CalculateTotalStats()
		{
			int finalHP = BaseStats.MaxHP;
			int finalATK = BaseStats.Attack;
			int finalDEF = BaseStats.Defense;
			int finalSPD = BaseStats.Speed;
			float finalEff = BaseStats.Effectiveness;
			float finalRes = BaseStats.Resistance;
			float finalCritRate = BaseStats.CritRate > 0 ? BaseStats.CritRate : 0.15f;
			float finalCritDmg = BaseStats.CritDamage > 0 ? BaseStats.CritDamage : 1.50f;


			float bonusHpPct = 0;
			float bonusAtkPct = 0;
			float bonusDefPct = 0;
			float bonusSpdPct = 0;
			float bonusEff = 0;
			float bonusRes = 0;
			float bonusCritRate = 0;
			float bonusCritDamage = 0;
			Dictionary<EquipmentSet, int> setCounts = new Dictionary<EquipmentSet, int>();
			foreach (var item in EquippedGear.Values)
			{
				// Đếm Set
				if (!setCounts.ContainsKey(item.Set)) setCounts[item.Set] = 0;
				setCounts[item.Set]++;

				// Cộng chỉ số Main & Sub
				AddStatValue(item.MainStats, ref finalHP, ref finalATK, ref finalDEF, ref finalSPD,
						 ref bonusHpPct, ref bonusAtkPct, ref bonusDefPct, ref bonusSpdPct,
						 ref finalCritRate, ref finalCritDmg, ref finalEff, ref finalRes);

				foreach (var sub in item.SubStats)
				{
					AddStatValue(sub, ref finalHP, ref finalATK, ref finalDEF, ref finalSPD,
								 ref bonusHpPct, ref bonusAtkPct, ref bonusDefPct, ref bonusSpdPct,
								 ref finalCritRate, ref finalCritDmg, ref finalEff, ref finalRes);
				}
			}
			if (setCounts.ContainsKey(EquipmentSet.Attack_Set)) bonusAtkPct += (setCounts[EquipmentSet.Attack_Set] / 4) * 0.35f;
			if (setCounts.ContainsKey(EquipmentSet.Health_Set)) bonusHpPct += (setCounts[EquipmentSet.Health_Set] / 2) * 0.15f;
			if (setCounts.ContainsKey(EquipmentSet.Defense_Set)) bonusDefPct += (setCounts[EquipmentSet.Defense_Set] / 2) * 0.15f;
			if (setCounts.ContainsKey(EquipmentSet.Speed_Set)) bonusSpdPct += (setCounts[EquipmentSet.Speed_Set] / 4) * 0.25f;
			if (setCounts.ContainsKey(EquipmentSet.Effectiveness_Set)) bonusEff += (setCounts[EquipmentSet.Effectiveness_Set] / 2) * 0.15f;
			if (setCounts.ContainsKey(EquipmentSet.Resistance_Set)) bonusRes += (setCounts[EquipmentSet.Resistance_Set] / 2) * 0.15f;
			if (setCounts.ContainsKey(EquipmentSet.Critical_Set)) bonusCritRate += (setCounts[EquipmentSet.Critical_Set] / 2) * 0.15f;
			if (setCounts.ContainsKey(EquipmentSet.Effectiveness_Set)) bonusCritDamage += (setCounts[EquipmentSet.CritDamage_Set] / 4) * 0.50f;

			SheetHP = finalHP + (int)(BaseStats.MaxHP * bonusHpPct);
			SheetATK = finalATK + (int)(BaseStats.Attack * bonusAtkPct);
			SheetDEF = finalDEF + (int)(BaseStats.Defense * bonusDefPct);
			SheetSPD = finalSPD + (int)(BaseStats.Speed * bonusSpdPct);

			SheetCritRate = Math.Min(1.0f, finalCritRate); // Max 100%
			SheetCritDamage = Math.Min(3.5f ,finalCritDmg); // Max 350% 
			SheetEff = finalEff;
			SheetRes = finalRes;

			// Cập nhật máu hiện tại
			if (CurrentHP > SheetHP) CurrentHP = SheetHP;

		}
		private void AddStatValue(EquipmentStats stat,
		ref int hp, ref int atk, ref int def, ref int spd,
		ref float hpPct, ref float atkPct, ref float defPct, ref float spdPct,
		ref float cRate, ref float cDmg, ref float eff, ref float res)
		{
			switch (stat.StatType)
			{
				// Chỉ số cơ bản
				case StatType.Hp: hp += (int)stat.Value; break;
				case StatType.Atk: atk += (int)stat.Value; break;
				case StatType.Def: def += (int)stat.Value; break;
				case StatType.Speed: spd += (int)stat.Value; break;
				case StatType.HpPercent: hpPct += stat.Value; break;
				case StatType.AtkPercent: atkPct += stat.Value; break;
				case StatType.DefPercent: defPct += stat.Value; break;

				// Chỉ số nâng cao (Cộng thẳng %)
				case StatType.CritRate: cRate += stat.Value; break;
				case StatType.CritDamage: cDmg += stat.Value; break;
				case StatType.Effectiveness: eff += stat.Value; break;
				case StatType.Resistance: res += stat.Value; break;
			}
		}
		
		// Gây sát thương cho mục tiêu
		protected void DealDamage(Character target, float skillMultiplier = 1.0f)
		{
			CombatSystem.CalculateAndDealDamage
				(this, target, skillMultiplier);
		}
		public virtual void Attack(Character target)
		{
			GD.Print($"{Name} tấn công {target.Name}");
			DealDamage(target);
		}
		// Tấn công diện rộng với bộ lọc
		public void AoeAttack(string skillName, List<Character> enemyteam, Func<Character, bool> filterRule, float skillMultiplier , Action<Character> onHitAction = null)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			GD.Print($"\n⚔️ {Name} tung kỹ năng diện rộng: {skillName}!");
			Console.ResetColor();

			bool hitAnyone = false;
			foreach (var enemy in enemyteam)
			{
				if (enemy.CurrentHP <= 0) continue;
				if (filterRule(enemy))
				{
					Console.Write($"   -> Trúng {enemy.Name}: ");
					DealDamage(enemy, skillMultiplier);
					onHitAction?.Invoke(enemy);
					hitAnyone = true;
				}
			}
			if (!hitAnyone)
			{
				GD.Print("   -> (Kỹ năng không trúng ai cả!)");
			}
			GD.Print();
		}
		public virtual void AoeApplyDot(string skillName, List<Character> enemies, Dot dotType, int duration , int stackcount = 1)
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			GD.Print($"\n☠️ {Name} tung kỹ năng {skillName} (Áp dụng {stackcount} tầng hiệu ứng)!");
			Console.ResetColor();

			foreach (var enemy in enemies)
			{
				
				if (enemy.CurrentHP > 0) 
				{
					for (int i = 0; i < stackcount; i++)
					{
						var effect = new StatusEffect(dotType, duration, 0);
						enemy.ApplyStatusEffect(effect, this);
					}
				}
			}
			GD.Print();
		}

		// 2. HÀM THUẦN DEBUFF (Giảm chỉ số: Công, Thủ, Tốc...)
		public virtual void AoeApplyDebuff(string skillName, List<Character> enemies, Debuff debuffType, int duration)
		{
			Console.ForegroundColor = ConsoleColor.DarkMagenta;
			GD.Print($"\n💢 {Name} gào thét gây suy yếu: {skillName}!");
			Console.ResetColor();

			foreach (var enemy in enemies)
			{
				if (enemy.CurrentHP > 0)
				{
					var effect = new StatusEffect(debuffType, duration, 0);
					enemy.ApplyStatusEffect(effect, this);
				}
			}
			GD.Print();
		}

		// 3. HÀM THUẦN CC (Khống chế: Choáng, Ngủ...)
		public virtual void AoeApplyCC(string skillName, List<Character> enemies, CCType ccType, int duration)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			GD.Print($"\n💤 {Name} tung khống chế diện rộng: {skillName}!");
			Console.ResetColor();

			foreach (var enemy in enemies)
			{
				if (enemy.CurrentHP > 0)
				{
					var effect = new StatusEffect(ccType, duration);
					enemy.ApplyStatusEffect(effect, this);
				}
			}
			GD.Print();
		}
		// Áp dụng hiệu ứng trạng thái
		public void ApplyStatusEffect(StatusEffect newEffect, Character source)
		{
			StatusManager.ApplyEffect(this, newEffect, source);
		}
		// Xử lý bắt đầu lượt
		public bool OnturnStart()
		{
			foreach (var skill in Skills)
			{
				skill.ReduceCooldown();
			}
			return StatusManager.ProcessTurnStart(this);
		}
		// Xử lý kết thúc lượt
		public void OnturnEnd()
		{
			// Gọi Manager để trừ thời gian hiệu ứng
			StatusManager.ProcessTurnEnd(this);
		}

		// Vẽ UI trạng thái nhân vật
		public void DrawUI()
		{
			// Gọi Helper vẽ UI
			ConsoleUIHelper.DrawCharacterStatus(this);
		}
	}
}
