using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public enum Items1ChoiceType1
	{
		Component,
		Discipline,
		Equipment,
		LowerLimitDesignPressure,
		LowerLimitDesignTemperature,
		MaximumDesignPressure,
		MaximumDesignTemperature,
		MinimumDesignPressure,
		MinimumDesignTemperature,
		Nozzle,
		UpperLimitDesignPressure,
		UpperLimitDesignTemperature
	}
}