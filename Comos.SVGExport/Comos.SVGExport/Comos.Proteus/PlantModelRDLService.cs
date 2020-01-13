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
	[XmlType(AnonymousType=true)]
	public class PlantModelRDLService
	{
		private string serviceAddressField;

		private string queryParameterField;

		[DefaultValue("query")]
		[XmlAttribute]
		public string QueryParameter
		{
			get
			{
				return this.queryParameterField;
			}
			set
			{
				this.queryParameterField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string ServiceAddress
		{
			get
			{
				return this.serviceAddressField;
			}
			set
			{
				this.serviceAddressField = value;
			}
		}

		public PlantModelRDLService()
		{
			this.queryParameterField = "query";
		}
	}
}