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

	public static class ConsoleUIHelper
	{
		public static void DrawCharacterStatus(Character c)
		{
			GD.Print($"--- {c.Name} ---");
			Console.Write($"HP: {c.CurrentHP}/{c.SheetHP} ");

			Console.Write("| ");
			foreach (var skill in c.Skills)
			{
				if (skill.IsReady())
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($"[{skill.Name}: SẴN SÀNG] ");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write($"[{skill.Name}: {skill.CurrentCD}] ");
				}
				Console.ResetColor();
			}
			GD.Print();
		}
	}
}
