namespace GameFish;

/// <summary>
/// The most basic form of an object that can separately exist.
/// </summary>
public abstract partial class BaseEntity : BaseClass, ITransform
{
	protected const string ENTITY = Library.ENTITY;
	protected const int ENTITY_ORDER = ID_ORDER + 100;

	protected const string NETWORKING = Library.NETWORKING;
	protected const int NETWORK_ORDER = ENTITY_ORDER + 1;

	protected const string DEBUG = Library.DEBUG;
	protected const int DEBUG_ORDER = ENTITY_ORDER - 1;

	protected const string SETTINGS = Library.SETTINGS;
	protected const string CONFIG = Library.CONFIG;

	protected const string SCENE = Library.SCENE;
	protected const string SCENES = Library.SCENES;
	protected const string LEVEL = Library.LEVEL;
	protected const string LEVELS = Library.LEVELS;
	protected const string MAP = Library.MAP;
	protected const string MAPS = Library.MAPS;

	protected const string ATTRIBUTES = Library.ATTRIBUTES;
	protected const string CALLBACKS = Library.CALLBACKS;
	protected const string MODULES = Library.MODULES;
	protected const string FILTERS = Library.FILTERS;
	protected const string LOGIC = Library.LOGIC;

	protected const string SPECTATOR = Library.SPECTATOR;
	protected const string PLAYER = Library.PLAYER;
	protected const string AGENT = Library.AGENT;
	protected const string PAWN = Library.PAWN;
	protected const string NPC = Library.NPC;

	protected const string CONSTRAINTS = Library.CONSTRAINTS;
	protected const string COLLISION = Library.COLLISION;
	protected const string MOMENTUM = Library.MOMENTUM;
	protected const string ROTATION = Library.ROTATION;
	protected const string TRIGGER = Library.TRIGGER;
	protected const string PHYSICS = Library.PHYSICS;
	protected const string FORCES = Library.FORCES;
	protected const string DRAG = Library.DRAG;

	protected const string EFFECTS = Library.EFFECTS;
	protected const string FACTION = Library.FACTION;
	protected const string TIMING = Library.TIMING;
	protected const string SOUNDS = Library.SOUNDS;
	protected const string FLYING = Library.FLYING;
	protected const string HEALTH = Library.HEALTH;
	protected const string INPUT = Library.INPUT;
	protected const string MODEL = Library.MODEL;
	protected const string VIEW = Library.VIEW;

	protected const string AMMO = Library.AMMO;
	protected const string EQUIP = Library.EQUIP;
	protected const string WEAPON = Library.WEAPON;

	protected const string COMBAT = Library.COMBAT;
	protected const string CHARGE = Library.CHARGE;
	protected const string PROJECTILE = Library.PROJECTILE;

	protected const string EXPLOSION = Library.EXPLOSION;
	protected const string BULLET = Library.BULLET;
	protected const string MELEE = Library.MELEE;
	protected const string MAGIC = Library.MAGIC;

	protected const string SPAWNING = Library.SPAWNING;
	protected const string SPAWNER = Library.SPAWNER;
	protected const string WAVES = Library.WAVES;

	protected const string TAG_TRIGGER = Library.TAG_TRIGGER;
	protected const string TAG_LADDER = Library.TAG_LADDER;

	protected const string TAG_PROJECTILE = Library.TAG_PROJECTILE;
	protected const string TAG_ENTITY = Library.TAG_ENTITY;
	protected const string TAG_EQUIP = Library.TAG_EQUIP;

	protected const string TAG_SPECTATOR = Library.TAG_SPECTATOR;
	protected const string TAG_PLAYER = Library.TAG_PLAYER;
	protected const string TAG_PAWN = Library.TAG_PAWN;
	protected const string TAG_DEAD = Library.TAG_DEAD;
	protected const string TAG_HULL = Library.TAG_HULL;
	protected const string TAG_NPC = Library.TAG_NPC;

	/// <summary>
	/// Is this currently loaded in a valid editor scene? <br />
	/// You can use this with <see cref="HideIfAttribute"/> or <see cref="ShowIfAttribute"/>.
	/// </summary>
	public bool InEditor => this.InEditor();

	/// <summary>
	/// Is this currently loaded in a valid play mode scene? <br />
	/// You can use this with <see cref="HideIfAttribute"/> or <see cref="ShowIfAttribute"/>.
	/// </summary>
	public bool InGame => this.InGame();

	/// <summary>
	/// Allows for custom teleportation behavior.
	/// </summary>
	/// <remarks> Example: telling a pawn to set their eye rotation instead. </remarks>
	/// <returns> If the teleport was successful. </returns>
	public virtual bool TryTeleport( in Transform tWorld )
		=> false;
}
