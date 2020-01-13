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
	public class ControlPointArray
	{
		private Coordinate[][] controlPointsField;

		[XmlArray(Form=XmlSchemaForm.Unqualified)]
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

		public ControlPointArray()
		{
		}
	}
}