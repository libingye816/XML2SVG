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
	public class Transaction
	{
		private string[] remarkField;

		private Comos.Proteus.GenericAttributes[] genericAttributesField;

		private TransactionType typeField;

		private bool typeFieldSpecified;

		private DateTime dateField;

		private bool dateFieldSpecified;

		private DateTime timeField;

		private bool timeFieldSpecified;

		private string companyNameField;

		private string personField;

		[XmlAttribute]
		public string CompanyName
		{
			get
			{
				return this.companyNameField;
			}
			set
			{
				this.companyNameField = value;
			}
		}

		[XmlAttribute(DataType="date")]
		public DateTime Date
		{
			get
			{
				return this.dateField;
			}
			set
			{
				this.dateField = value;
			}
		}

		[XmlIgnore]
		public bool DateSpecified
		{
			get
			{
				return this.dateFieldSpecified;
			}
			set
			{
				this.dateFieldSpecified = value;
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

		[XmlAttribute]
		public string Person
		{
			get
			{
				return this.personField;
			}
			set
			{
				this.personField = value;
			}
		}

		[XmlElement("Remark", Form=XmlSchemaForm.Unqualified)]
		public string[] Remark
		{
			get
			{
				return this.remarkField;
			}
			set
			{
				this.remarkField = value;
			}
		}

		[XmlAttribute(DataType="time")]
		public DateTime Time
		{
			get
			{
				return this.timeField;
			}
			set
			{
				this.timeField = value;
			}
		}

		[XmlIgnore]
		public bool TimeSpecified
		{
			get
			{
				return this.timeFieldSpecified;
			}
			set
			{
				this.timeFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public TransactionType Type
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

		public Transaction()
		{
		}
	}
}