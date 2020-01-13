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
	[XmlRoot(Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class CrossPageConnection
	{
		private PersistentID itemField;

		private string drawingNameField;

		private string linkLabelField;

		[XmlAttribute]
		public string DrawingName
		{
			get
			{
				return this.drawingNameField;
			}
			set
			{
				this.drawingNameField = value;
			}
		}

		[XmlElement("LinkedPersistentID", Form=XmlSchemaForm.Unqualified)]
		public PersistentID Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		[XmlAttribute]
		public string LinkLabel
		{
			get
			{
				return this.linkLabelField;
			}
			set
			{
				this.linkLabelField = value;
			}
		}

		public CrossPageConnection()
		{
		}
	}
}