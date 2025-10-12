namespace GameFish;

partial class BaseEquip
{
	[Property]
	[Feature( EQUIP ), Group( DEBUG )]
	[ShowIf( nameof( InGame ), true )]
	public bool IsDeployed => this.IsValid() && EquipState == EquipState.Deployed;

	[Property]
	[Title( "Equip State" )]
	[Feature( EQUIP ), Group( DEBUG )]
	[ShowIf( nameof( InGame ), true )]
	protected EquipState DebugEquipState => EquipState;

	[Sync]
	public EquipState EquipState
	{
		get => _equipState;
		set
		{
			if ( _equipState == value )
				return;

			_equipState = value;

			if ( this.InGame() )
				OnEquipStateChanged( _equipState );
		}
	}

	protected EquipState _equipState;

	[Property]
	[Feature( EQUIP ), Group( DEBUG )]
	public PawnEquipment Inventory
		=> Owner?.GetModule<PawnEquipment>();

	protected virtual void OnEquipStateChanged( EquipState state )
	{
		if ( this.InEditor() || !GameObject.IsValid() )
			return;

		// Log.Info( $"DEBUG: {this}.EquipState = {state}" );

		switch ( EquipState )
		{
			case EquipState.Dropped:
				OnDrop();
				break;
			case EquipState.Deployed:
				OnDeploy();
				break;
			case EquipState.Holstered:
				OnHolster();
				break;
		}
	}

	protected override void OnPreRender()
	{
		base.OnPreRender();

		switch ( EquipState )
		{
			case EquipState.Dropped:
				SetVisibility( viewModel: false, worldModel: true );
				break;
			case EquipState.Deployed:
				SetVisibility( viewModel: true, worldModel: false );
				break;
			case EquipState.Holstered:
				SetVisibility( viewModel: false, worldModel: false );
				break;
		}
	}

	public void SetVisibility( bool viewModel, bool worldModel = false )
	{
		var r = ViewRenderer?.ModelRenderer;

		if ( r.IsValid() )
			r.Enabled = viewModel;

		if ( WorldRenderer.IsValid() )
			WorldRenderer.Enabled = worldModel;
	}

	protected virtual void OnEquip( BasePawn owner )
	{

	}

	protected virtual void OnDrop()
	{
	}

	protected virtual void OnDeploy()
	{
	}

	protected virtual void OnHolster()
	{
	}
}
