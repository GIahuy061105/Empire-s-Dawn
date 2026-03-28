using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
	public string AccountName { get; set; } = "Huy Nguyễn";
	public int Gold { get; set; } = 10000;
	public int Diamond { get; set; } = 100;
	public int CurrentStage { get; set; } = 1;
	public List<string> OwnedCardIds { get; set; } = new List<string>();

	public void SetDefault()
	{
		AccountName = "Nguyễn Phạm Gia Huy";
		Gold = 10000;
		Diamond = 100;
		CurrentStage = 1;
		OwnedCardIds = new List<string> { "Arthur", "Merlin", "Anna", "Zed" }; // 4 Hero mặc định
	}
}
