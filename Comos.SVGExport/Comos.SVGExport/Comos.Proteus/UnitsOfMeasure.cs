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
	public class UnitsOfMeasure
	{
		private AreaUnitsType areaField;

		private AngleUnitsType angleField;

		private LengthUnitsType distanceField;

		private TemperatureUnitsType temperatureField;

		private PressureUnitsType pressureField;

		private VolumeUnitsType volumeField;

		private MassUnitsType weightField;

		[DefaultValue(AngleUnitsType.Degreeangle)]
		[XmlAttribute]
		public AngleUnitsType Angle
		{
			get
			{
				return this.angleField;
			}
			set
			{
				this.angleField = value;
			}
		}

		[DefaultValue(AreaUnitsType.MetreSquared)]
		[XmlAttribute]
		public AreaUnitsType Area
		{
			get
			{
				return this.areaField;
			}
			set
			{
				this.areaField = value;
			}
		}

		[DefaultValue(LengthUnitsType.Millimetre)]
		[XmlAttribute]
		public LengthUnitsType Distance
		{
			get
			{
				return this.distanceField;
			}
			set
			{
				this.distanceField = value;
			}
		}

		[DefaultValue(PressureUnitsType.Bar)]
		[XmlAttribute]
		public PressureUnitsType Pressure
		{
			get
			{
				return this.pressureField;
			}
			set
			{
				this.pressureField = value;
			}
		}

		[DefaultValue(TemperatureUnitsType.DegreeCelsius)]
		[XmlAttribute]
		public TemperatureUnitsType Temperature
		{
			get
			{
				return this.temperatureField;
			}
			set
			{
				this.temperatureField = value;
			}
		}

		[DefaultValue(VolumeUnitsType.MetreCubed)]
		[XmlAttribute]
		public VolumeUnitsType Volume
		{
			get
			{
				return this.volumeField;
			}
			set
			{
				this.volumeField = value;
			}
		}

		[DefaultValue(MassUnitsType.Kilogram)]
		[XmlAttribute]
		public MassUnitsType Weight
		{
			get
			{
				return this.weightField;
			}
			set
			{
				this.weightField = value;
			}
		}

		public UnitsOfMeasure()
		{
			this.areaField = AreaUnitsType.MetreSquared;
			this.angleField = AngleUnitsType.Degreeangle;
			this.distanceField = LengthUnitsType.Millimetre;
			this.temperatureField = TemperatureUnitsType.DegreeCelsius;
			this.pressureField = PressureUnitsType.Bar;
			this.volumeField = VolumeUnitsType.MetreCubed;
			this.weightField = MassUnitsType.Kilogram;
		}
	}
}