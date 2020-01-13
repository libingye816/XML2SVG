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
	public class AnnotationItem
	{
		private object[] itemsField;

		private string idField;

		private string componentClassField;

		private string componentClassURIField;

		private string componentNameField;

		private AnnotationItemComponentType componentTypeField;

		private bool componentTypeFieldSpecified;

		private string revisionField;

		private string revisionURIField;

		private AnnotationItemStatus statusField;

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
		public AnnotationItemComponentType ComponentType
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

		[XmlElement("Association", typeof(Association))]
		[XmlElement("ConnectionPoints", typeof(ConnectionPoints))]
		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("Description", typeof(string))]
		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("History", typeof(History))]
		[XmlElement("PersistentID", typeof(PersistentID))]
		[XmlElement("Position", typeof(Position))]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("Scale", typeof(Scale))]
		[XmlElement("Symbol", typeof(Symbol))]
		[XmlElement("Text", typeof(Text))]
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
		public AnnotationItemStatus Status
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

		public AnnotationItem()
		{
		}
	}
}