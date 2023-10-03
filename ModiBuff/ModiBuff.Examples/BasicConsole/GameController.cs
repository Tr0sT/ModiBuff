using ModiBuff.Core;

namespace ModiBuff.Examples.BasicConsole
{
	/// <summary>
	///		Simple solo example, of player unit fighting a single enemy unit
	/// </summary>
	public sealed class GameController : IGameController
	{
		private readonly ModifierIdManager _idManager;
		private readonly ModifierRecipes _recipes;
		private readonly ModifierPool _pool;

		private readonly Unit _player;

		private readonly EnemySpawner _enemySpawner;

		public GameController()
		{
			//We set the logger first, so ModiBuff can tell us if anything goes wrong 
			Logger.SetLogger<ConsoleLogger>();
			//(Optional) We can set any default in the static config file
			Config.PoolSize = 100;

			//These 3 classes need to always be created in the start of the game
			//The ModifierIdManage and ModifierRecipe will help us get info about the modifiers
			_idManager = new ModifierIdManager();
			_recipes = new ModifierRecipes(_idManager);
			//We need to register our recipes inside the pool, so we can pre-allocate the modifiers
			//Most likely you won't use the pool directly, but we still need to create it 
			_pool = new ModifierPool(_recipes);
			//_defenseModifierId = idManager.GetId("Defense");

			//Now we've come to the game logic initialization, which will be custom for your game
			//Here we have just one player, and one enemy spawning each time the enemy dies
			_player = new Unit("Player", 100, 5);

			_enemySpawner = new EnemySpawner(_player);
			_enemySpawner.Spawn();

			//We can add modifiers to any unit at this stage (after the pool is created)
			//Here we're adding a modifier called "DoT" to the player
			//But we're adding it as an applier, and not as a normal modifier
			//This means that instead of it being applier to the player
			//it will be applied to a unit that the player attacks
			_player.ModifierController.TryAddApplier(_idManager.GetId("DoT"), false, ApplierType.Attack);
			_player.ModifierController.TryAddApplier(_idManager.GetId("InitHeal"), false, ApplierType.Cast);
		}

		public bool Update()
		{
			bool isRunning = PlayerAction();
			if (isRunning == false)
				return false;

			//We're making the game turn-based, by having a constant 1 delta
			const float delta = 1;

			_player.Update(delta);
			_enemySpawner.Update(delta);

			return true;
		}

		private bool PlayerAction()
		{
			bool validAction = false;
			while (validAction == false)
			{
				Console.GameMessage("Actions: Attack - 1, Cast - 2, Quit - q");
				string action = System.Console.ReadLine();
				switch (action)
				{
					case "1":
						validAction = true;
						_player.AutoAttack();
						break;
					case "2":
						validAction = PlayerCastAction();
						break;
					case "q":
						return false;
					default:
						Console.GameMessage("Invalid action");
						break;
				}
			}

			return true;
		}

		private bool PlayerCastAction()
		{
			//Display all possible modifiers to cast, then when one was chosen, choose the target
			var modifierIds = _player.ModifierController.GetApplierCastModifierIds();

			while (true)
			{
				Console.GameMessage("Choose modifier to cast, or c to cancel");
				for (int i = 0; i < modifierIds.Count; i++)
					Console.GameMessage($"{i + 1} - {_recipes.GetModifierInfo(modifierIds[i]).Name}");
				
				string castAction = System.Console.ReadLine();
				if (int.TryParse(castAction, out int castActionInt))
				{
					if (castActionInt > 0 && castActionInt <= modifierIds.Count)
					{
						//TODO: choosing target
						_player.TryCast(modifierIds[castActionInt - 1], _player);
						break;
					}
				}

				if (castAction == "c")
					return false;

				Console.GameMessage("Invalid action");
			}

			return true;
		}
	}
}