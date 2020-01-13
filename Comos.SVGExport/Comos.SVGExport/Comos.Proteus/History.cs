using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlRoot(Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class History
	{
		private Transaction[] itemsField;

		private string numTransactionsField;

		[XmlElement("Transaction")]
		public Transaction[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlAttribute(DataType="nonNegativeInteger")]
		public string NumTransactions
		{
			get
			{
				return this.numTransactionsField;
			}
			set
			{
				this.numTransactionsField = value;
			}
		}

		public History()
		{
		}
	}
}