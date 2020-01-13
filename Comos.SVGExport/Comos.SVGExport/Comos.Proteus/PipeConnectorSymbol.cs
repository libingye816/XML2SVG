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
	public class PipeConnectorSymbol : AnnotationItem
	{
		private Comos.Proteus.CrossPageConnection crossPageConnectionField;

		public Comos.Proteus.CrossPageConnection CrossPageConnection
		{
			get
			{
				return this.crossPageConnectionField;
			}
			set
			{
				this.crossPageConnectionField = value;
			}
		}

		public PipeConnectorSymbol()
		{
		}
	}
}