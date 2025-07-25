namespace GameFish;

partial class PawnView
{
	/// <summary>
	/// The <see cref="global::GameFish.ViewModel"/> component. <br />
	/// Needs to be on a child of this object!
	/// </summary>
	[Property]
	[Feature( VIEW )]
	public ViewModel ViewModel
	{
		// Auto-cache the component.
		get => _vm.IsValid() ? _vm
			: _vm = Components?.Get<ViewModel>( FindMode.EverythingInDescendants );

		set { _vm = value; }
	}

	protected ViewModel _vm;
}
