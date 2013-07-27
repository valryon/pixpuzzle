using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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

		private string mPuzzlePath;
		private string mCustomPuzzlePath;
		private string mSavedgamePath;

		/// <summary>
		/// Tell the service where to look for puzzles and where is the save game
		/// </summary>
		/// <param name="puzzlePath">Puzzle path.</param>
		/// <param name="savedgamePath">Savedgame path.</param>
		public void Initialize (string puzzlePath, string saveFilename)
		{
			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

			mCustomPuzzlePath = System.IO.Path.Combine(path, "custom");
			if (Directory.Exists (mCustomPuzzlePath) == false) {
				Directory.CreateDirectory (mCustomPuzzlePath);
			}

			// Load the saved infos
			//----------------------------------------------------------------
			this.mSavedgamePath = System.IO.Path.Combine (path, saveFilename);

			if (File.Exists (mSavedgamePath) == false) {

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

			this.mPuzzlePath = puzzlePath;

			if (Directory.Exists (puzzlePath) == false) {
				throw new ArgumentException ("Invalid puzzle path location: " + puzzlePath + " is not a valid directory!");
			}

			// Look for puzzles!
			var knowPuzzles = Savedgame.Puzzles.Where(p => p.IsCustom == false).Select (p => p.Filename);
			bool shouldSave = false;

			foreach (string file in Directory.GetFiles(this.mPuzzlePath)) {
				if (knowPuzzles.Contains (file) == false) {
					Savedgame.Puzzles.Add (new PuzzleData () {
						Filename = file,
						IsNew = true,
						IsCustom = false,
						OwnerId = "Pixelnest Studio"
					});

					shouldSave = true;
				}
			}

			if (shouldSave) {
				Save ();
			}
		}

		/// <summary>
		/// Get all known puzzles
		/// </summary>
		/// <returns>The puzzles.</returns>
		/// <param name="newOnly">If set to <c>true</c> new only.</param>
		/// <param name="removeCompleted">If set to <c>true</c> remove completed.</param>
		public List<PuzzleData> GetPuzzles (bool newOnly = false, bool removeCompleted = false)
		{
			var puzzles = Savedgame.Puzzles;

			if (newOnly) {
				puzzles = puzzles.Where (p => p.IsNew).ToList ();
			}

			if (removeCompleted) {
				puzzles = puzzles.Where (p => p.BestScore.HasValue == false).ToList ();
			}


			return puzzles.ToList ();
		}

		/// <summary>
		/// Save a new custom puzzle
		/// </summary>
		#if IOS
		public PuzzleData AddPuzzle (string filename, string owner, MonoTouch.UIKit.UIImage image)
		{
			string completeFilePath = System.IO.Path.Combine (mCustomPuzzlePath, filename);

			Logger.I ("Adding a new " + owner+ " puzzle " + filename);

			// Save the new image
			MonoTouch.Foundation.NSError error;
			image.AsPNG ().Save (completeFilePath, true, out error);

			if (error == null) {
				Logger.I ("Adding puzzle OK");
			}
			else {
				Logger.E ("Adding puzzle KO: " + error);
			}

			PuzzleData newPuzzle = new PuzzleData () {
				Filename = completeFilePath,
				IsCustom = true,
				IsNew = true,
				OwnerId = owner
			};

			Savedgame.Puzzles.Add (newPuzzle);
			Save ();

			return newPuzzle;
		}
#endif

		/// <summary>
		/// Save the game properties
		/// </summary>
		public void Save ()
		{
			Logger.I ("Saving game...");

			try {
				XmlSerializer xs = new XmlSerializer (typeof(Savedgame));
				using (StreamWriter wr = new StreamWriter(mSavedgamePath)) {
					xs.Serialize (wr, Savedgame);
				}

				Logger.I ("Saving OK");
			} catch (Exception e) {
				Logger.E ("Save KO", e);
			}
		}

		/// <summary>
		/// Load the game properties
		/// </summary>
		public void Load ()
		{	
			Logger.I ("Loading savedgame...");

			try {
				XmlSerializer xs = new XmlSerializer (typeof(Savedgame));
				using (StreamReader rd = new StreamReader(mSavedgamePath)) {
					Savedgame = xs.Deserialize (rd) as Savedgame;
				}

				Logger.I ("Loading OK. " + Savedgame.Puzzles.Count + " known puzzles.");
			} catch (Exception e) {
				Logger.E ("Loading KO", e);
				Savedgame = new Savedgame ();
			}
		}

		public Savedgame Savedgame { get; private set; }
	}
}

