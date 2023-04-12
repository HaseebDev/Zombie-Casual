using System;
using Adic;

public class SoundSwitcherController : SwitcherController
{
	public SoundSwitcherController([Inject("soundSwitcherModel")] SwitchModel _switchModel) : base(_switchModel)
	{
	}
}
