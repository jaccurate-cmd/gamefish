using System;
using Sandbox.UI;

namespace GameFish;

/// <summary>
/// Manages and optionally spawns equipment.
/// </summary>
[Icon( "backpack" )]
public partial class PawnEquipment : Module
{
	public const string GROUP_SLOTTING = "Slotting";
	public const string GROUP_INVENTORY = "Inventory";

	/// <summary> If true: only try to pick up weapons in their intended slot. </summary>
	[Title( "Strict" )]
	[Property, Feature( EQUIP ), Group( GROUP_SLOTTING )]
	public virtual bool StrictSlots { get; set; }

	/// <summary> If true: prevent picking up multiple instances of a weapon. </summary>
	[Title( "Unique" )]
	[Property, Feature( EQUIP ), Group( GROUP_SLOTTING )]
	public virtual bool UniqueEquips { get; set; } = true;

	/// <summary> How many weapons can fit in each individual slot? </summary>
	[Title( "Size" )]
	[Range( 1, 4, clamped: false ), Step( 1f )]
	[Property, Feature( EQUIP ), Group( GROUP_SLOTTING )]
	public virtual int SlotCapacity { get; set; } = 1;

	/// <summary> How many equipment slots are available overall? </summary>
	[Title( "Available" )]
	[Range( 0, 10, clamped: false ), Step( 1f )]
	[Property, Feature( EQUIP ), Group( GROUP_SLOTTING )]
	public virtual int SlotCount { get; set; } = 10;

	/// <summary> How many equipment slots are available overall? </summary>
	[Title( "Input Prefix" )]
	[Range( 0, 10, clamped: false ), Step( 1f )]
	[Property, Feature( EQUIP ), Group( GROUP_SLOTTING )]
	public virtual string InputPrefix { get; set; } = "Slot";

	/// <summary>
	/// Automatically give the loadout when this module first starts? <br />
	/// If not then you'll need to call <see cref="GiveLoadout"/> yourself.
	/// </summary>
	[Property, Feature( EQUIP ), Group( GROUP_INVENTORY )]
	public virtual bool AutoGiveLoadout { get; set; } = true;

	/// <summary> The weapons to spawn. </summary>
	[Property, Feature( EQUIP ), Group( GROUP_INVENTORY )]
	public virtual List<EquipLoadoutEntry> Loadout { get; set; } = [];

	[InlineEditor, ReadOnly]
	[Sync( SyncFlags.FromHost )]
	[ShowIf( nameof( InGame ), true )]
	[Property, Feature( EQUIP ), Group( GROUP_INVENTORY )]
	public NetList<BaseEquip> Equipped { get; set; } = [];

	/// <summary>
	/// The equipment actively deployed(if any).
	/// </summary>
	[Sync]
	public BaseEquip ActiveEquip
	{
		get => _activeEquip.IsValid() && _activeEquip.Active
			? _activeEquip : null;

		set
		{
			if ( !value.IsValid() )
			{
				Holster( _activeEquip );
				_activeEquip = null;

				return;
			}

			if ( _activeEquip == value )
				return;

			Holster( _activeEquip );

			_activeEquip = value;

			Deploy( value );
		}
	}

	protected BaseEquip _activeEquip;

	public BasePawn Pawn => Parent as BasePawn;

	public override bool IsParent( ModuleEntity comp )
		=> comp.IsValid() && comp is BasePawn;

	protected override void OnStart()
	{
		base.OnStart();

		if ( !Networking.IsHost )
			return;

		if ( AutoGiveLoadout && this.InGame() )
		{
			// this.Log( $"Auto-giving loadout for pawn:{Pawn}" );
			GiveLoadout();
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( IsProxy )
			return;

		var owner = Pawn;

		if ( !owner.IsValid() || !owner.IsAlive )
			return;

		SelectEquipUpdate();
	}

	protected virtual void SelectEquipUpdate()
	{
		if ( Equipped is null || Equipped.Count == 0 )
			return;

		// Scrolling
		var yScroll = Input.MouseWheel.y.Sign();

		if ( yScroll != 0f )
		{
			// this.Log( yScroll );

			var slots = Equipped
				.Where( e => e.IsValid() )
				.Select( e => e.Slot )
				.Distinct()
				.Order()
				.ToList();

			var currentSlot = ActiveEquip.IsValid() ? ActiveEquip.Slot : 0;
			var slotIndex = slots.IndexOf( currentSlot );

			if ( slotIndex == -1 || !slots.Contains( currentSlot ) )
			{
				ActiveEquip = Equipped.FirstOrDefault() ?? ActiveEquip;
				return;
			}

			var nextSlotIndex = (slotIndex + yScroll).UnsignedMod( slots.Count );
			var nextSlot = slots.ElementAtOrDefault( nextSlotIndex );
			var selectEquip = Equipped.FirstOrDefault( e => e.IsValid() && e.Slot == nextSlot );

			if ( selectEquip.IsValid() )
			{
				ActiveEquip = selectEquip;
				return;
			}
		}

		// Slot Pressing
		foreach ( var equip in Equipped )
		{
			if ( !equip.IsValid() )
				continue;

			if ( Input.Pressed( InputPrefix + equip.Slot ) )
			{
				ActiveEquip = equip;
				return;
			}
		}
	}

	public virtual void GiveLoadout()
	{
		if ( Loadout is null )
			return;

		foreach ( var entry in Loadout )
		{
			// this.Log( entry );
			int? slot = entry.Slot?.AsInt();

			if ( slot.HasValue && slot.Value <= 0 )
				slot = null;

			TryEquip( entry.Prefab, out _, slot );
		}
	}

	public virtual BaseEquip SelectFirstEquip()
	{
		if ( Equipped is null )
			return null;

		return Equipped.OrderBy( e => e?.Slot ?? 0 )
			.FirstOrDefault( e => e.IsValid() );
	}

	protected virtual void Holster( BaseEquip e )
	{
		if ( !e.IsValid() )
			return;

		e.EquipState = EquipState.Holstered;
	}

	protected virtual void Deploy( BaseEquip e )
	{
		if ( !e.IsValid() )
			return;

		// Holster previous equipment.
		if ( ActiveEquip.IsValid() && ActiveEquip.IsDeployed )
		{
			if ( ActiveEquip == e )
				return;

			Holster( ActiveEquip );
		}

		// this.Log( $"Deployed equip:[{e}]" );

		e.EquipState = EquipState.Deployed;
	}

	public virtual bool TryRemove( BaseEquip e )
	{
		if ( !e.IsValid() )
		{
			// Check if it was destroyed but still referenced.
			if ( e is not null && Equipped is not null )
				if ( Equipped.Contains( e ) )
					RefreshList();

			return false;
		}

		e.GameObject.Destroy();

		RefreshList();

		return true;
	}

	public virtual bool TryDrop( BaseEquip e )
	{
		throw new NotImplementedException();
	}

	public virtual bool TryEquip( string classId, out BaseEquip e, int? slot = null )
	{
		if ( !TryGetPrefab( classId, out var ePrefab ) )
		{
			e = null;
			return false;
		}

		return TryEquip( ePrefab, out e, slot );
	}

	public virtual bool TryEquip( PrefabFile p, out BaseEquip e, int? slot = null )
	{
		e = null;

		if ( !Networking.IsHost )
			return false;

		if ( !p.TrySpawn( WorldPosition, out var go ) )
		{
			this.Warn( $"Tried to equip invalid equipment prefab:[{p}]" );
			return false;
		}

		e = go.Components.Get<BaseEquip>( FindMode.EverythingInSelf );

		if ( TryEquip( e, slot ) )
			return true;

		go.Destroy();
		return false;
	}

	public virtual bool TryEquip( BaseEquip e, int? slot = null )
	{
		if ( !Networking.IsHost )
			return false;

		var pawn = Pawn;

		if ( !pawn.IsValid() )
		{
			this.Warn( $"Tried to equip:[{e}] onto an invalid parent:[{pawn}]" );
			return false;
		}

		if ( !e.IsValid() )
		{
			this.Warn( $"Tried to equip an invalid equip on parent:[{pawn}]" );
			return false;
		}

		// Allow restricting one weapon at a time.
		if ( UniqueEquips && Any( e.ClassId ) )
			return false;

		// The "None" EquipSlot is zero. Nullify for simplicity.
		if ( slot.HasValue && slot.Value <= 0 )
			slot = null;

		// Allow restricting to a specific slot.
		if ( StrictSlots )
		{
			var defaultSlot = e.DefaultSlot.AsInt();
			slot ??= defaultSlot;

			if ( slot != defaultSlot )
				return false;
		}
		else
		{
			slot ??= FirstFreeSlot();

			if ( !slot.HasValue )
				return false;
		}

		var inSlot = GetAllInSlot( slot );

		if ( inSlot.Count() > SlotCapacity )
		{
			this.Log( $"Tried to equip:[{e}] in full slot:[{slot}] on parent:[{pawn}]" );
			return false;
		}

		// Equip might have something to say about this.
		if ( !e.AllowEquip( pawn ) )
			return false;

		// Assign the slot so it can be swapped between.
		e.Slot = slot.Value;

		// Network spawn it before making it a child!
		e.UpdateNetworking( null );

		// Setting this should parent it and shit.
		e.SetOwner( pawn );

		// So that clients know what's up.
		RefreshList();

		return true;
	}
}
