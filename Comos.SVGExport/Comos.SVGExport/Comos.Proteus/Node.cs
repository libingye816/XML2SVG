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
	public class Node
	{
		private object[] itemsField;

		private ItemsChoiceType2[] itemsElementNameField;

		private string idField;

		private string nameField;

		private NodeType typeField;

		private bool typeFieldSpecified;

		private NodeFunction functionField;

		private bool functionFieldSpecified;

		private NodeFlow flowField;

		private bool flowFieldSpecified;

		[XmlAttribute]
		public NodeFlow Flow
		{
			get
			{
				return this.flowField;
			}
			set
			{
				this.flowField = value;
			}
		}

		[XmlIgnore]
		public bool FlowSpecified
		{
			get
			{
				return this.flowFieldSpecified;
			}
			set
			{
				this.flowFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public NodeFunction Function
		{
			get
			{
				return this.functionField;
			}
			set
			{
				this.functionField = value;
			}
		}

		[XmlIgnore]
		public bool FunctionSpecified
		{
			get
			{
				return this.functionFieldSpecified;
			}
			set
			{
				this.functionFieldSpecified = value;
			}
		}

		[XmlAttribute(DataType="ID")]
		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("ConnectionType", typeof(Comos.Proteus.String))]
		[XmlElement("EndPreparation", typeof(Comos.Proteus.String))]
		[XmlElement("GasketGap", typeof(LengthDouble))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("InsideDiameter", typeof(LengthDouble))]
		[XmlElement("NominalDiameter", typeof(LengthDouble))]
		[XmlElement("OutsideDiameter", typeof(LengthDouble))]
		[XmlElement("PersistentID", typeof(PersistentID))]
		[XmlElement("Position", typeof(Position))]
		[XmlElement("Rating", typeof(Comos.Proteus.String))]
		[XmlElement("ScheduleThickness", typeof(LengthString))]
		[XmlElement("WeldType", typeof(Comos.Proteus.String))]
		public object[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType2[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public NodeType Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return this.typeFieldSpecified;
			}
			set
			{
				this.typeFieldSpecified = value;
			}
		}

		public Node()
		{
		}
	}
}