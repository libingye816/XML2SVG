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
	[XmlRoot("Weights", Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class Weights1
	{
		private double[] controlPointWeightField;

		[XmlElement("ControlPointWeight")]
		public double[] ControlPointWeight
		{
			get
			{
				return this.controlPointWeightField;
			}
			set
			{
				this.controlPointWeightField = value;
			}
		}

		public Weights1()
		{
		}
	}
}