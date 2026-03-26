using Godot;
using System.Collections.Generic;
using DemoRPGGame;

public partial class BattleGrid : Node2D
{
	[Export] public int GridWidth = 10;
	[Export] public int GridHeight = 10;
	[Export] public int TileSize = 64;

	// Lưu trữ vị trí các Unit trên bàn cờ
	public Dictionary<Vector2I, UnitView> UnitLocations = new Dictionary<Vector2I, UnitView>();

	public override void _Draw()
	{
		// Vẽ lưới 10x10 để bạn dễ quan sát
		for (int x = 0; x <= GridWidth; x++)
			DrawLine(new Vector2(x * TileSize, 0), new Vector2(x * TileSize, GridHeight * TileSize), Colors.DimGray);
		for (int y = 0; y <= GridHeight; y++)
			DrawLine(new Vector2(0, y * TileSize), new Vector2(GridWidth * TileSize, y * TileSize), Colors.DimGray);
	}

	public Vector2 GridToWorld(Vector2I gridPos)
	{
		return new Vector2(gridPos.X * TileSize + TileSize / 2, gridPos.Y * TileSize + TileSize / 2);
	}
}
