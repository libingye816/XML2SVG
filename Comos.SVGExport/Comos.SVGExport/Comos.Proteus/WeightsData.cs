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
	public class WeightsData
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

		public WeightsData()
		{
		}
	}
}