using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nls.BaseAssembly.EnumResponses;

namespace Nls.BaseAssembly {
	public class Response {
		#region Fields
		private readonly ImportDataSet _dsImport;
		private readonly LinksDataSet _dsLinks;
		#endregion
		#region Constructor
		public Response ( ImportDataSet dsImport, LinksDataSet dsLinks ) {
			if ( dsImport == null ) throw new ArgumentNullException("dsImport");
			if ( dsLinks == null ) throw new ArgumentNullException("dsLinks");
			if ( dsLinks.tblResponse.Count != 0 ) throw new InvalidOperationException("tblResponse must be empty before creating rows for it.");
			if ( dsImport.tblGen1Links.Count == 0 ) throw new InvalidOperationException("tblGen1Links must NOT be empty before reading responses from it.");
			if ( dsImport.tblGen2Links.Count == 0 ) throw new InvalidOperationException("tblGen2Links must NOT be empty before reading responses from it.");
			if ( dsImport.tblGen2LinksFromGen1.Count == 0 ) throw new InvalidOperationException("tblGen2LinksFromGen1 must NOT be empty before reading responses from it.");
			if ( dsImport.tblGen2ImplicitFather.Count == 0 ) throw new InvalidOperationException("tblGen2ImplicitFather must NOT be empty before reading responses from it.");
            if( dsImport.tblGen1Outcomes.Count == 0 ) throw new InvalidOperationException("tblGen1Outcomes must NOT be empty before reading responses from it.");
            if( dsImport.tblGen2OutcomesHeight.Count == 0 ) throw new InvalidOperationException("tblGen2OutcomesHeight must NOT be empty before reading responses from it.");
            if( dsImport.tblGen2OutcomesWeight.Count == 0 ) throw new InvalidOperationException("tblGen2OutcomesWeight must NOT be empty before reading responses from it.");
            if( dsImport.tblGen2OutcomesMath.Count == 0 ) throw new InvalidOperationException("tblGen2OutcomesMath must NOT be empty before reading responses from it.");
			_dsImport = dsImport;
			_dsLinks = dsLinks;
		}
		#endregion
		#region Public Methods
		public string Go ( ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			CheckVariableExistInImportedDataTables();
			Int32 reponseRecordsAddedCount = 0;
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen1Links, Generation.Gen1, false, Constants.Gen1PassoverResponses, _dsImport.tblGen1Links);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen1Explicit, Generation.Gen1, false, Constants.Gen1PassoverResponses, _dsImport.tblGen1Explicit);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen1Implicit, Generation.Gen1, false, Constants.Gen1PassoverResponses, _dsImport.tblGen1Implicit);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2Links, Generation.Gen2, false, Constants.Gen2PassoverResponses, _dsImport.tblGen2Links);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2LinksFromGen1, Generation.Gen1, false, Constants.Gen1PassoverResponses, _dsImport.tblGen2LinksFromGen1);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2ImplicitFather, Generation.Gen2, false, Constants.Gen2PassoverResponses, _dsImport.tblGen2ImplicitFather);
			reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2FatherFromGen1, Generation.Gen1, true, Constants.Gen1PassoverResponses, _dsImport.tblGen2FatherFromGen1);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen1Outcomes, Generation.Gen1, false, Constants.Gen1PassoverResponsesNoNegatives, _dsImport.tblGen1Outcomes);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2OutcomesHeight, Generation.Gen2, false, Constants.Gen2PassoverResponseNoNegatives, _dsImport.tblGen2OutcomesHeight);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2OutcomesWeight, Generation.Gen2, false, Constants.Gen2PassoverResponseNoNegatives, _dsImport.tblGen2OutcomesWeight);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Gen2OutcomesMath, Generation.Gen2, false, Constants.Gen2PassoverResponseNoNegatives, _dsImport.tblGen2OutcomesMath);
			sw.Stop();
			return string.Format("{0:N0} response records were translated.\nElapsed time: {1}", reponseRecordsAddedCount, sw.Elapsed.ToString());
		}
		private Int32 TranslateExtractSource ( ExtractSource extractSource, Generation generation, bool femalesOnly, Int32[] passoverValues, DataTable dtImport ) {
			Int32 gen1ReponseRecordsAddedCount = 0;
			LinksDataSet.tblVariableRow[] drsVariablesToTranslate = VariablesToTranslate(extractSource);
			_dsLinks.tblResponse.BeginLoadData();

			string subjectIDColumnName;
			string genderColumnName;
			switch ( generation ) {
				case Generation.Gen1:
					subjectIDColumnName = Constants.Gen1SubjectIDColumn;
					genderColumnName = Constants.Gen1GenderColumn;
					break;
				case Generation.Gen2:
					subjectIDColumnName = Constants.Gen2SubjectIDColumn;
					genderColumnName = Constants.Gen2GenderColumn;
					break;
				default:
					throw new ArgumentOutOfRangeException("generation", generation, "The generation value was not recognized.");
			}

			foreach ( DataRow drImport in dtImport.Rows ) {
				GenderBothGenerations gender = (GenderBothGenerations)Convert.ToInt32(drImport[genderColumnName]);
				if ( !femalesOnly || gender == GenderBothGenerations.Female ) {
					Int32 subjectID = Convert.ToInt32(drImport[subjectIDColumnName]);
					Int32 subjectTag = Retrieve.SubjectTagFromSubjectIDAndGeneration(subjectID, generation, _dsLinks);
					foreach ( LinksDataSet.tblVariableRow drVariable in drsVariablesToTranslate ) {
						string columnName = drVariable.VariableCode;
						LinksDataSet.tblResponseRow drResponse = _dsLinks.tblResponse.NewtblResponseRow();
						drResponse.Generation = (byte)generation;
						drResponse.SubjectTag = subjectTag;
						drResponse.ExtendedID = GetExtendedID(subjectTag);
						drResponse.SurveySource = drVariable.SurveySource;
						drResponse.SurveyYear = drVariable.SurveyYear;
						drResponse.Item = drVariable.Item; //if ( drResponse.Item == 13 ) Trace.Assert(true);
						drResponse.Value = Convert.ToInt32(drImport[columnName]);

						LinksDataSet.tblItemRow drItem = drVariable.tblItemRow;
						if ( !(drItem.MinValue <= drResponse.Value && drResponse.Value <= drItem.MaxValue) )
							throw new InvalidOperationException(string.Format("For Item '{0}', variable '{1}', the value '{2}' exceeded the bounds of [{3}, {4}].", drVariable.Item, drVariable.ID, drResponse.Value, drItem.MinValue, drItem.MaxValue));
						if ( 0 <= drResponse.Value && drResponse.Value < drItem.MinNonnegative )
							throw new InvalidOperationException(string.Format("For Item '{0}', variable '{1}', the value '{2}' dipped below the minimum nonnegative value of {3}.", drVariable.Item, drVariable.ID, drResponse.Value, drItem.MinNonnegative));
						if ( !passoverValues.Contains(drResponse.Value) ) {
							drResponse.LoopIndex = drVariable.LoopIndex;
							_dsLinks.tblResponse.AddtblResponseRow(drResponse);
							gen1ReponseRecordsAddedCount += 1;
						}
					}
				}
			}
			_dsLinks.tblResponse.EndLoadData();
			return gen1ReponseRecordsAddedCount;
		}
		#endregion
		#region Private Methods
		private Int16 GetExtendedID ( Int32 subjectTag ) {
			return _dsLinks.tblSubject.FindBySubjectTag(subjectTag).ExtendedID;
		}
		private void CheckVariableExistInImportedDataTables ( ) {
			foreach ( LinksDataSet.tblVariableRow drVariable in _dsLinks.tblVariable ) {
				if ( drVariable.Translate ) {
					string tableName = ConverExtractSourceToTableName((ExtractSource)drVariable.ExtractSource);
					AssertColumnExistsInImportTable(tableName, drVariable.VariableCode);
				}
			}
		}
		private string ConverExtractSourceToTableName ( ExtractSource extractSource ) {
			switch ( extractSource ) {
				case ExtractSource.Gen1Links: return _dsImport.tblGen1Links.TableName;
				case ExtractSource.Gen1Explicit: return _dsImport.tblGen1Explicit.TableName;
				case ExtractSource.Gen1Implicit: return _dsImport.tblGen1Implicit.TableName;
				case ExtractSource.Gen2Links: return _dsImport.tblGen2Links.TableName;
				case ExtractSource.Gen2LinksFromGen1: return _dsImport.tblGen2LinksFromGen1.TableName;
				case ExtractSource.Gen2ImplicitFather: return _dsImport.tblGen2ImplicitFather.TableName;
				case ExtractSource.Gen2FatherFromGen1: return _dsImport.tblGen2FatherFromGen1.TableName;
                case ExtractSource.Gen1Outcomes: return _dsImport.tblGen1Outcomes.TableName;
                case ExtractSource.Gen2OutcomesHeight: return _dsImport.tblGen2OutcomesHeight.TableName;
                case ExtractSource.Gen2OutcomesWeight: return _dsImport.tblGen2OutcomesWeight.TableName;
                case ExtractSource.Gen2OutcomesMath: return _dsImport.tblGen2OutcomesMath.TableName;
				default: throw new ArgumentOutOfRangeException("extractSource", extractSource, "The Extract Source is not recognized in this function.");
			}
		}
		private void AssertColumnExistsInImportTable ( string tableName, string columnName ) {
			Int32 index = _dsImport.Tables[tableName].Columns.IndexOf(columnName);
			if ( index < 0 ) throw new InvalidOperationException("The column '" + columnName + "' doesn't exist in " + tableName + ".");
		}
		private LinksDataSet.tblVariableRow[] VariablesToTranslate ( ExtractSource extractSource ) {
			string select = string.Format("{0}={1} AND {2}={3}",
				(byte)extractSource, _dsLinks.tblVariable.ExtractSourceColumn.ColumnName,
				"TRUE", _dsLinks.tblVariable.TranslateColumn.ColumnName);
			LinksDataSet.tblVariableRow[] drVariablesToTranslate = (LinksDataSet.tblVariableRow[])_dsLinks.tblVariable.Select(select);
			return drVariablesToTranslate;
		}
		#endregion
	}
}
