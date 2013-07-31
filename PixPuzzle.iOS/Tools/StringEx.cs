using System;

namespace System
{
	/// <summary>
	/// Extension for System.String
	/// </summary>
	public static class StringEx
	{
		/// <summary>
		/// Convert the current text string in a Base64 string
		/// </summary>
		/// <returns>The base64.</returns>
		/// <param name="s">S.</param>
		public static string ToBase64(this string s) 
		{
			byte[] stringData = System.Text.Encoding.UTF8.GetBytes(s);
			return Convert.ToBase64String(stringData);
		}
	}
}