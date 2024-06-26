using EdiEngine.Common.Enums;
using EdiEngine.Common.Definitions;
using EdiEngine.Standards.X12_004010VICS.Segments;

namespace EdiEngine.Standards.X12_004010VICS.Maps
{
	public class M_893 : MapLoop
	{
		public M_893() : base(null)
		{
			Content.AddRange(new MapBaseEntity[] {
				new G39() { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 9999 },
				new G69() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 1 },
				new L_N1(this) { ReqDes = RequirementDesignator.Mandatory, MaxOccurs = 999 },
			});
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
					new N4() { ReqDes = RequirementDesignator.Optional, MaxOccurs = 2 },
				});
			}
		}

	}
 }

