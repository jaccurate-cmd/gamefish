using System.Threading.Tasks;
using Boxfish.Library;

namespace Playground;

public partial class VoxelMap : NetworkedVoxelVolume, Component.ExecuteInEditor
{
	[Property]
	[Group( MAP )]
	[Sync( SyncFlags.FromHost )]
	[FilePath( Extension = "vox" )]
	[Change( Name = nameof( ReloadMap ) )]
	public string MapPath { get; set; }

	/*
	protected override async Task OnLoad()
	{
		await base.OnLoad();

		await LoadMap( MapPath );
	}
	*/

	protected override void OnStart()
	{
		base.OnStart();

		Task.RunInThreadAsync( ReloadMap );
	}

	protected async Task LoadMap( string path = null )
	{
		if ( path.IsBlank() )
			return;

		await Task.MainThread();

		await Import( path );
	}

	[Button]
	[Group( MAP )]
	public async Task ReloadMap()
		=> await LoadMap( MapPath );

	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( MapPath.IsBlank() )
			return;

		if ( Chunks?.Any() is not true )
			Task.RunInThreadAsync( ReloadMap );
	}
}
