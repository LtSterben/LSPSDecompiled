using System;
using Protection_Squad.Positions;
using Rage;

namespace Protection_Squad
{
	// Token: 0x02000006 RID: 6
	internal class Map
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00005863 File Offset: 0x00003A63
		public Vector3 sPosLimo
		{
			get
			{
				if (EManager.currentCall == EManager.Calls.LSAtoARC || EManager.currentCall == EManager.Calls.LSAtoGAL || EManager.currentCall == EManager.Calls.LSAtoMZB || EManager.currentCall == EManager.Calls.LSAtoNOO || EManager.currentCall == EManager.Calls.LSAtoUD)
				{
					return LSAirport.sPos_limo;
				}
				return Vector3.Zero;
			}
		}
	}
}
