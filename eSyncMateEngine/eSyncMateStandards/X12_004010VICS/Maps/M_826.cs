using EdiEngine.Common.Enums;
using EdiEngine.Common.Definitions;
using EdiEngine.Standards.X12_004010VICS.Segments;

namespace EdiEngine.Standards.X12_004010VICS.Maps
{
	public class M_826 : MapLoop
	{
		public M_826() : base(null)
		{
			Content.AddRange(new MapBaseEntity[] {
				new BTI() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
				new DTM() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				new REF() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				new TIA() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				new YNQ() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				new L_N1(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				new L_TFS(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
			});
		}

		//1000
		public class L_N1 : MapLoop
		{
			public L_N1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new IN2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new PER() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				});
			}
		}

		//2000
		public class L_TFS : MapLoop
		{
			public L_TFS(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new TFS() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new REF() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new DTM() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new TIA() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new YNQ() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new L_N1_1(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new L_FGS(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				});
			}
		}

		//2100
		public class L_N1_1 : MapLoop
		{
			public L_N1_1(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new IN2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new PER() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				});
			}
		}

		//2200
		public class L_FGS : MapLoop
		{
			public L_FGS(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new FGS() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new REF() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new DTM() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new TIA() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new YNQ() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new L_N1_2(this) { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				});
			}
		}

		//2210
		public class L_N1_2 : MapLoop
		{
			public L_N1_2(MapLoop parentLoop) : base(parentLoop)
			{
				Content.AddRange(new MapBaseEntity[] {
					new N1() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 1 },
					new N2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new IN2() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
					new N3() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
					new PER() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 999999 },
				});
			}
		}

	}
 }

