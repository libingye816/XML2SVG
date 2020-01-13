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
	public class CurveBoundedSurface : Surface
	{
		private Surface surface1Field;

		private Comos.Proteus.OuterBoundary outerBoundaryField;

		private Comos.Proteus.InnerBoundary[] innerBoundaryField;

		[XmlElement("InnerBoundary")]
		public Comos.Proteus.InnerBoundary[] InnerBoundary
		{
			get
			{
				return this.innerBoundaryField;
			}
			set
			{
				this.innerBoundaryField = value;
			}
		}

		public Comos.Proteus.OuterBoundary OuterBoundary
		{
			get
			{
				return this.outerBoundaryField;
			}
			set
			{
				this.outerBoundaryField = value;
			}
		}

		[XmlElement("Surface")]
		public Surface Surface1
		{
			get
			{
				return this.surface1Field;
			}
			set
			{
				this.surface1Field = value;
			}
		}

		public CurveBoundedSurface()
		{
		}
	}
}