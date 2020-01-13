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
	public class Connection
	{
		private string toIDField;

		private string toNodeField;

		private string fromIDField;

		private string fromNodeField;

		[XmlAttribute]
		public string FromID
		{
			get
			{
				return this.fromIDField;
			}
			set
			{
				this.fromIDField = value;
			}
		}

		[XmlAttribute(DataType="positiveInteger")]
		public string FromNode
		{
			get
			{
				return this.fromNodeField;
			}
			set
			{
				this.fromNodeField = value;
			}
		}

		[XmlAttribute]
		public string ToID
		{
			get
			{
				return this.toIDField;
			}
			set
			{
				this.toIDField = value;
			}
		}

		[XmlAttribute(DataType="positiveInteger")]
		public string ToNode
		{
			get
			{
				return this.toNodeField;
			}
			set
			{
				this.toNodeField = value;
			}
		}

		public Connection()
		{
		}
	}
}