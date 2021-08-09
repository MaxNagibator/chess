namespace bg.chess.domain
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Игровое поле.
	/// </summary>
	public class Field
	{
		/// <summary>
		/// Позиции на поле.
		/// </summary>
		public List<Position> Positions { get; set; }

		/// <summary>
		/// Ширина поля
		/// </summary>
		public int FieldWidth { get; internal set; }

		/// <summary>
		/// Высота поля
		/// </summary>
		public int FieldHeight { get; internal set; }

		/// <summary>
		/// Взять позицию по координатам.
		/// </summary>
		/// <param name="x">По ширине.</param>
		/// <param name="y">По высоте.</param>
		/// <returns>Позиция.</returns>
		public Position this[int x, int y]
		{
			get
			{
				return Positions.First(p => p.X == x && p.Y == y);
			}
		}

		public void Move(Side side, int fromX, int fromY, int toX, int toY)
		{
			var piece = this[fromX, fromY].Piece;
			if (piece == null)
			{
				throw new Exception("piece not found by " + fromX + "/" + fromY);
			}
			if (piece.Side != side)
			{
				throw new Exception("piece not this side");
			}
			//piece.CanMove(this);
		}
	}
}
