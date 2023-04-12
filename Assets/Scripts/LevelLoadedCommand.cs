using System;
using Adic;

public class LevelLoadedCommand : Command
{
	public override void Execute(params object[] parameters)
	{
		this.upgradebleObjectsController.LoadUpgradebleObjects();
	}

	[Inject]
	private UpgradebleObjectsController upgradebleObjectsController;
}
