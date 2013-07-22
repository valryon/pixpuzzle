using System;

namespace PixPuzzle
{
	public static class Logger
	{
		public static void D(string message){
			Console.WriteLine ("PixPuzzle - DEBUG: " + message);
		}

		public static void I(string message){
			Console.WriteLine ("PixPuzzle - INFO : " + message);
		}

		public static void W(string message){
			Console.WriteLine ("PixPuzzle - WARN : " + message);
		}

		public static void E(string message){
			Console.WriteLine ("PixPuzzle - ERROR: " + message);
		}
	}
}

