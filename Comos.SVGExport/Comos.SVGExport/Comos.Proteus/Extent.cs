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
	public class Extent
	{
		private Comos.Proteus.Min minField;

		private Comos.Proteus.Max maxField;

		public Comos.Proteus.Max Max
		{
			get
			{
				return this.maxField;
			}
			set
			{
				this.maxField = value;
			}
		}

		public Comos.Proteus.Min Min
		{
			get
			{
				return this.minField;
			}
			set
			{
				this.minField = value;
			}
		}

		public Extent()
		{
		}
	}
}