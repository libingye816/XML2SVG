using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	public enum TransactionType
	{
		[XmlEnum("to approve")]
		toapprove,
		[XmlEnum("to check")]
		tocheck,
		[XmlEnum("to copy")]
		tocopy,
		[XmlEnum("to create")]
		tocreate,
		[XmlEnum("to modify")]
		tomodify,
		[XmlEnum("to request")]
		torequest,
		[XmlEnum("to release")]
		torelease
	}
}