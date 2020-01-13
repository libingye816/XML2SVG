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
	public class CompositeCurve : Curve
	{
		private object[] itemsField;

		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("Presentation", typeof(Presentation))]
		public object[] Items
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

		public CompositeCurve()
		{
		}
	}
}