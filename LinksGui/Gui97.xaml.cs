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
using BA = Nls.Base97;

namespace LinksGui {
	public partial class Gui97 : Window {
		#region Fields
		private BA.ImportDataSet _dsImport;
		private BA.LinksDataSet _dsLinks;
		private SqlConnection _cnn;
		private const string _cnnStringNameInAppConfig = "LinksGui.Properties.Settings.Nlsy97ConnectionStringGui";
		private const string _combinedButtonTag = "CombinedButton";
		#endregion
        public Gui97( ) {
			InitializeComponent();
		}
        private void Window_Loaded( object sender, RoutedEventArgs e ) {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[_cnnStringNameInAppConfig];
            Trace.Assert(settings != null, "The connectionStringSettings object should be initialized propertly.");
            _cnn = new SqlConnection(settings.ConnectionString);

            _dsImport = ((BA.ImportDataSet)(this.FindResource("importDataSet")));
            _dsLinks = ((BA.LinksDataSet)(this.FindResource("linksDataSet")));

            LoadExtracts();
            LoadMetadata();

            LoadSubject();
            LoadRelatedStructure();
            LoadResponse();
            LoadSurveyTime();
            LoadSurveyTimeMostRecent(); //Needed for tblRelated
            LoadRoster();
            //LoadParentsOfGen1Retro();
            LoadSubjectDetails();
            LoadMarker();
            LoadRelatedValues();
            //LoadRelatedValuesNextVersionNumber();
            sw.Stop();
            string message = string.Format("DataSets loaded (Elapsed time: {0})", sw.Elapsed.ToString());
            Trace.WriteLine(message);
            Console.Beep(3000, 100);
            Console.Beep(300, 100);
            Console.Beep(3000, 100);
        }
        #region Fill & Update
        private void btnSubject_Click( object sender, RoutedEventArgs e ) {
            string message = BA.Subject.CreateSubject(_dsImport, _dsLinks);
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblSubject);
        }
        private void btnRelatedStructure_Click( object sender, RoutedEventArgs e ) {
            BA.RelatedStructure related = new BA.RelatedStructure(_dsLinks);
            string message = related.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblRelatedStructure);
        }
        private void btnResponse_Click( object sender, RoutedEventArgs e ) {
            BA.Response response = new BA.Response(_dsImport, _dsLinks);
            string message = response.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            ////WriteXml(_dsLinks.tblResponse);
        }
        private void btnSurveyTime_Click( object sender, RoutedEventArgs e ) {
            BA.SurveyTime surveyTime = new BA.SurveyTime(_dsLinks);
            string message = surveyTime.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblSurveyTime);

            LoadSurveyTimeMostRecent(); 
        }
        private void btnRoster_Click( object sender, RoutedEventArgs e ) {
            BA.Roster roster = new BA.Roster(_dsLinks);
            string message = roster.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblRosterGen1);
        }

        private void btnSubjectDetails_Click( object sender, RoutedEventArgs e ) {
            BA.SubjectDetails subjectDetails = new BA.SubjectDetails(_dsLinks);
            string message = subjectDetails.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblSubjectDetails);
        }
        private void btnMarker_Click( object sender, RoutedEventArgs e ) {
            BA.Marker marker = new BA.Marker(_dsLinks, _dsImport);
            string message = marker.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblMarker);
        }
        private void btnRelatedValues_Click( object sender, RoutedEventArgs e ) {
            BA.RelatedValues related = new BA.RelatedValues(_dsImport, _dsLinks);
            string message = related.Go();
            Trace.WriteLine(message);
            if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            //WriteXml(_dsLinks.tblRelatedValues);
        }
        private void btnOutcome_Click( object sender, RoutedEventArgs e ) {
            //BA.Outcome outcome = new BA.Outcome(_dsLinks);
            //string message = outcome.Go();
            //Trace.WriteLine(message);
            //if( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
            ////WriteXml(_dsLinks.tblRelatedValues);
        }
        private void btnRelatedValuesArchive_Click ( object sender, RoutedEventArgs e ) {            
        //    Int16 algorithmVersion = Int16.Parse(txtAlogrithmNumber.Text);
        //    string message = BA.RelatedValues.Archive(algorithmVersion, _dsLinks);
        //    Trace.WriteLine(message);
        //    if ( e.Source.ToString() != _combinedButtonTag ) MessageBox.Show(message);
        //    //WriteXml(_dsLinks.tblRelatedValuesArchive);
        }

        private void btnUpdateAllTables_Click ( object sender, RoutedEventArgs e ) {
            const string schemaName = "Process";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BulkUpdate(schemaName, _dsLinks.tblSubject, LoadSubject);
            BulkUpdate(schemaName, _dsLinks.tblRelatedStructure, LoadRelatedStructure);

            BulkUpdate(schemaName, _dsLinks.tblResponse, AcceptResponseChanges);
            BulkUpdate(schemaName, _dsLinks.tblSurveyTime, LoadSurveyTime);
            BulkUpdate(schemaName, _dsLinks.tblRoster, LoadRoster);
        //    BulkUpdate(schemaName, _dsLinks.tblParentsOfGen1Retro, LoadParentsOfGen1Retro);
            BulkUpdate(schemaName, _dsLinks.tblSubjectDetails, LoadSubjectDetails);
            BulkUpdate(schemaName, _dsLinks.tblMarker, LoadMarker);
            BulkUpdate(schemaName, _dsLinks.tblRelatedValues, LoadRelatedValues);
        //    BulkUpdate(schemaName, _dsLinks.tblOutcome, LoadOutcomes);
        //    BulkUpdate("Archive", _dsLinks.tblRelatedValuesArchive, null);

            sw.Stop();
            //string message = string.Format("The follow records were affected for each table ({0} Elapsed):\n{1:N0} tblSubject\n{2:N0} tblResponse", sw.Elapsed.ToString(), -999, -999);
            string message = string.Format("Elapsed time for BulkCopy operations: {0}", sw.Elapsed.ToString());
            Trace.WriteLine(message);
            MessageBox.Show(message);
        }
        private void BulkUpdate( string schemaName, DataTable dt, Action loadMethod ) {
            const Int32 batchSize = 10000;
            bool hasChanges = true;
            try {
                hasChanges = dt.GetChanges() != null;
            } catch( OutOfMemoryException ex ) {
                Debug.WriteLine(ex.ToString());//If this fails from, then there were changes.
                throw new Exception("There were changes to the database.", ex);
            }

            if( hasChanges ) {
                try {
                    _cnn.Open();
                    SqlBulkCopy blk = new SqlBulkCopy(_cnn, SqlBulkCopyOptions.CheckConstraints, null);//Pass a null transaction.
                    blk.DestinationTableName = schemaName + "." + dt.TableName;
                    blk.NotifyAfter = 100;
                    blk.BulkCopyTimeout = _cnn.ConnectionTimeout;
                    blk.BatchSize = batchSize;
                    blk.WriteToServer(dt);
                    blk.Close();
                    if( loadMethod != null ) loadMethod();
                } catch {
                    throw;
                } finally {
                    _cnn.Close();
                }
            }//End if(dt.GetChanges() ! =null)
        }
        #endregion
        #region Load DataTables
        private void LoadExtracts( ) {
            if( _dsLinks.tblSubject.Count == 0 || _dsLinks.tblRelatedStructure.Count == 0 || _dsLinks.tblSubject.Count == 0 || _dsLinks.tblSubject.Count == 0 ) {
                BA.ImportDataSetTableAdapters.tblDemographicsTableAdapter ta = new BA.ImportDataSetTableAdapters.tblDemographicsTableAdapter();
                ta.Fill(_dsImport.tblDemographics);

                BA.ImportDataSetTableAdapters.tblRosterTableAdapter ta_roster = new BA.ImportDataSetTableAdapters.tblRosterTableAdapter();
                ta_roster.Fill(_dsImport.tblRoster);

                BA.ImportDataSetTableAdapters.tblSurveyTimeTableAdapter ta_survey_time = new BA.ImportDataSetTableAdapters.tblSurveyTimeTableAdapter();
                ta_survey_time.Fill(_dsImport.tblSurveyTime);

                BA.ImportDataSetTableAdapters.tblLinksExplicitTableAdapter ta_links_explicit = new BA.ImportDataSetTableAdapters.tblLinksExplicitTableAdapter();
                ta_links_explicit.Fill(_dsImport.tblLinksExplicit);

                BA.ImportDataSetTableAdapters.tblLinksImplicitTableAdapter ta_links_implicit = new BA.ImportDataSetTableAdapters.tblLinksImplicitTableAdapter();
                ta_links_implicit.Fill(_dsImport.tblLinksImplicit);

                BA.ImportDataSetTableAdapters.tblTwinsTableAdapter ta_twins = new BA.ImportDataSetTableAdapters.tblTwinsTableAdapter();
                ta_twins.Fill(_dsImport.tblTwins);
            }
        }
        private void LoadMetadata( ) {
            BA.LinksDataSetTableAdapters.tblItemTableAdapter ta_item = new BA.LinksDataSetTableAdapters.tblItemTableAdapter();
            ta_item.Fill(_dsLinks.tblItem);
         
            BA.LinksDataSetTableAdapters.tblVariableTableAdapter ta_variable = new BA.LinksDataSetTableAdapters.tblVariableTableAdapter();
            ta_variable.Fill(_dsLinks.tblVariable);
         
            BA.LinksDataSetTableAdapters.tblMzManualTableAdapter ta_mz_manual = new BA.LinksDataSetTableAdapters.tblMzManualTableAdapter();
            ta_mz_manual.Fill(_dsLinks.tblMzManual);
         
            BA.LinksDataSetTableAdapters.tblRosterAssignmentTableAdapter ta_roster_assignment = new BA.LinksDataSetTableAdapters.tblRosterAssignmentTableAdapter();
            ta_roster_assignment.Fill(_dsLinks.tblRosterAssignment);
        }

        ///////////////////////////

        private void LoadSubject( ) {
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
            SqlCommand cmd = new SqlCommand("Process.prcResponseRetrieveSubset", _cnn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(_dsLinks.tblResponse);

            CollectionViewSource tblResponseViewSource = ((CollectionViewSource)(this.FindResource("tblResponseViewSource")));
            tblResponseViewSource.View.MoveCurrentToFirst();
        }
        private void AcceptResponseChanges( ) {
            _dsLinks.tblResponse.AcceptChanges();
        }
        private void LoadSurveyTime( ) {
            BA.LinksDataSetTableAdapters.tblSurveyTimeTableAdapter ta = new BA.LinksDataSetTableAdapters.tblSurveyTimeTableAdapter();
            ta.Fill(_dsLinks.tblSurveyTime);
            CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblSurveyTimeViewSource")));
            vs.View.MoveCurrentToFirst();
        }
        private void LoadSurveyTimeMostRecent( ) {
            BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter ta = new BA.LinksDataSetTableAdapters.vewSurveyTimeMostRecentTableAdapter();
            ta.Fill(_dsLinks.vewSurveyTimeMostRecent);
        }
        private void LoadRoster( ) {
            BA.LinksDataSetTableAdapters.tblRosterTableAdapter ta = new BA.LinksDataSetTableAdapters.tblRosterTableAdapter();
            ta.Fill(_dsLinks.tblRoster);
            CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblRosterViewSource")));
            vs.View.MoveCurrentToFirst();
        }
        private void LoadSubjectDetails( ) {
            BA.LinksDataSetTableAdapters.tblSubjectDetailsTableAdapter ta = new BA.LinksDataSetTableAdapters.tblSubjectDetailsTableAdapter();
            ta.Fill(_dsLinks.tblSubjectDetails);
            CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblSubjectDetailsViewSource")));
            vs.View.MoveCurrentToFirst();
        }
        private void LoadMarker( ) {
            BA.LinksDataSetTableAdapters.tblMarkerTableAdapter ta = new BA.LinksDataSetTableAdapters.tblMarkerTableAdapter();
            ta.Fill(_dsLinks.tblMarker);
            CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblMarkerViewSource")));
            vs.View.MoveCurrentToFirst();
        }

        private void LoadRelatedValues( ) {
            BA.LinksDataSetTableAdapters.tblRelatedValuesTableAdapter ta = new BA.LinksDataSetTableAdapters.tblRelatedValuesTableAdapter();
            ta.Fill(_dsLinks.tblRelatedValues);
            CollectionViewSource vs = ((CollectionViewSource)(this.FindResource("tblRelatedValuesViewSource")));
            vs.View.MoveCurrentToFirst();
        }
        private void LoadOutcomes( ) {
            BA.LinksDataSetTableAdapters.tblOutcomeTableAdapter ta = new BA.LinksDataSetTableAdapters.tblOutcomeTableAdapter();
            ta.Fill(_dsLinks.tblOutcome);
        }
        //private void LoadRelatedValuesNextVersionNumber ( ) {
        //    Int16 currentMaxVersion = Int16.MinValue;
        //    using ( SqlCommand cmd = new SqlCommand("Process.prcArchiveMaxVersion", _cnn) ) {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        try {
        //            _cnn.Open();
        //            object v = cmd.ExecuteScalar();
        //            currentMaxVersion = Convert.ToInt16(v);
        //        }
        //        finally {
        //            _cnn.Close();
        //        }
        //    }
        //    txtAlogrithmNumber.Text = Convert.ToString(currentMaxVersion + 1);
        //}
        #endregion
        //private void WriteXml ( DataTable dt ) {
        //    string timeCode = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
        //    string path = @"F:\Projects\Nls\Links2011\StableComparisonData\" + dt.TableName + timeCode + ".xml";
        //    dt.WriteXml(path, XmlWriteMode.WriteSchema);
        //}
        private void btnSetup_Click( object sender, RoutedEventArgs e ) {
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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            e.Source = _combinedButtonTag;

            LoadSurveyTimeMostRecent(); 
            btnRoster_Click(sender, e);
            //btnParentsOfGen1Retro_Click(sender, e);
            btnSubjectDetails_Click(sender, e);
            btnMarker_Click(sender, e);
            btnRelatedValues_Click(sender, e);
            btnOutcome_Click(sender, e);
            //btnRelatedValuesArchive_Click(sender, e);

            sw.Stop();
            string message = string.Format("Elapsed time for btnCombine2 operations: {0}", sw.Elapsed.ToString());
            btnUpdateAllTables_Click(sender, e);
            Trace.WriteLine(message);
            MessageBox.Show(message);
        }

    }
}
