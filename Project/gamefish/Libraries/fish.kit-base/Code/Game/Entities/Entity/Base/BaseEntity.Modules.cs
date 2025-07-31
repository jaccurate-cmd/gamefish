namespace GameFish;

/// <summary>
/// An entity that supports modules.
/// </summary>
public partial class ModuleEntity : BaseEntity
{
	[Property, ReadOnly]
	[Feature( ENTITY ), Group( MODULES )]
	public List<Module> ModuleList { get; set; }

	protected static bool IsModule<TMod>( Module m ) where TMod : Module
		=> m.IsValid() && m is TMod;

	/// <summary>
	/// Quickly checks the cache to see if we have this module.
	/// </summary>
	/// <typeparam name="TMod"> The specific module. </typeparam>
	/// <returns> If the module exists. </returns>
	public bool HasModule<TMod>() where TMod : Module
	{
		var mType = typeof( TMod );
		return GetModules().Any( IsModule<TMod> );
	}

	/// <summary>
	/// Always gives you a cached list of modules. Registers them if that hasn't been done yet.
	/// </summary>
	/// <returns> The cached list of modules. </returns>
	public IEnumerable<Module> GetModules()
	{
		if ( ModuleList is null )
			RegisterModules();

		return ModuleList ??= [];
	}

	/// <typeparam name="TMod"> The specific module. </typeparam>
	public TMod GetModule<TMod>() where TMod : Module
	{
		var mType = typeof( TMod );
		return GetModules().FirstOrDefault( IsModule<TMod> ) as TMod;
	}

	/// <typeparam name="TMod"> The specific module. </typeparam>
	/// <returns> Every module of this type(if any, never null). </returns>
	public IEnumerable<TMod> GetModules<TMod>() where TMod : Module
	{
		return GetModules()
			.Select( m => m as TMod )
			.Where( m => m.IsValid() );
	}

	/// <typeparam name="TMod"> The specific module. </typeparam>
	public bool TryGetModule<TMod>( out TMod m ) where TMod : Module
	{
		m = GetModule<TMod>();
		return m.IsValid();
	}

	public void RegisterModules()
	{
		ModuleList ??= [];

		foreach ( var m in Components.GetAll<Module>( FindMode.EverythingInSelfAndDescendants ) )
			RegisterModule( m );
	}

	public bool RegisterModule( Module m )
	{
		if ( !m.IsValid() )
			return false;

		if ( ModuleList is null )
		{
			ModuleList = [m];
		}
		else
		{
			if ( ModuleList.Contains( m ) )
				return false;

			ModuleList.Add( m );
		}

		m.OnRegistered( this );

		OnRegisterModule( m );

		return true;
	}

	public void RemoveModule( Module m )
	{
		// Not using IsValid because it might be destroyed.
		if ( m is null || ModuleList is null )
			return;

		ModuleList.RemoveAll( mod => !mod.IsValid() || mod == m );

		if ( m.IsValid() )
			m.OnRemoved( this );

		OnRemoveModule( m );

		return;
	}

	public void OnRegisterModule( Module m )
	{
	}

	public void OnRemoveModule( Module m )
	{
	}
}
