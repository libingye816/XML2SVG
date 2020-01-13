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
	[XmlInclude(typeof(PipingComponent))]
	public class PlantItem
	{
		private object[] itemsField;

		private ItemsChoiceType3[] itemsElementNameField;

		private string idField;

		private string tagNameField;

		private string specificationField;

		private string specificationURIField;

		private string stockNumberField;

		private string componentClassField;

		private string componentClassURIField;

		private string componentNameField;

		private PlantItemComponentType componentTypeField;

		private bool componentTypeFieldSpecified;

		private string revisionField;

		private string revisionURIField;

		private PlantItemStatus statusField;

		private bool statusFieldSpecified;

		private string statusURIField;

		[XmlAttribute]
		public string ComponentClass
		{
			get
			{
				return this.componentClassField;
			}
			set
			{
				this.componentClassField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string ComponentClassURI
		{
			get
			{
				return this.componentClassURIField;
			}
			set
			{
				this.componentClassURIField = value;
			}
		}

		[XmlAttribute]
		public string ComponentName
		{
			get
			{
				return this.componentNameField;
			}
			set
			{
				this.componentNameField = value;
			}
		}

		[XmlAttribute]
		public PlantItemComponentType ComponentType
		{
			get
			{
				return this.componentTypeField;
			}
			set
			{
				this.componentTypeField = value;
			}
		}

		[XmlIgnore]
		public bool ComponentTypeSpecified
		{
			get
			{
				return this.componentTypeFieldSpecified;
			}
			set
			{
				this.componentTypeFieldSpecified = value;
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
		[XmlElement("Association", typeof(Association))]
		[XmlElement("ConnectionPoints", typeof(ConnectionPoints))]
		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("Description", typeof(string))]
		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("History", typeof(History))]
		[XmlElement("Identifier", typeof(Identifier))]
		[XmlElement("Label", typeof(Label))]
		[XmlElement("Manufacturer", typeof(string), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Material", typeof(string))]
		[XmlElement("MaterialDescription", typeof(string))]
		[XmlElement("MaterialOfConstruction", typeof(Comos.Proteus.String))]
		[XmlElement("ModelNumber", typeof(string), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("PersistentID", typeof(PersistentID))]
		[XmlElement("Position", typeof(Comos.Proteus.Position))]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("Scale", typeof(Scale))]
		[XmlElement("Supplier", typeof(string), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Surface", typeof(Surface))]
		[XmlElement("Text", typeof(Text))]
		[XmlElement("Weight", typeof(MassDouble))]
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
		public ItemsChoiceType3[] ItemsElementName
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
		public string Revision
		{
			get
			{
				return this.revisionField;
			}
			set
			{
				this.revisionField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string RevisionURI
		{
			get
			{
				return this.revisionURIField;
			}
			set
			{
				this.revisionURIField = value;
			}
		}

		[XmlAttribute]
		public string Specification
		{
			get
			{
				return this.specificationField;
			}
			set
			{
				this.specificationField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string SpecificationURI
		{
			get
			{
				return this.specificationURIField;
			}
			set
			{
				this.specificationURIField = value;
			}
		}

		[XmlAttribute]
		public PlantItemStatus Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		[XmlIgnore]
		public bool StatusSpecified
		{
			get
			{
				return this.statusFieldSpecified;
			}
			set
			{
				this.statusFieldSpecified = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string StatusURI
		{
			get
			{
				return this.statusURIField;
			}
			set
			{
				this.statusURIField = value;
			}
		}

		[XmlAttribute]
		public string StockNumber
		{
			get
			{
				return this.stockNumberField;
			}
			set
			{
				this.stockNumberField = value;
			}
		}

		[XmlAttribute]
		public string TagName
		{
			get
			{
				return this.tagNameField;
			}
			set
			{
				this.tagNameField = value;
			}
		}

		public PlantItem()
		{
		}
	}
}