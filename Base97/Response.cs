using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Nls.Base97.EnumResponses;

namespace Nls.Base97 {
	public class Response {
		#region Fields
		private readonly ImportDataSet _dsImport;
		private readonly LinksDataSet _dsLinks;
		#endregion
		#region Constructor
        public Response( ImportDataSet dsImport, LinksDataSet dsLinks ) {
            if( dsImport == null ) throw new ArgumentNullException("dsImport");
            if( dsLinks == null ) throw new ArgumentNullException("dsLinks");
            if( dsLinks.tblResponse.Count != 0 ) throw new InvalidOperationException("tblResponse must be empty before creating rows for it.");
            if( dsImport.tblRoster.Count == 0 ) throw new InvalidOperationException("tblRoster must NOT be empty before reading responses from it.");
            if( dsImport.tblLinksExplicit.Count == 0 ) throw new InvalidOperationException("tblLinksExplicit must NOT be empty before reading responses from it.");
            if( dsImport.tblLinksImplicit.Count == 0 ) throw new InvalidOperationException("tblLinksImplicit must NOT be empty before reading responses from it.");
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
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Demographics, false, Constants.PassoverResponses, _dsImport.tblDemographics);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.Roster, false, Constants.PassoverResponses, _dsImport.tblRoster);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.SurveyTime, false, Constants.PassoverResponses, _dsImport.tblSurveyTime);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.LinksExplicit, false, Constants.PassoverResponses, _dsImport.tblLinksExplicit);
            reponseRecordsAddedCount += TranslateExtractSource(ExtractSource.LinksImplicit, false, Constants.PassoverResponses, _dsImport.tblLinksImplicit);
			sw.Stop();
			return string.Format("{0:N0} response records were translated.\nElapsed time: {1}", reponseRecordsAddedCount, sw.Elapsed.ToString());
		}
		private Int32 TranslateExtractSource ( ExtractSource extractSource,  bool femalesOnly, Int32[] passoverValues, DataTable dtImport ) {
			Int32 gen1ReponseRecordsAddedCount = 0;
			LinksDataSet.tblVariableRow[] drsVariablesToTranslate = VariablesToTranslate(extractSource);
			_dsLinks.tblResponse.BeginLoadData();

			string subjectIDColumnName = Constants.SubjectIDColumn;
			string genderColumnName = Constants.GenderColumn;


			foreach ( DataRow drImport in dtImport.Rows ) {
				Gender gender = (Gender)Convert.ToInt32(drImport[genderColumnName]);
				if ( !femalesOnly || gender == Gender.Female ) {
					Int32 subjectID = Convert.ToInt32(drImport[subjectIDColumnName]);
                    Int32 subjectTag = subjectID;
					foreach ( LinksDataSet.tblVariableRow drVariable in drsVariablesToTranslate ) {
						string columnName = drVariable.VariableCode;
						LinksDataSet.tblResponseRow drResponse = _dsLinks.tblResponse.NewtblResponseRow();
						drResponse.SubjectTag = subjectTag;
						drResponse.ExtendedID = GetExtendedID(subjectTag);
						drResponse.SurveyYear = drVariable.SurveyYear;
						drResponse.Item = drVariable.Item; //if ( drResponse.Item == 13 ) Trace.Assert(true);
						drResponse.Value = Convert.ToInt32(drImport[columnName]);

						LinksDataSet.tblItemRow drItem = drVariable.tblItemRow;
						if ( !(drItem.MinValue <= drResponse.Value && drResponse.Value <= drItem.MaxValue) )
							throw new InvalidOperationException(string.Format("For Item '{0}', variable '{1}', the value '{2}' exceeded the bounds of [{3}, {4}].", drVariable.Item, drVariable.VariableCode, drResponse.Value, drItem.MinValue, drItem.MaxValue));
						if ( 0 <= drResponse.Value && drResponse.Value < drItem.MinNonnegative )
                            throw new InvalidOperationException(string.Format("For Item '{0}', variable '{1}', the value '{2}' dipped below the minimum nonnegative value of {3}.", drVariable.Item, drVariable.VariableCode, drResponse.Value, drItem.MinNonnegative));
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
                case ExtractSource.Demographics: return _dsImport.tblDemographics.TableName;
                case ExtractSource.Roster: return _dsImport.tblRoster.TableName;
                case ExtractSource.SurveyTime: return _dsImport.tblSurveyTime.TableName;
				case ExtractSource.LinksExplicit: return _dsImport.tblLinksExplicit.TableName;
				case ExtractSource.LinksImplicit: return _dsImport.tblLinksImplicit.TableName;

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
