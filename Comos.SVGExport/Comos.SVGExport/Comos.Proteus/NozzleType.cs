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
	public class NozzleType
	{
		private NozzleTypeValue valueField;

		[DefaultValue(NozzleTypeValue.Flanged)]
		[XmlAttribute]
		public NozzleTypeValue Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		public NozzleType()
		{
			this.valueField = NozzleTypeValue.Flanged;
		}
	}
}