namespace Playground;

public partial class RemoverTool : EditorTool
{
	public override void FixedSimulate( in float deltaTime )
	{
		if ( Input.Pressed( "Attack1" ) )
			TryRemoveTarget();
	}

	protected virtual bool TryRemoveTarget()
	{
		if ( !IsAllowed( Connection.Local ) )
			return false;

		if ( !Editor.TryTrace( Scene, out var tr ) )
			return false;

		if ( !CanRemove( tr.GameObject ) )
			return false;

		RpcTryRemoveObject( tr.GameObject );
		return true;
	}

	public virtual bool CanRemove( GameObject obj )
	{
		if ( !obj.IsValid() )
			return false;

		// Can't remove pawns.
		if ( Pawn.TryGet( obj, out _ ) )
			return false;

		if ( obj.GetComponent<MapCollider>( includeDisabled: true ).IsValid() )
			return false;

		return true;
	}

	[Rpc.Host( NetFlags.Reliable )]
	protected void RpcTryRemoveObject( GameObject obj )
	{
		if ( !IsAllowed( Rpc.Caller ) )
			return;

		if ( !CanRemove( obj ) )
			return;

		obj.Destroy();
	}
}
