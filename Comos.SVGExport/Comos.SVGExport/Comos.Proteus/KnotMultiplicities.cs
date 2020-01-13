using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	public class KnotMultiplicities
	{
		private string[] multiplicityField;

		[XmlElement("Multiplicity", Form=XmlSchemaForm.Unqualified, DataType="nonNegativeInteger")]
		public string[] Multiplicity
		{
			get
			{
				return this.multiplicityField;
			}
			set
			{
				this.multiplicityField = value;
			}
		}

		public KnotMultiplicities()
		{
		}
	}
}