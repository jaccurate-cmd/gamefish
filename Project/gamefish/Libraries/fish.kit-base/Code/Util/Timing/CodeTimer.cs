using System;

namespace GameFish;

/// <summary>
/// Tells you how long your code block took to execute.
/// </summary>
/// <remarks>
/// Made originally by ubre at Small Fish. <br />
/// Usage: <c>using ( CodeTimer( "Example" ) ) { ... }</c>
/// </remarks>
public sealed class CodeTimer : IDisposable
{
	private readonly DateTime _started;
	private readonly string _name;

	public CodeTimer( string timerName = "", bool startMessage = true )
	{
		_name = timerName;
		_started = DateTime.Now;

		if ( startMessage )
			Print.InfoFrom( this, $"Starting {_name}..." );
	}

	public void Dispose()
	{
		Print.InfoFrom( this, $"{_name} took {(DateTime.Now - _started).TotalMilliseconds}ms" );
	}
}
