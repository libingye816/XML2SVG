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
	[XmlRoot("ControlPointArray", Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class ControlPointArray1
	{
		private Coordinate[][] controlPointsField;

		[XmlArrayItem("Coordinate", typeof(Coordinate), IsNullable=false)]
		public Coordinate[][] ControlPoints
		{
			get
			{
				return this.controlPointsField;
			}
			set
			{
				this.controlPointsField = value;
			}
		}

		public ControlPointArray1()
		{
		}
	}
}