using Godot;
using System;
using DemoRPGGame;

namespace DemoRPGGame
{
	public static class CombatSystem
	{
		public static void CalculateAndDealDamage(Character attacker, Character defender, float skillMultiplier = 1.0f)
		{
			int totalAttack = attacker.CurrentATK;
			bool isCrit = System.Random.Shared.NextDouble() < attacker.CurrentCritRate;
			
			float critMult = 1.0f;
			float ElementMult = ElementAdvantage(attacker.Element, defender.Element);

			if (isCrit)
			{
				critMult = attacker.CurrentCritDmg;
				GD.Print("⚡ CRITICAL HIT!");
			}

			// Tính Damage 
			int rawDamage = (int)(totalAttack * critMult * skillMultiplier * ElementMult);
			int finalDamage = rawDamage - defender.CurrentDEF / 2;
			if (finalDamage <= 0) finalDamage = 1;

			// Trừ máu
			ApplyDamage(defender, finalDamage);
		}

		public static void ApplyDamage(Character target, int amount)
		{
			target.CurrentHP -= amount;
			GD.Print($"   -> {target.Name} mất {amount} máu. (HP: {target.CurrentHP}/{target.SheetHP})");

			// 1. Kiểm tra chết
			if (target.CurrentHP <= 0)
			{
				target.CurrentHP = 0;
				GD.Print($"💀 {target.Name} đã gục ngã!");
				return; 
			}

			// 2. Kiểm tra nội tại Boss (Hàm CheckEnrage từ Monster.cs)
			if (target is Monster boss)
			{
				boss.CheckEnrage();
			}
		}

		public static void ApplyHeal(Character target, int amount)
		{
			target.CurrentHP += amount;
			if (target.CurrentHP > target.SheetHP)
				target.CurrentHP = target.SheetHP;

			GD.Print($"   -> {target.Name} hồi phục {amount} HP. (HP: {target.CurrentHP}/{target.SheetHP})");
		}

		public static float ElementAdvantage(Element attackerElement, Element defenderElement)
		{
			float multiplier = 1.0f;
			switch (attackerElement)
			{
				case Element.Fire:
					if (defenderElement == Element.Earth) multiplier = 1.2f;
					else if (defenderElement == Element.Water) multiplier = 0.8f;
					break;
				case Element.Water:
					if (defenderElement == Element.Fire) multiplier = 1.2f;
					else if (defenderElement == Element.Earth) multiplier = 0.8f;
					break;
				case Element.Earth:
					if (defenderElement == Element.Water) multiplier = 1.2f;
					else if (defenderElement == Element.Fire) multiplier = 0.8f;
					break;
				case Element.Light:
					if (defenderElement == Element.Dark) multiplier = 1.2f;
					break;
				case Element.Dark:
					if (defenderElement == Element.Light) multiplier = 1.2f;
					break;
				default:
					multiplier = 1.0f;
					break;
			}
			return multiplier;
		}
	}
}
