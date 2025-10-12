namespace GameFish;

partial class BaseEntity
{
	public static bool TryGetPrefab( string classId, out PrefabFile prefabFile )
	{
		prefabFile = null;

		if ( string.IsNullOrWhiteSpace( classId ) || PrefabLibrary.All is null )
			return false;

		foreach ( var def in PrefabLibrary.FindByComponent<BaseEntity>() )
		{
			if ( !def.Prefab.IsValid() )
				continue;

			var comps = def.PrefabScene?.Components;

			if ( comps is null || !comps.TryGet<BaseEntity>( out var ent, FindMode.EverythingInSelf ) )
				continue;

			if ( ent.IsClass && ent.ClassId == classId )
				return (prefabFile = def.Prefab).IsValid();
		}

		return false;
	}

	/// <summary>
	/// Lets you spawn entities where you're looking by their class ID(if they have one).
	/// </summary>
	[ConCmd( "spawn", ConVarFlags.Cheat )]
	public static void DebugSpawn( string classId, float distance = 1000f )
	{
		if ( !TryGetPrefab( classId, out var prefabFile ) )
		{
			Print.WarnFrom( typeof( BaseEntity ), $"Couldn't find Entity with ID:\"{classId}\"." );
			return;
		}

		GameObject go;

		// Try to spawn it under our current pawn's aim.
		if ( Client.Local?.Pawn is var pawn && pawn.IsValid() )
		{
			var endPos = pawn.GetEyeTrace( distance ).Run().EndPosition;
			prefabFile.TrySpawn( pawn.EyeTransform.WithPosition( endPos ), out go );
		}
		else
		{
			prefabFile.TrySpawn( out go );
		}

		if ( go.IsValid() && go.Components.TryGet<BaseEntity>( out var ent ) )
			ent.SetupNetworking( force: true );
	}
}
