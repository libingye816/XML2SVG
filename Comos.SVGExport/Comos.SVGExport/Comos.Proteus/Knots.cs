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
	public class Knots
	{
		private double[] knotField;

		[XmlElement("Knot", Form=XmlSchemaForm.Unqualified)]
		public double[] Knot
		{
			get
			{
				return this.knotField;
			}
			set
			{
				this.knotField = value;
			}
		}

		public Knots()
		{
		}
	}
}