using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nls.BaseAssembly {
	public class MarkerGen1Summary {
		#region Fields
		private readonly MarkerEvidence _sameGeneration;
		private readonly MarkerEvidence _shareBiomom;
		private readonly MarkerEvidence _shareBiodad;
		private readonly MarkerEvidence _shareBiograndparent;
		#endregion
		#region Properties
		public MarkerEvidence SameGeneration { get { return _sameGeneration; } }
		public MarkerEvidence ShareBiomom { get { return _shareBiomom; } }
		public MarkerEvidence ShareBiodad { get { return _shareBiodad; } }
		public MarkerEvidence ShareBiograndparent { get { return _shareBiograndparent; } }
		#endregion
		#region Constructor
		public MarkerGen1Summary ( MarkerEvidence sameGeneration, MarkerEvidence shareBiomom, MarkerEvidence shareBiodad, MarkerEvidence shareBiograndparent ) {
			_sameGeneration = sameGeneration;
			_shareBiomom = shareBiomom;
			_shareBiodad = shareBiodad;
			_shareBiograndparent = shareBiograndparent;
		}
		#endregion
	}
}
