using Godot;
using DemoRPGGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DemoRPGGame
{
	public static class StatusManager
	{

		public static void ApplyEffect(Character target, StatusEffect newEffect, Character source)
		{
			if (newEffect.DotID == Dot.Burn || newEffect.DotID == Dot.Bleed)
			{
				newEffect.Magnitude = (int)(source.BaseStats.Attack * 0.6f);
			}
			if (newEffect.DotID == Dot.Poison)
			{
				newEffect.Magnitude = 0.03f; 
			}
			// 2. Cấu hình Buff (Hỗ trợ) - Cố định chỉ số
			switch (newEffect.BuffID)
			{
				case Buff.ATKUp: newEffect.Magnitude = 0.5f; break; // +50%
				case Buff.DEFUp: newEffect.Magnitude = 0.5f; break; // +50%
				case Buff.SPDUp: newEffect.Magnitude = 0.3f; break; // +30%
			}

			// 3. Cấu hình Debuff (Hại) - Cố định chỉ số
			switch (newEffect.DebuffID)
			{
				case Debuff.ATKDown: newEffect.Magnitude = 0.5f; break; // -50%
				case Debuff.DEFDown: newEffect.Magnitude = 0.5f; break; // -50%
				case Debuff.SPDDown: newEffect.Magnitude = 0.3f; break; // -30%
			}
			// 1. Nếu là Buff -> Luôn trúng
			if (newEffect.Type == EffectType.Buff)
			{
				AddOrRefreshEffect(target, newEffect);
				return;
			}
			// 2. Nếu là Debuff -> Tính Eff/Res
			float baseChance = 1.0f; // Cơ bản 100%
			float hitChance = baseChance + source.CurrentEff - target.CurrentRes;

			if (hitChance > 0.85f) hitChance = 0.85f; // Max 85% 
			if (Random.Shared.NextDouble() > hitChance)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				GD.Print($"   -> 🛡️ {target.Name} đã KHÁNG lại hiệu ứng {newEffect.Name}!");
				Console.ResetColor();
				return;
			}

			// 3. Trúng -> Thêm vào
			AddOrRefreshEffect(target, newEffect);
		}
		private static void AddOrRefreshEffect(Character target ,StatusEffect newEffect)
		{
			// Logic DoT cộng dồn
			if (newEffect.DotID != Dot.None)
			{
				target.ActiveEffect.Add(newEffect);
				Console.ForegroundColor = ConsoleColor.Magenta;
				GD.Print($"   -> {target.Name} dính thêm {newEffect.Name} ({newEffect.Duration} lượt).");
				Console.ResetColor();
				return;
			}

			// Logic Ghi đè cho Buff/Debuff/CC
			var existing = target.ActiveEffect.FirstOrDefault(e =>
				e.Type == newEffect.Type &&
				(newEffect.BuffID != Buff.None && e.BuffID == newEffect.BuffID ||
				 newEffect.DebuffID != Debuff.None && e.DebuffID == newEffect.DebuffID ||
				 newEffect.CcID != CCType.None && e.CcID == newEffect.CcID));

			if (existing != null)
			{
				existing.Duration = newEffect.Duration;
				if (newEffect.Magnitude > existing.Magnitude) existing.Magnitude = newEffect.Magnitude;

				Console.ForegroundColor = ConsoleColor.Cyan;
				GD.Print($"   -> {target.Name}: Hiệu ứng {newEffect.Name} được làm mới.");
				Console.ResetColor();
			}
			else
			{
				target.ActiveEffect.Add(newEffect);
				Console.ForegroundColor = newEffect.Type == EffectType.Buff ? ConsoleColor.Green : ConsoleColor.Magenta;
				GD.Print($"   -> {target.Name} nhận hiệu ứng: {newEffect.Name} ({newEffect.Duration} lượt).");
				Console.ResetColor();
			}
		}
		public static bool ProcessTurnStart(Character c)
		{
			bool canMove = true;
			foreach (var effect in c.ActiveEffect)
			{
				if (effect.Type == EffectType.Debuff && effect.DotID != Dot.None)
				{
					int dotDamage = 0;
					switch (effect.DotID)
					{
						case Dot.Burn:
							dotDamage = (int)effect.Magnitude;
							Console.ForegroundColor = ConsoleColor.Red;
							GD.Print($"   -> 🔥 {c.Name} bị thiêu đốt (-{dotDamage} HP).");
							break;
						case Dot.Bleed:
							dotDamage = (int)effect.Magnitude;
							Console.ForegroundColor = ConsoleColor.Red;
							GD.Print($"   -> 🔥 {c.Name} bị chảy máu (-{dotDamage} HP).");
							break;
						case Dot.Poison:
							dotDamage = (int)(c.BaseStats.MaxHP * effect.Magnitude);
							Console.ForegroundColor = ConsoleColor.Magenta;
							GD.Print($"   -> ☠️ {c.Name} trúng độc (-{dotDamage} HP).");
							break;
					}
					if (dotDamage > 0)
					{
						Console.ResetColor();
						CombatSystem.ApplyDamage(c, dotDamage);
					}
				}
				if (effect.Type == EffectType.Debuff && effect.CcID != CCType.None)
				{
					if (effect.CcID == CCType.Stun)
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						GD.Print($"   -> 💫 {c.Name} đang bị CHOÁNG!");
						Console.ResetColor();
						canMove = false;
					}
					else if (effect.CcID == CCType.Sleep)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						GD.Print($"   -> 💤 {c.Name} đang NGỦ SÂU!");
						Console.ResetColor();
						canMove = false;
					}
				}
			}
			return canMove;
		}
		public static void ProcessTurnEnd(Character c)
		{
			for (int i = c.ActiveEffect.Count - 1; i >= 0; i--)
			{
				StatusEffect effect = c.ActiveEffect[i];
				effect.Duration--;
				if (effect.Duration <= 0)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					GD.Print($"   -> {c.Name}: Hiệu ứng {effect.Name} đã kết thúc.");
					Console.ResetColor();
					c.ActiveEffect.RemoveAt(i);
				}
			}
		}
	}
}
