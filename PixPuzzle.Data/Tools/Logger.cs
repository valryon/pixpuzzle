using System;

namespace PixPuzzle
{
	public static class Logger
	{
		public static void D (string message)
		{
			Console.WriteLine ("DEBUG: " + message);
		}

		public static void I (string message)
		{
			Console.WriteLine ("INFO : " + message);
		}

		public static void W (string message)
		{
			Console.WriteLine ("WARN : " + message);
		}

		public static void E (string message)
		{
			Console.WriteLine ("ERROR: " + message);
		}

		public static void E (string message, Exception e)
		{
			Console.WriteLine ("ERROR: " + message + "\n" + e.ToString ());
		}
	}
}

