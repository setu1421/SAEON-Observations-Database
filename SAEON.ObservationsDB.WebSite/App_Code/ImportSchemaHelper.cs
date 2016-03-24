using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Observations.Data;
using FileHelpers;
using FileHelpers.Dynamic;
using System.IO;
using System.Data;
using SubSonic;
using System.Globalization;
using Evaluant.Calculator;
using System.Text;

/// <summary>
/// Summary description for ImportSchema
/// </summary>
public class ImportSchemaHelper : IDisposable
{
	FileHelperEngine engine;
	DataTable dtResults;
	DataSource dataSource;
	List<DataSourceTransformation> transformations;
	Type recordType;
	List<SchemaDefinition> schemaDefs;
	public List<SchemaValue> SchemaValues;

	SensorProcedure Sensor = null;

	/// <summary>
	/// Gap Record Helper
	/// </summary>
	ImportLogHelper LogHelper = null;

	//public SchemaValues

	Boolean concatedatetime = false;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="schema"></param>
	/// <param name="InputStream"></param>
	public ImportSchemaHelper(DataSource ds, DataSchema schema, string Data, SensorProcedure obj = null, ImportLogHelper loghelper = null)
	{
		dataSource = ds;

		recordType = ClassBuilder.LoadFromXmlString(schema.DataSchemaX).CreateRecordClass();
		engine = new FileHelperEngine(recordType);
		engine.ErrorMode = ErrorMode.SaveAndContinue;


		dtResults = engine.ReadStringAsDT(Data);
		transformations = new List<DataSourceTransformation>();
		schemaDefs = new List<SchemaDefinition>();

		SchemaValues = new List<SchemaValue>();

		Sensor = obj;

		LogHelper = loghelper;
	}

	/// <summary>
	/// 
	/// </summary>
	void BuildSchemaDefinition()
	{

		for (int i = 0; i < dtResults.Columns.Count; i++)
		{
			DataColumn dtcol = dtResults.Columns[i];

			SchemaDefinition def = new SchemaDefinition();

			def.Index = i;
			def.FieldName = dtcol.ColumnName;

			if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(ValueDateAttribute), true))
			{
				ValueDateAttribute att = (ValueDateAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(ValueDateAttribute), true)[0];
				def.IsDate = true;
				def.Dateformat = att.Format;
			}

			if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(ValueTimeAttribute), true))
			{
				ValueTimeAttribute att = (ValueTimeAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(ValueTimeAttribute), true)[0];
				def.IsTime = true;
				def.Timeformat = att.Format;
			}

			if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(FieldValueDiscardedAttribute), true))
				def.IsIgnored = true;

			if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(ValueCommentAttribute), true))
				def.IsComment = true;

			if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(PhenomenonOfferingAttribute), true))
			{
				PhenomenonOfferingAttribute att = (PhenomenonOfferingAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(PhenomenonOfferingAttribute), true)[0];
				def.PhenomenonOfferingID = att.PhenomenonOfferingId;

				def.IsOffering = true;

				PhenomenonOffering off = new PhenomenonOffering(def.PhenomenonOfferingID);
				if (off == null)
					def.InValidOffering = true;
				else
					def.DataSourceTransformationIDs = LoadTransformations(att.PhenomenonOfferingId);


				if (Sensor == null)
				{
					SensorProcedureCollection colsens = new Select()
														  .From(SensorProcedure.Schema)
														  .Where(SensorProcedure.PhenomenonIDColumn).IsEqualTo(off.PhenomenonID)
														  .And(SensorProcedure.DataSourceIDColumn).IsEqualTo(this.dataSource.Id)
														  .ExecuteAsCollection<SensorProcedureCollection>();

					if (colsens.Count() == 0)
						def.SensorProcedureNotFound = true;
					else
						def.SensorProcedureID = colsens[0].Id;
				}
				else
					def.SensorProcedureID = Sensor.Id;


				if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(PhenomenonUOMAttribute), true))
				{
					PhenomenonUOMAttribute attuom = (PhenomenonUOMAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(PhenomenonUOMAttribute), true)[0];
					def.PhenomenonUOMID = attuom.phenomenonUOMId;

					PhenomenonUOM uom = new PhenomenonUOM(def.PhenomenonUOMID);
					if (uom == null)
						def.InValidUOM = true;
				}

				if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(ValueEmptyAttribute), true))
				{
					ValueEmptyAttribute attemp = (ValueEmptyAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(ValueEmptyAttribute), true)[0];
					def.IsEmptyValue = true;
					def.EmptyValue = attemp.EmptyValue;
				}

				if (recordType.GetField(dtcol.ColumnName).IsDefined(typeof(FixedTimeAttribute), true))
				{
					FixedTimeAttribute fatt = (FixedTimeAttribute)recordType.GetField(dtcol.ColumnName).GetCustomAttributes(typeof(FixedTimeAttribute), true)[0];
					def.FixedTimeValue = TimeSpan.Parse(fatt.timeSpan);
					def.IsFixedTime = true;
				}
			}

			schemaDefs.Add(def);
		}

		if (schemaDefs.FirstOrDefault(t => t.IsDate) != null && (schemaDefs.FirstOrDefault(t => t.IsTime) != null) || (schemaDefs.FirstOrDefault(t => t.IsFixedTime) != null))
			concatedatetime = true;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	List<Guid> LoadTransformations(Guid offId)
	{
		List<Guid> transforms = new List<Guid>();

		DataSourceTransformationCollection col = new Select()
												.From(DataSourceTransformation.Schema)
												.InnerJoin(Phenomenon.IdColumn, DataSourceTransformation.PhenomenonIDColumn)
												.InnerJoin(PhenomenonOffering.PhenomenonIDColumn, Phenomenon.IdColumn)
												.InnerJoin(TransformationType.IdColumn, DataSourceTransformation.TransformationTypeIDColumn)
												.Where(PhenomenonOffering.IdColumn).IsEqualTo(offId)
												.And(DataSourceTransformation.DataSourceIDColumn).IsEqualTo(this.dataSource.Id)
												.And(DataSourceTransformation.StartDateColumn).IsLessThanOrEqualTo(DateTime.Now.Date)
												.AndExpression(DataSourceTransformation.Columns.EndDate).IsNull()
													.Or(DataSourceTransformation.EndDateColumn).IsGreaterThanOrEqualTo(DateTime.Now)
												.CloseExpression()
												.OrderAsc(TransformationType.IorderColumn.QualifiedName)
												.ExecuteAsCollection<DataSourceTransformationCollection>();

		foreach (var item in col)
		{
			if (transformations.FirstOrDefault(t => t.Id == item.Id) == null)
			{
				transformations.Add(item);
			}

			transforms.Add(item.Id);
		}

		return transforms;
	}

	/// <summary>
	/// 
	/// </summary>
	public void ProcessSchema()
	{
		BuildSchemaDefinition();

		for (int i = 0; i < dtResults.Rows.Count; i++)
		{
			DataRow dr = dtResults.Rows[i];
			ProcessRow(dr);
		}

		//Add

		if (this.LogHelper != null)
		{
			List<SchemaDefinition> defs = schemaDefs.Where(t => t.IsOffering).ToList();
			List<DateTime> unprocesseddt = LogHelper.GetUnprocessedDates();


			try
			{
				for (int b = 0; b < unprocesseddt.Count; b++)
				{
					for (int i = 0; i < defs.Count; i++)
					{
						SchemaDefinition def = defs[i];

						SchemaValue rec = new SchemaValue();

						rec.DateValue = unprocesseddt[b];
						rec.SensorProcedureNotFound = def.SensorProcedureNotFound;
						rec.SensorProcedureID = def.SensorProcedureID;

						if (rec.SensorProcedureNotFound)
							rec.InvalidStatuses.Add(Status.SensorProcedureNotFound);

						rec.InValidOffering = def.InValidOffering;
						rec.PhenomenonOfferingID = def.PhenomenonOfferingID;

						if (rec.InValidOffering)
							rec.InvalidStatuses.Add(Status.OfferingInvalid);

						rec.InValidUOM = def.InValidUOM;
						rec.PhenomenonUOMID = def.PhenomenonUOMID;
						if (rec.InValidUOM)
							rec.InvalidStatuses.Add(Status.UOMInvalid);

						rec.RawValue = null;
						rec.DataValue = null;


						SchemaValues.Add(rec);
					}
				}
			}
			catch (Exception Ex)
			{
				throw Ex;
			}

		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dr"></param>
	void ProcessRow(DataRow dr)
	{
		DateTime dttme = DateTime.MinValue,
					dt = DateTime.MinValue,
					tm = DateTime.MinValue;
		Boolean ErrorInDate = false;
		Boolean ErrorInTime = false;

		string InvalidDateValue = String.Empty,
			   InvalidTimeValue = String.Empty,
			   RowComment = String.Empty;


		SchemaDefinition dtdef = schemaDefs.FirstOrDefault(t => t.IsDate);
		SchemaDefinition tmdef = schemaDefs.FirstOrDefault(t => t.IsTime);


		if (tmdef == null)
			tmdef = schemaDefs.FirstOrDefault(t => t.IsFixedTime);

		if (dtdef != null)
		{
			string sDateValue = dr[dtdef.Index].ToString();
			if (String.IsNullOrEmpty(sDateValue) || !DateTime.TryParseExact(sDateValue.ToUpper().Trim(), dtdef.Dateformat, null, DateTimeStyles.None, out dt))
			{
				ErrorInDate = true;
				InvalidDateValue = sDateValue;
			}
		}

		if (tmdef != null)
		{
			if (tmdef.IsTime)
			{
				string sTimeValue = dr[tmdef.Index].ToString();
				if (String.IsNullOrEmpty(sTimeValue) || !DateTime.TryParseExact(sTimeValue.ToUpper().Trim(), tmdef.Timeformat, null, DateTimeStyles.None, out tm))
				{
					ErrorInTime = true;
					InvalidTimeValue = sTimeValue;
				}
			}
			else if (tmdef.IsFixedTime)
				tm = DateTime.Now.Date.AddMilliseconds(tmdef.FixedTimeValue.TotalMilliseconds);
		}

		if (concatedatetime &&
		   !ErrorInDate &&
		   !ErrorInTime)
		{
			dttme = dt.Date.AddMilliseconds(tm.TimeOfDay.TotalMilliseconds);
		}

		//Add Row Comment
		foreach (var df in schemaDefs.Where(t => t.IsComment))
		{
			RowComment += dr[df.Index].ToString();
		}

		for (int i = 0; i < schemaDefs.Count; i++)
		{
			SchemaDefinition def = schemaDefs[i];

			if (def.IsOffering)
			{
				SchemaValue rec = new SchemaValue();

				rec.SensorProcedureNotFound = def.SensorProcedureNotFound;
				rec.SensorProcedureID = def.SensorProcedureID;

				if (rec.SensorProcedureNotFound)
					rec.InvalidStatuses.Add(Status.SensorProcedureNotFound);

				rec.InValidOffering = def.InValidOffering;
				rec.PhenomenonOfferingID = def.PhenomenonOfferingID;

				if (rec.InValidOffering)
					rec.InvalidStatuses.Add(Status.OfferingInvalid);

				rec.InValidUOM = def.InValidUOM;
				rec.PhenomenonUOMID = def.PhenomenonUOMID;
				if (rec.InValidUOM)
					rec.InvalidStatuses.Add(Status.UOMInvalid);


				if (ErrorInTime)
				{
					rec.TimeValueInvalid = true;
					rec.InvalidTimeValue = InvalidTimeValue;
					rec.InvalidStatuses.Add(Status.TimeInvalid);
				}

				if (ErrorInDate)
				{
					rec.DateValueInvalid = true;
					rec.InvalidDateValue = InvalidDateValue;
					rec.InvalidStatuses.Add(Status.DateInvalid);

					//Make Time visible on input
					rec.TimeValueInvalid = true;
					rec.InvalidTimeValue = "";
				}

				if (concatedatetime && !ErrorInDate && !ErrorInTime)
					rec.DateValue = dttme;
				else if (!ErrorInDate)
					rec.DateValue = dt;

				if (!ErrorInTime)
					rec.TimeValue = tm;

				string RawValue = dr[def.Index].ToString();


				if (!ErrorInDate && LogHelper != null && LogHelper.CheckRecordGap(rec.DateValue))
				{
					rec.RawValue = null;
					rec.DataValue = null;
				}
				else if (String.IsNullOrEmpty(RawValue) || def.IsEmptyValue && RawValue.Trim() == def.EmptyValue)
				{
					rec.FieldRawValue = RawValue;
					rec.RawValue = null;// dataSource.DefaultNullValue;
					rec.DataValue = null;// dataSource.DefaultNullValue;

					foreach (var dtid in def.DataSourceTransformationIDs)
					{
						//TransformValue(dtid, ref rec);
						//
						DataSourceTransformation dst = new DataSourceTransformation(dtid.ToString());
						if (dst.NewPhenomenonOfferingID != null)
						{
							rec.PhenomenonOfferingID = dst.NewPhenomenonOfferingID;
						}
						if (dst.NewPhenomenonUOMID != null)
						{
							rec.PhenomenonUOMID = dst.NewPhenomenonUOMID;
						}
						//
					}
				}
				else
				{
					rec.FieldRawValue = RawValue;
					Double dvalue = -1;

					if (!Double.TryParse(RawValue, out dvalue))
					{
						rec.RawValueInvalid = true;
						rec.InvalidRawValue = RawValue;
						rec.InvalidStatuses.Add(Status.ValueInvalid);
					}
					else
					{
						rec.RawValue = dvalue;

						if (def.DataSourceTransformationIDs.Count > 0)
						{
							foreach (var dtid in def.DataSourceTransformationIDs)
							{
								TransformValue(dtid, ref rec);
								//
								
								//
							}
						}
						else
							rec.DataValue = rec.RawValue;
					}
				}

				if (RowComment.Length > 0)
					rec.Comment = RowComment;

				SchemaValues.Add(rec);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void TransformValue(Guid dtid, ref SchemaValue rec)
	{
		bool valid = true;
		var trns = transformations.FirstOrDefault(t => t.Id == dtid);

		bool process = trns.PhenomenonOfferingID.HasValue ? trns.PhenomenonOfferingID.Value == rec.PhenomenonOfferingID : true &&
				trns.PhenomenonUOMID.HasValue ? trns.PhenomenonUOMID.Value == rec.PhenomenonUOMID : true;

		if (trns.TransformationTypeID.ToString() == TransformationType.CorrectionValues )
		{
			if (process)
			{
				Dictionary<string, string> corrvals = trns.CorrectionValues;

				if (corrvals.ContainsKey("eq"))
				{
					//string eq = corrvals["equation"].Replace("\"","").Replace("\"","");
					Expression exp = new Expression(corrvals["eq"]);
					exp.Parameters["value"] = rec.RawValue;
					object val = exp.Evaluate();
					rec.DataValue = Double.Parse(val.ToString());

					DataSourceTransformation dst = new DataSourceTransformation(dtid.ToString());
					if (dst.NewPhenomenonOfferingID != null)
					{
						rec.PhenomenonOfferingID = dst.NewPhenomenonOfferingID;
					}
					if (dst.NewPhenomenonUOMID != null)
					{
						rec.PhenomenonUOMID = dst.NewPhenomenonUOMID;
					}
				} 
			}
			else
			{
				rec.DataValue = rec.RawValue;
			}
		}
		else if (trns.TransformationTypeID.ToString() == TransformationType.RatingTable && process)
		{

			try
			{
				//List<double> vals = trns.RatingTableValues.ToList();

				//var index = vals.BinarySearch(rec.RawValue.Value);

				//if (index < 0)
				//{
				//    index = ~index;
				//    index -= 1;
				//}

				//var result = vals[index];

				//if (index != vals.Count - 1 && rec.RawValue.Value != result)
				//{
				//    var floor = result;
				//    var ceiling = vals[vals.Count - 1];

				//    if ((rec.RawValue - floor) > (ceiling - rec.RawValue))
				//        rec.DataValue = ceiling;
				//    else
				//        rec.DataValue = floor;
				//}
				//else
				//    rec.DataValue = rec.RawValue;


				if (process)
				{
					rec.DataValue = trns.GetRatingValue(rec.RawValue.Value);

					DataSourceTransformation dst = new DataSourceTransformation(dtid.ToString());
					if (dst.NewPhenomenonOfferingID != null)
					{
						rec.PhenomenonOfferingID = dst.NewPhenomenonOfferingID;
					}
					if (dst.NewPhenomenonUOMID != null)
					{
						rec.PhenomenonUOMID = dst.NewPhenomenonUOMID;
					}
				}
				else
				{
					rec.DataValue = rec.RawValue;
				}

			}
			catch (Exception Ex)
			{
				rec.DataValue = rec.RawValue;
			}

		}

		else if (trns.TransformationTypeID.ToString() == TransformationType.QualityControlValues)
		{

			if (!rec.DataValue.HasValue)
				rec.DataValue = rec.RawValue;

			Dictionary<string, Double> qv = trns.QualityValues;
			if (qv.ContainsKey("min") && rec.DataValue.Value < qv["min"])
				valid = false;

			if (qv.ContainsKey("max") && rec.DataValue.Value > qv["max"])
				valid = false;

			if (!valid)
			{
				rec.InvalidStatuses.Add(Status.TransformValueInvalid);
				rec.DataSourceTransformationID = trns.Id;

				rec.DataValueInvalid = true;
				rec.InvalidDataValue = rec.DataValue.ToString();
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public List<object> Errors
	{
		get
		{
			List<object> _errors = new List<object>();

			foreach (ErrorInfo errinfo in engine.ErrorManager.Errors)
			{
				_errors.Add(new { ErrorMessage = errinfo.ExceptionInfo.Message, LineNo = errinfo.LineNumber, RecordString = errinfo.RecordString });
			}

			return _errors;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void Dispose()
	{

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="ds"></param>
	/// <param name="reader"></param>
	/// <returns></returns>
	public static string GetWorkingStream(DataSchema ds, StreamReader reader)
	{

		String Result = String.Empty;

		if (!String.IsNullOrEmpty(ds.SplitSelector))
		{
			StringBuilder sb = new StringBuilder();

			string line;

			int index = 0;

			while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
			{
				if (line.StartsWith(ds.SplitSelector))
					index++;

				if (index == ds.SplitIndex)
				{
					sb.AppendLine(line);
					break;
				}
			}

			while ((line = reader.ReadLine()) != null && index <= ds.SplitIndex)
			{
				if (line.StartsWith(ds.SplitSelector))
					index++;

				if (index <= ds.SplitIndex)
				{
					sb.AppendLine(line);
				}
			}

			Result = sb.ToString();
		}
		else
			Result = reader.ReadToEnd();

		return Result;
	}

}

public class SchemaDefinition
{

	public SchemaDefinition()
	{
		this.DataSourceTransformationIDs = new List<Guid>();
	}

	public int Index { get; set; }
	public string FieldName { get; set; }
	public Boolean IsDate { get; set; }
	public string Dateformat { get; set; }
	public Boolean IsTime { get; set; }
	public string Timeformat { get; set; }
	public bool IsFixedTime { get; set; }
	public TimeSpan FixedTimeValue { get; set; }
	public Boolean IsIgnored { get; set; }
	public Guid? PhenomenonOfferingID { get; set; }
	public Boolean InValidOffering { get; set; }
	public Guid? PhenomenonUOMID { get; set; }
	public Boolean InValidUOM { get; set; }
	public List<Guid> DataSourceTransformationIDs { get; set; }
	public Boolean IsEmptyValue { get; set; }
	public string EmptyValue { get; set; }
	public Boolean IsOffering { get; set; }

	public bool IsComment { get; set; }
	public Guid? SensorProcedureID { get; set; }
	public bool SensorProcedureNotFound { get; set; }
}

/// <summary>
/// 
/// </summary>
public class SchemaValue
{

	public SchemaValue()
	{
		this.InvalidStatuses = new List<string>();
		this.Comment = String.Empty;
	}

	public DateTime DateValue { get; set; }
	public DateTime? TimeValue { get; set; }
	public string InvalidDateValue { get; set; }
	public string InvalidTimeValue { get; set; }
	public Boolean DateValueInvalid { get; set; }
	public Boolean TimeValueInvalid { get; set; }
	public Guid? PhenomenonOfferingID { get; set; }
	public Boolean InValidOffering { get; set; }
	public Guid? PhenomenonUOMID { get; set; }
	public Boolean InValidUOM { get; set; }
	public double? RawValue { get; set; }
	public double? DataValue { get; set; }
	public Boolean RawValueInvalid { get; set; }
	public string InvalidRawValue { get; set; }
	public Boolean DataValueInvalid { get; set; }
	public string InvalidDataValue { get; set; }
	public List<string> InvalidStatuses { get; set; }
	public Guid? DataSourceTransformationID { get; set; }
	public Guid? SensorProcedureID { get; set; }
	public bool SensorProcedureNotFound { get; set; }
	public string FieldRawValue { get; set; }
	public string Comment { get; set; }


	/// <summary>
	/// 
	/// </summary>
	public bool IsValid
	{
		get
		{
			return !DateValueInvalid &&
				   !TimeValueInvalid &&
				   !InValidOffering &&
				   !InValidUOM &&
				   !RawValueInvalid &&
				   !DataValueInvalid &&
				   !SensorProcedureNotFound;
		}
	}
}

