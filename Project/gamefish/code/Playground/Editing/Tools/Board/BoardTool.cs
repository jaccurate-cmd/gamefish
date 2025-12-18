using System.ComponentModel;
using System.Drawing;

namespace Playground;

public partial class BoardTool : EditorTool
{
	[Property]
	[Title( "Board" )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER )]
	public PrefabFile BoardPrefab { get; set; }

	[Property]
	[Range( 1f, 1024f, clamped: false )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER )]
	public float BoxSize { get; set; } = 50f;


	[Property]
	[Title( "Board Width" )]
	[Range( 1f, 32f, clamped: false )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public float BoardWidth { get; set; } = 20f;

	[Property]
	[Title( "Board Height" )]
	[Range( 1f, 16f, clamped: false )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public float BoardHeight { get; set; } = 5f;

	[Property]
	[Range( 0f, 4096f )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public float DistanceLimit { get; set; } = 1024f;


	/// <summary>
	/// If defined: the starting point for the shape.
	/// </summary>
	public Vector3? StartPoint { get; set; }


	public override void OnExit()
	{
		base.OnExit();

		StopShaping();
	}

	public override void FrameSimulate( in float deltaTime )
	{
		if ( !Mouse.Active )
			return;

		UpdatePlace( in deltaTime );
		UpdateCancel( in deltaTime );
	}

	protected virtual void StopShaping()
	{
		StartPoint = null;
	}

	protected virtual void UpdateCancel( in float deltaTime )
	{
		if ( PressedSecondary )
			StopShaping();
	}

	protected virtual void UpdatePlace( in float deltaTime )
	{
		if ( !IsClientAllowed( Client.Local ) )
			return;

		if ( !TryTrace( out var tr ) )
			return;

		var pointDist = tr.Distance.Min( DistanceLimit );
		var point = tr.StartPosition + (tr.Direction * pointDist);

		if ( tr.Hit )
			point += tr.Normal * ((BoardHeight / 2f) + 0.1f);


		var c1 = Color.White.WithAlpha( 0.4f );
		var c2 = Color.White.WithAlpha( 0.1f );

		this.DrawSphere( 2f, point, Color.Transparent, c1, global::Transform.Zero );

		if ( StartPoint is Vector3 start )
		{
			var dist = start.Distance( point );
			var dir = start.Direction( point );
			var center = start.LerpTo( point, 0.5f );

			var length = dist;
			var scale = new Vector3( length, BoardWidth, BoardHeight );
			var rDir = Rotation.LookAt( dir, tr.Normal );

			var bounds = BBox.FromPositionAndSize( Vector3.Zero, Vector3.One );
			var tBox = new Transform( center, rDir, scale );

			var isError = dist < BoardWidth;

			if ( isError )
			{
				var red = Color.Red.Desaturate( 0.4f );
				c1 = red.WithAlpha( 0.4f );
				c2 = red.WithAlpha( 0.2f );
			}

			this.DrawBox( bounds, c1, c2, tBox );

			if ( PressedPrimary && !isError )
			{
				var tBoard = tBox.WithScale( tBox.Scale / BoxSize );

				if ( TryCreateBoard( tBoard, out _ ) )
					StartPoint = null;
			}
		}
		else
		{
			if ( PressedPrimary )
				StartPoint = point;
		}
	}

	protected virtual bool TryCreateBoard( in Transform t, out GameObject objBoard )
	{
		if ( !IsClientAllowed( Client.Local ) )
		{
			objBoard = null;
			return false;
		}

		if ( !BoardPrefab.TrySpawn( t, out objBoard ) )
			return false;

		objBoard.NetworkSetup(
			cn: Connection.Local,
			orphanMode: NetworkOrphaned.ClearOwner,
			ownerTransfer: OwnerTransfer.Takeover,
			netMode: NetworkMode.Object,
			ignoreProxy: true
		);

		return true;
	}
}
