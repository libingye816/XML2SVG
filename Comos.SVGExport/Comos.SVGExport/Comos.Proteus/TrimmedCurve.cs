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
	public class TrimmedCurve : Curve
	{
		private Curve itemField;

		private Comos.Proteus.GenericAttributes genericAttributesField;

		private double startAngleField;

		private double endAngleField;

		[XmlAttribute]
		public double EndAngle
		{
			get
			{
				return this.endAngleField;
			}
			set
			{
				this.endAngleField = value;
			}
		}

		public Comos.Proteus.GenericAttributes GenericAttributes
		{
			get
			{
				return this.genericAttributesField;
			}
			set
			{
				this.genericAttributesField = value;
			}
		}

		[XmlElement("Circle", typeof(Circle))]
		[XmlElement("Ellipse", typeof(Ellipse))]
		public Curve Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		[XmlAttribute]
		public double StartAngle
		{
			get
			{
				return this.startAngleField;
			}
			set
			{
				this.startAngleField = value;
			}
		}

		public TrimmedCurve()
		{
		}
	}
}