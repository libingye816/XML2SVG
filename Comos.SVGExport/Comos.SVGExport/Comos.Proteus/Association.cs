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
	public class Association
	{
		private AssociationType typeField;

		private string itemIDField;

		private string tagNameField;

		private string nameField;

		private string itemURIField;

		private string persistentIDIdentifierField;

		private string persistentIDContextField;

		[XmlAttribute(DataType="IDREF")]
		public string ItemID
		{
			get
			{
				return this.itemIDField;
			}
			set
			{
				this.itemIDField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string ItemURI
		{
			get
			{
				return this.itemURIField;
			}
			set
			{
				this.itemURIField = value;
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
		public string PersistentIDContext
		{
			get
			{
				return this.persistentIDContextField;
			}
			set
			{
				this.persistentIDContextField = value;
			}
		}

		[XmlAttribute]
		public string PersistentIDIdentifier
		{
			get
			{
				return this.persistentIDIdentifierField;
			}
			set
			{
				this.persistentIDIdentifierField = value;
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

		[XmlAttribute]
		public AssociationType Type
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

		public Association()
		{
		}
	}
}