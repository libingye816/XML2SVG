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
	public class Identifier
	{
		private IdentifierElement[] itemsField;

		private string purposeField;

		[XmlElement("IdentifierElement")]
		public IdentifierElement[] Items
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

		[XmlAttribute]
		public string Purpose
		{
			get
			{
				return this.purposeField;
			}
			set
			{
				this.purposeField = value;
			}
		}

		public Identifier()
		{
		}
	}
}