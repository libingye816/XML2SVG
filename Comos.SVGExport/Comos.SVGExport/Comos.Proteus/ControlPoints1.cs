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
	[XmlRoot("ControlPoints", Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class ControlPoints1
	{
		private Comos.Proteus.Coordinate[] coordinateField;

		[XmlElement("Coordinate")]
		public Comos.Proteus.Coordinate[] Coordinate
		{
			get
			{
				return this.coordinateField;
			}
			set
			{
				this.coordinateField = value;
			}
		}

		public ControlPoints1()
		{
		}
	}
}