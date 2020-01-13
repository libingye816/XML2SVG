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
	public class PersistentID
	{
		private string identifierField;

		private string contextField;

		[XmlAttribute]
		public string Context
		{
			get
			{
				return this.contextField;
			}
			set
			{
				this.contextField = value;
			}
		}

		[XmlAttribute]
		public string Identifier
		{
			get
			{
				return this.identifierField;
			}
			set
			{
				this.identifierField = value;
			}
		}

		public PersistentID()
		{
		}
	}
}