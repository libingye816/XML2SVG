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
	public class Axis
	{
		private double xField;

		private double yField;

		private double zField;

		[XmlAttribute]
		public double X
		{
			get
			{
				return this.xField;
			}
			set
			{
				this.xField = value;
			}
		}

		[XmlAttribute]
		public double Y
		{
			get
			{
				return this.yField;
			}
			set
			{
				this.yField = value;
			}
		}

		[XmlAttribute]
		public double Z
		{
			get
			{
				return this.zField;
			}
			set
			{
				this.zField = value;
			}
		}

		public Axis()
		{
		}
	}
}