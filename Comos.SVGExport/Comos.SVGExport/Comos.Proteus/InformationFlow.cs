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
	public class InformationFlow : PlantItem
	{
		private object[] items1Field;

		[XmlElement("CenterLine", typeof(CenterLine))]
		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("Connection", typeof(Connection))]
		[XmlElement("SignalConnectorSymbol", typeof(SignalConnectorSymbol))]
		[XmlElement("Symbol", typeof(Symbol))]
		public object[] Items1
		{
			get
			{
				return this.items1Field;
			}
			set
			{
				this.items1Field = value;
			}
		}

		public InformationFlow()
		{
		}
	}
}