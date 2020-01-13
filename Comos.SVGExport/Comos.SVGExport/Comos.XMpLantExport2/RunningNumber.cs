using System;
using System.Runtime.CompilerServices;

namespace Comos.SVGExport
{
	internal class RunningNumber
	{
		public int Number
		{
			get;
			private set;
		}

		public RunningNumber(int initialNumber = 1)
		{
			this.Number = initialNumber;
		}

		public int GetNextNumber()
		{
			int number = this.Number + 1;
			this.Number = number;
			return number;
		}
	}
}