using System;
namespace PixPuzzle.Data
{
	public interface IGrid
	{
		event Action GridCompleted;

		void SetupGrid (CellColor[][] pixels);
	
		int CellSize 
		{
			get;
		}
	}
}

