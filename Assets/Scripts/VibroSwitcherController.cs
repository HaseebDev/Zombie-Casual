using System;
using Adic;

public class VibroSwitcherController : SwitcherController
{
	public VibroSwitcherController([Inject("vibroSwitcherModel")] SwitchModel _switchModel) : base(_switchModel)
	{
	}
}
