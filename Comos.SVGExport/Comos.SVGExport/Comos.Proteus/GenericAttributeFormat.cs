using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(AnonymousType=true)]
	public enum GenericAttributeFormat
	{
		@string,
		@byte,
		@short,
		integer,
		@long,
		@ulong,
		@float,
		@double,
		anyURI
	}
}