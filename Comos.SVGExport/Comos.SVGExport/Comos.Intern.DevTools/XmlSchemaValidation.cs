using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Comos.Intern.DevTools
{
	public class XmlSchemaValidation
	{
		private ArrayList _aryValidationEvents;

		public ArrayList ValidationEvents
		{
			get
			{
				return this._aryValidationEvents;
			}
		}

		public string XmlFileName
		{
			get;
			set;
		}

		public string XmlSchemaFileName
		{
			get;
			set;
		}

		public XmlSchemaValidation()
		{
			this._aryValidationEvents = new ArrayList();
		}

		private ValidationReturnValues _validateFile()
		{
			ValidationReturnValues validationReturnValue;
			try
			{
				this._aryValidationEvents.Clear();
				if (!File.Exists(this.XmlFileName))
				{
					validationReturnValue = ValidationReturnValues.XmlFileNotFound;
				}
				else if (File.Exists(this.XmlSchemaFileName))
				{
					XmlReaderSettings xmlReaderSetting = new XmlReaderSettings();
					xmlReaderSetting.Schemas.Add(null, this.XmlSchemaFileName);
					xmlReaderSetting.ValidationType = ValidationType.Schema;
					xmlReaderSetting.ValidationEventHandler += new ValidationEventHandler(this.schemaSettingsValidationEventHandler);
					XmlReader xmlReader = XmlReader.Create(this.XmlFileName, xmlReaderSetting);
					while (xmlReader.Read())
					{
					}
					validationReturnValue = ValidationReturnValues.NoExceptions;
				}
				else
				{
					validationReturnValue = ValidationReturnValues.SchemaFileNotfound;
				}
			}
			catch (Exception exception)
			{
				validationReturnValue = ValidationReturnValues.UnknownError;
			}
			return validationReturnValue;
		}

		private void schemaSettingsValidationEventHandler(object sender, ValidationEventArgs e)
		{
			this._aryValidationEvents.Add(e);
			this.ThrowSchemaValidationEvent(e);
		}

		protected void ThrowSchemaValidationEvent(ValidationEventArgs ea)
		{
			if (this.SchemaValidationEvent != null)
			{
				this.SchemaValidationEvent(this, ea);
			}
		}

		public ValidationReturnValues ValidateFile()
		{
			return this._validateFile();
		}

		public event SchemaValidationEventDelegate SchemaValidationEvent;
	}
}