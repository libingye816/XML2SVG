using REPORTLib;
using System;

namespace Comos.ReportAnalysis
{
	public class FoundReportItemEventArgs : EventArgs
	{
		private Item m_DocItem;

		public Item DocItem
		{
			get
			{
				return this.m_DocItem;
			}
		}

		public FoundReportItemEventArgs(Item DocItem)
		{
			this.m_DocItem = DocItem;
		}
	}
}