using EdiEngine.Common.Enums;
using EdiEngine.Common.Definitions;
using EdiEngine.Standards.X12_005010.Segments;

namespace EdiEngine.Standards.X12_005010.Maps
{
	public class M_107 : MapLoop
	{
		public M_107() : base(null)
		{
			Content.AddRange(new MapBaseEntity[] {
				new BGN() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
				new G62() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 10 },
				new AT5() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
				new PR() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
				new ID4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new IV1() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new MI1() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new CUR() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new MCT() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999 },
				new L_MS2(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new L_N1(this) { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 10 },
				new L_LX(this) { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 99999 },
			});
		}

		//0050
		public class L_MS2 : MapLoop
		{
			public L_MS2(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new MS2() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new AT9() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				});
			}
		}

		//0100
		public class L_N1 : MapLoop
		{
			public L_N1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new PER() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 5 },
				});
			}
		}

		//0200
		public class L_LX : MapLoop
		{
			public L_LX(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new LX() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new GY() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 999 },
					new CUR() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new PR() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
					new ID4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new AT5() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
					new L_MS2_1(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new L_N1_1(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new L_CA1(this) { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 99999 },
				});
			}
		}

		//0205
		public class L_MS2_1 : MapLoop
		{
			public L_MS2_1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new MS2() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new AT9() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				});
			}
		}

		//0210
		public class L_N1_1 : MapLoop
		{
			public L_N1_1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new G62() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 10 },
				});
			}
		}

		//0230
		public class L_CA1 : MapLoop
		{
			public L_CA1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new CA1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new GY() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 999 },
					new PR() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
					new ID4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new IV1() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new SV() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new AT5() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 99 },
					new MCT() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999 },
					new L_MS2_2(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new L_N1_2(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				});
			}
		}

		//0232
		public class L_MS2_2 : MapLoop
		{
			public L_MS2_2(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new MS2() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new AT9() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				});
			}
		}

		//0235
		public class L_N1_2 : MapLoop
		{
			public L_N1_2(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new G62() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 10 },
				});
			}
		}

	}
 }

