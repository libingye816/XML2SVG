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
	public class Weights
	{
		private double[][] weightsDataField;

		[XmlArray(Form=XmlSchemaForm.Unqualified)]
		[XmlArrayItem("ControlPointWeight", typeof(double), IsNullable=false)]
		public double[][] WeightsData
		{
			get
			{
				return this.weightsDataField;
			}
			set
			{
				this.weightsDataField = value;
			}
		}

		public Weights()
		{
		}
	}
}