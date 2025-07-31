using System;

namespace GameFish;

/// <summary>
/// Lets the specified component be accessed directly by interfaces.
/// </summary>
public interface IComponentType<T> where T : Component
{
	public T Component { get; }
	public ComponentList ComponentList => Component?.Components;
}
