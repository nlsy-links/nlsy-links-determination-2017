using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BA = Nls.BaseAssembly;

namespace LinksGui {
	public partial class MainWindow : Window {
		#region Fields
		private BA.ImportDataSet _dsImport;
		private BA.LinksDataSet _dsLinks;
		private SqlConnection _cnn;
		private const string _cnnStringNameInAppConfig = "LinksGui.Properties.Settings.NlsLinksConnectionString";
		private const string _combinedButtonTag = "CombinedButton";
		#endregion
		public MainWindow ( ) {
			InitializeComponent();
		}
		private void Window_Loaded ( object sender, RoutedEventArgs e ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[_cnnStringNameInAppConfig];
			Trace.Assert(settings != null, "The connectionStringSettings object should be initialized propertly.");
			_cnn = new SqlConnection(settings.ConnectionString);

			_dsImport = ((BA.ImportDataSet)(this.FindResource("importDataSet")));
			_dsLinks = ((BA.LinksDataSet)(this.FindResource("linksDataSet")));

            //if( Convert.ToBoolean("true") ) {
            if( Convert.ToBoolean("false") ) {
				LoadExtractGen1Links();
				LoadExtractGen1Explicit();
				LoadExtractGen1Implicit();
				LoadExtractGen2Links();
				LoadExtractGen2LinksFromGen1();
				LoadExtractGen2ImplicitFather();
				LoadExtractGen2FatherFromGen1();
				LoadExtractGen1Outcomes();
                LoadExtractGen2OutcomesHeight();
                LoadExtractGen2OutcomesWeight();
                LoadExtractGen2OutcomesMath();
			}

			LoadGeocodeSanitized();//Needed for MarkerGen1
			LoadLinks2004Gen1();//Needed for RelatedValues
			LoadLinks2004Gen2();//Needed for RelatedValues
			//LoadLinks2004Gen1Mz();//Needed for RelatedValues
			LoadItem();
			LoadVariable();
			LoadMzManual();
			LoadRosterAssignment();

			LoadSubject();
			LoadRelatedStructure();
			LoadResponse();
			LoadSurveyTime();
			LoadSurveyTimeMostRecent(); //Needed for tblRelated
			LoadRosterGen1();
			LoadParentsOfGen1Retro();
			LoadParentsOfGen1Current();
			LoadBabyDaddy();
			LoadFatherOfGen2();
			LoadSubjectDetails();
			LoadMarkerGen1();
			LoadMarkerGen2();
			LoadRelatedValues();
			LoadRelatedValuesNextVersionNumber();
			sw.Stop();
			string message = string.Format("DataSets loaded (Elapsed time: {0})", sw.Elapsed.ToString());
			Trace.WriteLine(message);
			Console.Beep(3000, 100);
			Console.Beep(300, 100);
			Console.Beep(3000, 100);
		}
		#region Fill & Update
		private void btnSubject_Click ( object sender, RoutedEventArgs e ) {
			string message = BA.Subject.CreateSubject(_dsImport, _dsLinks);
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblSubject);
		}
		private void btnRelatedStructure_Click ( object sender, RoutedEventArgs e ) {
			BA.RelatedStructure related = new BA.RelatedStructure(_dsLinks);
			string message = related.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblRelatedStructure);
		}
		private void btnResponse_Click ( object sender, RoutedEventArgs e ) {
			BA.Response response = new BA.Response(_dsImport, _dsLinks);
			string message = response.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblResponse);
		}
		private void btnSurveyTime_Click ( object sender, RoutedEventArgs e ) {
			BA.SurveyTime surveyTime = new BA.SurveyTime(_dsLinks);
			string message = surveyTime.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblSurveyTime);
		}
		private void btnRosterGen1_Click ( object sender, RoutedEventArgs e ) {
			BA.RosterGen1 roster = new BA.RosterGen1(_dsLinks);
			string message = roster.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblRosterGen1);
		}
		private void btnParentsOfGen1Retro_Click ( object sender, RoutedEventArgs e ) {
			BA.ParentsOfGen1Retro retro = new BA.ParentsOfGen1Retro(_dsLinks);
			string message = retro.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblParentsOfGen1Retro);
		}
		private void btnParentsOfGen1Current_Click ( object sender, RoutedEventArgs e ) {
			BA.ParentsOfGen1Current current = new BA.ParentsOfGen1Current(_dsLinks);
			string message = current.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblParentsOfGen1Current);
		}
		private void btnBabyDaddy_Click ( object sender, RoutedEventArgs e ) {
			BA.BabyDaddy babyDaddy = new BA.BabyDaddy(_dsLinks);
			string message = babyDaddy.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblBabyDaddy);
		}
		private void btnFatherOfGen2_Click ( object sender, RoutedEventArgs e ) {
			BA.FatherOfGen2 fatherOfGen2 = new BA.FatherOfGen2(_dsLinks);
			string message = fatherOfGen2.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblFatherOfGen2);
		}
		private void btnSubjectDetails_Click ( object sender, RoutedEventArgs e ) {
			BA.SubjectDetails subjectDetails = new BA.SubjectDetails(_dsLinks);
			string message = subjectDetails.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblSubjectDetails);
		}
		private void btnMarkerGen1_Click ( object sender, RoutedEventArgs e ) {
			BA.MarkerGen1 marker = new BA.MarkerGen1(_dsLinks, _dsImport);
			string message = marker.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblMarkerGen2);
		}
		private void btnMarkerGen2_Click ( object sender, RoutedEventArgs e ) {
			BA.MarkerGen2 marker = new BA.MarkerGen2(_dsLinks);
			string message = marker.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblMarkerGen2);
		}
		private void btnRelatedValues_Click ( object sender, RoutedEventArgs e ) {
			BA.RelatedValues related = new BA.RelatedValues(_dsImport, _dsLinks);
			string message = related.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblRelatedValues);
		}
		private void btnOutcome_Click ( object sender, RoutedEventArgs e ) {
			BA.Outcome outcome = new BA.Outcome(_dsLinks);
			string message = outcome.Go();
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblRelatedValues);
		}
		private void btnRelatedValuesArchive_Click ( object sender, RoutedEventArgs e ) {            
			Int16 algorithmVersion = Int16.Parse(txtAlogrithmNumber.Text);
			string message = BA.RelatedValues.Archive(algorithmVersion, _dsLinks);
			Trace.WriteLine(message);
			if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
			//WriteXml(_dsLinks.tblRelatedValuesArchive);
		}
		private void btnUpdateAllTables_Click ( object sender, RoutedEventArgs e ) {
			const string schemaName = "Process";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			//Int32 SubjectCount = _taSubject.Update(_dsLinks);
			BulkUpdate(schemaName, _dsLinks.tblSubject, LoadSubject);
			BulkUpdate(schemaName, _dsLinks.tblRelatedStructure, LoadRelatedStructure);
			//Int32 responseCount = _taResponse.Update(_dsLinks);

			BulkUpdate(schemaName, _dsLinks.tblResponse, AcceptResponseChanges);
			BulkUpdate(schemaName, _dsLinks.tblSurveyTime, LoadSurveyTime);
			BulkUpdate(schemaName, _dsLinks.tblRosterGen1, LoadRosterGen1);
			BulkUpdate(schemaName, _dsLinks.tblParentsOfGen1Retro, LoadParentsOfGen1Retro);
			BulkUpdate(schemaName, _dsLinks.tblParentsOfGen1Current, LoadParentsOfGen1Current);
			BulkUpdate(schemaName, _dsLinks.tblBabyDaddy, LoadBabyDaddy);
			BulkUpdate(schemaName, _dsLinks.tblFatherOfGen2, LoadFatherOfGen2);
			BulkUpdate(schemaName, _dsLinks.tblSubjectDetails, LoadSubjectDetails);
			BulkUpdate(schemaName, _dsLinks.tblMarkerGen1, LoadMarkerGen1);
			BulkUpdate(schemaName, _dsLinks.tblMarkerGen2, LoadMarkerGen2);
			BulkUpdate(schemaName, _dsLinks.tblRelatedValues, LoadRelatedValues);
			BulkUpdate(schemaName, _dsLinks.tblOutcome, LoadOutcomes);
			BulkUpdate(schemaName, _dsLinks.tblRelatedValuesArchive, null);

			sw.Stop();
			//string message = string.Format("The follow records were affected for each table ({0} Elapsed):\n{1:N0} tblSubject\n{2:N0} tblResponse", sw.Elapsed.ToString(), -999, -999);
			string message = string.Format("Elapsed time for BulkCopy operations: {0}", sw.Elapsed.ToString());
			Trace.WriteLine(message);
			MessageBox.Show(message);
		}
		private void BulkUpdate ( string schemaName, DataTable dt, Action loadMethod ) {
			const Int32 batchSize = 10000;
			bool hasChanges = true;
			try {
				hasChanges = dt.GetChanges() != null;
			}
			catch ( OutOfMemoryException ex ) {
				Debug.WriteLine(ex.ToString());//If this fails from, then there were changes.
				throw new Exception("There were changes to the database.", ex);
			}

			if ( hasChanges ) {
				try {
					_cnn.Open();
					SqlBulkCopy blk = new SqlBulkCopy(_cnn, SqlBulkCopyOptions.CheckConstraints, null);//Pass a null transaction.
					blk.DestinationTableName = schemaName + "." + dt.TableName;
					blk.NotifyAfter = 100;
					blk.BulkCopyTimeout = _cnn.ConnectionTimeout;
					blk.BatchSize = batchSize;
					blk.WriteToServer(dt);
					blk.Close();
					if ( loadMethod != null ) loadMethod();
				}
				catch {
					throw;
				}
				finally {
					_cnn.Close();
				}
			}//End if(dt.GetChanges() ! =null)
		}
		#endregion
		#region Load DataTables
		private void LoadExtractGen1Links ( ) {
			BA.ImportDataSetTableAdapters.tblGen1LinksTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen1LinksTableAdapter();
			ta.Fill(_dsImport.tblGen1Links);
		}
		private void LoadExtractGen1Explicit ( ) {
			BA.ImportDataSetTableAdapters.tblGen1ExplicitTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen1ExplicitTableAdapter();
			ta.Fill(_dsImport.tblGen1Explicit);
		}
		private void LoadExtractGen1Implicit ( ) {
			BA.ImportDataSetTableAdapters.tblGen1ImplicitTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen1ImplicitTableAdapter();
			ta.Fill(_dsImport.tblGen1Implicit);
		}
		private void LoadExtractGen2Links ( ) {
			BA.ImportDataSetTableAdapters.tblGen2LinksTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2LinksTableAdapter();
			ta.Fill(_dsImport.tblGen2Links);
		}
		private void LoadExtractGen2LinksFromGen1 ( ) {
			BA.ImportDataSetTableAdapters.tblGen2LinksFromGen1TableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2LinksFromGen1TableAdapter();
			ta.Fill(_dsImport.tblGen2LinksFromGen1);
		}
		private void LoadExtractGen2ImplicitFather ( ) {
			BA.ImportDataSetTableAdapters.tblGen2ImplicitFatherTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2ImplicitFatherTableAdapter();
			ta.Fill(_dsImport.tblGen2ImplicitFather);
		}
		private void LoadExtractGen2FatherFromGen1 ( ) {
			BA.ImportDataSetTableAdapters.tblGen2FatherFromGen1TableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2FatherFromGen1TableAdapter();
			ta.Fill(_dsImport.tblGen2FatherFromGen1);
		}
		private void LoadGeocodeSanitized ( ) {
			BA.ImportDataSetTableAdapters.tblGeocodeSanitizedTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGeocodeSanitizedTableAdapter();
			ta.Fill(_dsImport.tblGeocodeSanitized);
		}
		private void LoadLinks2004Gen1 ( ) {
			BA.ImportDataSetTableAdapters.tblLinks2004Gen1TableAdapter ta = new BA.ImportDataSetTableAdapters.tblLinks2004Gen1TableAdapter();
			ta.Fill(_dsImport.tblLinks2004Gen1);
		}
		private void LoadLinks2004Gen2 ( ) {
			BA.ImportDataSetTableAdapters.tblLinks2004Gen2TableAdapter ta = new BA.ImportDataSetTableAdapters.tblLinks2004Gen2TableAdapter();
			ta.Fill(_dsImport.tblLinks2004Gen2);
		}
		private void LoadExtractGen1Outcomes ( ) {
			BA.ImportDataSetTableAdapters.tblGen1OutcomesTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen1OutcomesTableAdapter();
			ta.Fill(_dsImport.tblGen1Outcomes);
		}
        private void LoadExtractGen2OutcomesHeight( ) {
            BA.ImportDataSetTableAdapters.tblGen2OutcomesHeightTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2OutcomesHeightTableAdapter();
            ta.Fill(_dsImport.tblGen2OutcomesHeight);
        }
        private void LoadExtractGen2OutcomesWeight( ) {
            BA.ImportDataSetTableAdapters.tblGen2OutcomesWeightTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2OutcomesWeightTableAdapter();
            ta.Fill(_dsImport.tblGen2OutcomesWeight);
        }
        private void LoadExtractGen2OutcomesMath( ) {
            BA.ImportDataSetTableAdapters.tblGen2OutcomesMathTableAdapter ta = new BA.ImportDataSetTableAdapters.tblGen2OutcomesMathTableAdapter();
            ta.Fill(_dsImport.tblGen2OutcomesMath);
        }
		private void LoadItem ( ) {
			BA.LinksDataSetTableAdapters.tblItemTableAdapter ta = new BA.LinksDataSetTableAdapters.tblItemTableAdapter();
			ta.Fill(_dsLinks.tblItem);
		}
		private void LoadVariable ( ) {
			BA.LinksDataSetTableAdapters.tblVariableTableAdapter ta = new BA.LinksDataSetTableAdapters.tblVariableTableAdapter();
			ta.Fill(_dsLinks.tblVariable);
		}
		private void LoadMzManual ( ) {
			BA.LinksDataSetTableAdapters.tblMzManualTableAdapter ta = new BA.LinksDataSetTableAdapters.tblMzManualTableAdapter();
			ta.Fill(_dsLinks.tblMzManual);
		}
		private void LoadRosterAssignment ( ) {
			BA.LinksDataSetTableAdapters.tblLURosterGen1AssignmentTableAdapter ta = new BA.LinksDataSetTableAdapters.tblLURosterGen1AssignmentTableAdapter();
			ta.Fill(_dsLinks.tblLURosterGen1Assignment);
		}

		///////////////////////////

		private void LoadSubject ( ) {
			BA.LinksDataSetTableAdapters.tblSubjectTableAdapter ta = new BA.LinksDataSetTableAdapters.tblSubjectTableAdapter();
			ta.Fill(_dsLinks.tblSubject);
			CollectionViewSource tblSubjectViewSource = ((CollectionViewSource)(this.FindResource("tblSubjectViewSource")));
			tblSubjectViewSource.View.MoveCurrentToFirst();
		}
		private void LoadRelatedStructure ( ) {
			BA.LinksDataSetTableAdapters.tblRelatedStructureTableAdapter ta = new BA.LinksDataSetTableAdapters.tblRelatedStructureTableAdapter();
			ta.Fill(_dsLinks.tblRelatedStructure);
			CollectionViewSource tblRelatedStructureViewSource = ((CollectionViewSource)(this.FindResource("tblRelatedStructureViewSource")));
			tblRelatedStructureViewSource.View.MoveCurrentToFirst();
		}
		private void LoadResponse ( ) {
			//BA.LinksDataSetTableAdapters.tblResponseTableAdapter _taResponse = new BA.LinksDataSetTableAdapters.tblResponseTableAdapter();
			SqlCommand cmd = new SqlCommand("Process.prcResponseRetrieveSubset", _cnn);
			cmd.CommandType = CommandType.StoredProcedure;
			//_cnn.Open();
			//SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult);
			SqlDataAdapter adapter = new SqlDataAdapter(cmd);
			adapter.Fill(_dsLinks.tblResponse);
			//_dsLinks.tblResponse.Load(reader, LoadOption.OverwriteChanges);
			//_cnn.Close();

			//_taResponse.Fill(_dsLinks.tblResponse);
			CollectionViewSource tblResponseViewSource = ((CollectionViewSource)(this.FindResource("tblResponseViewSource")));
			tblResponseViewSource.View.MoveCurrentToFirst();
		}
		private void AcceptResponseChanges ( ) {
			_dsLinks.tblResponse.AcceptChanges();
		}
		private void LoadSurveyTime ( ) {
			BA.LinksDataSetTableAdapters.tblSurveyTimeTableAdapter ta = new BA.LinksDataSetTableAdapters.tblSurveyTimeTableAdapter();
			ta.Fill(_dsLinks.tblSurveyTime);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblSurveyTimeViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadSurveyTimeMostRecent ( ) {
			BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter ta = new BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter();
			ta.Fill(_dsLinks.vewSurveyTimeMostRecent);
		}
		private void LoadRosterGen1 ( ) {
			BA.LinksDataSetTableAdapters.tblRosterGen1TableAdapter ta = new BA.LinksDataSetTableAdapters.tblRosterGen1TableAdapter();
			ta.Fill(_dsLinks.tblRosterGen1);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblRosterGen1ViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadParentsOfGen1Retro ( ) {
			BA.LinksDataSetTableAdapters.tblParentsOfGen1RetroTableAdapter ta = new BA.LinksDataSetTableAdapters.tblParentsOfGen1RetroTableAdapter();
			ta.Fill(_dsLinks.tblParentsOfGen1Retro);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblParentsOfGen1RetroViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadParentsOfGen1Current ( ) {
			BA.LinksDataSetTableAdapters.tblParentsOfGen1CurrentTableAdapter ta = new BA.LinksDataSetTableAdapters.tblParentsOfGen1CurrentTableAdapter();
			ta.Fill(_dsLinks.tblParentsOfGen1Current);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblParentsOfGen1CurrentViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadBabyDaddy ( ) {
			BA.LinksDataSetTableAdapters.tblBabyDaddyTableAdapter ta = new BA.LinksDataSetTableAdapters.tblBabyDaddyTableAdapter();
			ta.Fill(_dsLinks.tblBabyDaddy);
			CollectionViewSource tblBabyDaddyViewSource = ((CollectionViewSource)(this.FindResource("tblBabyDaddyViewSource")));
			tblBabyDaddyViewSource.View.MoveCurrentToFirst();
		}
		private void LoadFatherOfGen2 ( ) {
			BA.LinksDataSetTableAdapters.tblFatherOfGen2TableAdapter ta = new BA.LinksDataSetTableAdapters.tblFatherOfGen2TableAdapter();
			ta.Fill(_dsLinks.tblFatherOfGen2);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblFatherOfGen2ViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadSubjectDetails ( ) {
			BA.LinksDataSetTableAdapters.tblSubjectDetailsTableAdapter ta = new BA.LinksDataSetTableAdapters.tblSubjectDetailsTableAdapter();
			ta.Fill(_dsLinks.tblSubjectDetails);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblSubjectDetailsViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadMarkerGen1 ( ) {
			BA.LinksDataSetTableAdapters.tblMarkerGen1TableAdapter ta = new BA.LinksDataSetTableAdapters.tblMarkerGen1TableAdapter();
			ta.Fill(_dsLinks.tblMarkerGen1);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblMarkerGen1ViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadMarkerGen2 ( ) {
			BA.LinksDataSetTableAdapters.tblMarkerGen2TableAdapter ta = new BA.LinksDataSetTableAdapters.tblMarkerGen2TableAdapter();
			ta.Fill(_dsLinks.tblMarkerGen2);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblMarkerGen2ViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadRelatedValues ( ) {
			BA.LinksDataSetTableAdapters.tblRelatedValuesTableAdapter ta = new BA.LinksDataSetTableAdapters.tblRelatedValuesTableAdapter();
			ta.Fill(_dsLinks.tblRelatedValues);
			CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblRelatedValuesViewSource")));
			vs.View.MoveCurrentToFirst();
		}
		private void LoadOutcomes ( ) {
			BA.LinksDataSetTableAdapters.tblOutcomeTableAdapter ta = new BA.LinksDataSetTableAdapters.tblOutcomeTableAdapter();
			ta.Fill(_dsLinks.tblOutcome);
		}
		private void LoadRelatedValuesNextVersionNumber ( ) {
			Int16 currentMaxVersion = Int16.MinValue;
			using ( SqlCommand cmd = new SqlCommand("Process.prcArchiveMaxVersion", _cnn) ) {
				cmd.CommandType = CommandType.StoredProcedure;
				try {
					_cnn.Open();
					object v = cmd.ExecuteScalar();
					currentMaxVersion = Convert.ToInt16(v);
				}
				finally {
					_cnn.Close();
				}
			}
			txtAlogrithmNumber.Text = Convert.ToString(currentMaxVersion + 1);
		}
		#endregion
		private void WriteXml ( DataTable dt ) {
			string timeCode = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
			string path = @"F:\Projects\Nls\Links2011\StableComparisonData\" + dt.TableName + timeCode + ".xml";
			dt.WriteXml(path, XmlWriteMode.WriteSchema);
		}
		private void btnCombine1_Click ( object sender, RoutedEventArgs e ) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			e.Source = _combinedButtonTag;

			btnSubject_Click(sender, e);
			btnRelatedStructure_Click(sender, e);
			btnResponse_Click(sender, e);
			btnSurveyTime_Click(sender, e);

			sw.Stop();
			string message = string.Format("Elapsed time for btnCombine1 operations: {0}", sw.Elapsed.ToString());
			btnUpdateAllTables_Click(sender, e);
			Trace.WriteLine(message);
			MessageBox.Show(message);
		}
		private void btnCombine2_Click ( object sender, RoutedEventArgs e ) {
			BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter taSurveyTimeRecent = new BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			e.Source = _combinedButtonTag;
			taSurveyTimeRecent.Fill(_dsLinks.vewSurveyTimeMostRecent);

			btnRosterGen1_Click(sender, e);
			btnParentsOfGen1Retro_Click(sender, e);
			btnParentsOfGen1Current_Click(sender, e);
			btnBabyDaddy_Click(sender, e);
			btnFatherOfGen2_Click(sender, e);
			//btnSubjectDetails_Click(sender, e);
			btnMarkerGen1_Click(sender, e);
			btnMarkerGen2_Click(sender, e);
			btnRelatedValues_Click(sender, e);
			btnOutcome_Click(sender, e);
			btnRelatedValuesArchive_Click(sender, e);

			sw.Stop();
			string message = string.Format("Elapsed time for btnCombine2 operations: {0}", sw.Elapsed.ToString());
			btnUpdateAllTables_Click(sender, e);
			Trace.WriteLine(message);
			MessageBox.Show(message);
		}
	}
}
