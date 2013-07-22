using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace PixPuzzle.Data
{
	/// <summary>
	/// Manage puzzles: list, completed or not, savedgame, etc
	/// </summary>
	public class PuzzleService
	{
		#region Singleton

		private PuzzleService ()
		{
		}

		private static PuzzleService instance;

		public static PuzzleService Instance {
			get {
				if (instance == null) {
					instance = new PuzzleService ();
				}

				return instance;
			}
		}

		#endregion

		private string puzzlePath;
		private string savedgamePath;

		public Savedgame Savedgame { get; private set; }

		/// <summary>
		/// Tell the service where to look for puzzles and where is the save game
		/// </summary>
		/// <param name="puzzlePath">Puzzle path.</param>
		/// <param name="savedgamePath">Savedgame path.</param>
		public void Initialize (string puzzlePath, string savedgamePath) {

			// Load the saved infos
			//----------------------------------------------------------------
			this.savedgamePath = savedgamePath;
			if (File.Exists (savedgamePath) == false) {

				Logger.I ("No savedgame found: creating a new one");

				// Create a new save game
				Savedgame = new Savedgame ();
				Save ();

			} else {

				// Load the existing one
				Load ();
			}

			// Find puzzles
			//----------------------------------------------------------------
			Logger.I ("Initializing puzzles...");

			this.puzzlePath = puzzlePath;

			if (Directory.Exists (puzzlePath) == false) {
				throw new ArgumentException ("Invalid puzzle path location: "+ puzzlePath+ " is not a valid directory!");
			}

			// Look for puzzles!
			var knowPuzzles = Savedgame.Puzzles.Select (p => p.Filename);

			foreach (string file in Directory.GetFiles(this.puzzlePath)) {
				if(knowPuzzles.Contains(file) == false) {
					Savedgame.Puzzles.Add (new PuzzleData () {
						Filename = file,
						IsNew = true,
						IsCustom = false,
						OwnerId = "Pixelnest Studio"
					});
				}
			}
		}

		/// <summary>
		/// Get all known puzzles
		/// </summary>
		/// <returns>The puzzles.</returns>
		/// <param name="newOnly">If set to <c>true</c> new only.</param>
		/// <param name="removeCompleted">If set to <c>true</c> remove completed.</param>
		public List<PuzzleData> GetPuzzles(bool newOnly = false, bool removeCompleted = false) {

			var puzzles = Savedgame.Puzzles;

			if (newOnly) {
				puzzles = puzzles.Where(p => p.IsNew).ToList();
			}

			if (removeCompleted) {
				puzzles = puzzles.Where (p => p.BestScore.HasValue == false).ToList ();
			}

			return puzzles.ToList();
		}

		/// <summary>
		/// Save a new custom puzzle
		/// </summary>
		public void AddPuzzle(string filename, string owner, byte[][] image) {

		}

		/// <summary>
		/// Save the game properties
		/// </summary>
		public void Save() {
			
			Logger.I ("Saving game...");

			// TODO
			Logger.I ("Saving OK");


		}

		/// <summary>
		/// Load the game properties
		/// </summary>
		public void Load() {
			
			Logger.I ("Loading savedgame...");

			Savedgame = new Savedgame();
			// TODO

			Logger.I ("Loading OK");
		}

	}
}

