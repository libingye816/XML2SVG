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
	public class TextStringFormatSpecification
	{
		private Comos.Proteus.ObjectAttributesReference[] objectAttributesReferenceField;

		[XmlElement("ObjectAttributesReference")]
		public Comos.Proteus.ObjectAttributesReference[] ObjectAttributesReference
		{
			get
			{
				return this.objectAttributesReferenceField;
			}
			set
			{
				this.objectAttributesReferenceField = value;
			}
		}

		public TextStringFormatSpecification()
		{
		}
	}
}