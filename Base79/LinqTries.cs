		//private static Int64 RetrieveSubjectTag ( Int32 subjectID, Generation generation, LinksDataSet dsLinks ) {
		//   IEnumerable<Int64> tags = from dr in dsLinks.tblSubject
		//                                                     where dr.SubjectID.Equals(subjectID) && dr.Generation.Equals((byte)generation)
		//                                                     select dr.SubjectTag;
		//   Trace.Assert(tags.Count() == 1, "There should be exactly one row retrieved.");
		//   return tags.First();
		//}
		//private static Int64 RetrieveSubjectTag ( Int32 subjectID, Generation generation, LinksDataSet dsLinks ) {
		//   IEnumerable<LinksDataSet.tblSubjectRow> drs = from dr in dsLinks.tblSubject
		//                                                     where dr.SubjectID.Equals(subjectID) && dr.Generation.Equals((byte)generation)
		//                                                     select dr;
		//   Trace.Assert(drs.Count() == 1, "There should be exactly one row retrieved.");
		//   return drs.First().SubjectTag;
		//}
		//private static Int64 RetrieveSubjectTagV3 ( Int32 subjectID, Generation generation, LinksDataSet dsLinks ) {
		//   return dsLinks.tblSubject.Single(dr => dr.SubjectID == subjectID && dr.Generation == (byte)generation).SubjectTag;
		//}

		//byte maxLoopIndex = (from dr in drsForLoopIndex select dr.LoopIndex).Max();
