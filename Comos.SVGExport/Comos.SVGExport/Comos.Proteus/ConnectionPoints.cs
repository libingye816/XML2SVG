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
	public class ConnectionPoints
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private Comos.Proteus.Node[] nodeField;

		private Comos.Proteus.GenericAttributes[] genericAttributesField;

		private string numPointsField;

		private string flowInField;

		private string flowOutField;

		public Comos.Proteus.Extent Extent
		{
			get
			{
				return this.extentField;
			}
			set
			{
				this.extentField = value;
			}
		}

		[XmlAttribute(DataType="positiveInteger")]
		public string FlowIn
		{
			get
			{
				return this.flowInField;
			}
			set
			{
				this.flowInField = value;
			}
		}

		[XmlAttribute(DataType="positiveInteger")]
		public string FlowOut
		{
			get
			{
				return this.flowOutField;
			}
			set
			{
				this.flowOutField = value;
			}
		}

		[XmlElement("GenericAttributes")]
		public Comos.Proteus.GenericAttributes[] GenericAttributes
		{
			get
			{
				return this.genericAttributesField;
			}
			set
			{
				this.genericAttributesField = value;
			}
		}

		[XmlElement("Node")]
		public Comos.Proteus.Node[] Node
		{
			get
			{
				return this.nodeField;
			}
			set
			{
				this.nodeField = value;
			}
		}

		[XmlAttribute(DataType="nonNegativeInteger")]
		public string NumPoints
		{
			get
			{
				return this.numPointsField;
			}
			set
			{
				this.numPointsField = value;
			}
		}

		public Comos.Proteus.Presentation Presentation
		{
			get
			{
				return this.presentationField;
			}
			set
			{
				this.presentationField = value;
			}
		}

		public ConnectionPoints()
		{
		}
	}
}