using System;
using Rage;
using Rage.Native;

namespace Protection_Squad
{
	// Token: 0x02000002 RID: 2
	internal class Backup
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void RequestHeli()
		{
			Backup.hasHeliBeenAffected = false;
			while (EManager.currentCallState != EManager.CallState.Arrived)
			{
				if (!Backup.hasHeliBeenAffected)
				{
					switch (EManager.currentCall)
					{
					case EManager.Calls.ARCtoGOC:
						Backup.h = new Vehicle("FROGGER", new Vector3(-285.9166f, -619.4257f, 50.05699f), 255f);
						break;
					case EManager.Calls.LSAtoNOO:
					case EManager.Calls.LSAtoGAL:
					case EManager.Calls.LSAtoUD:
					case EManager.Calls.LSAtoMZB:
					case EManager.Calls.NOOtoMZB:
						goto IL_1B0;
					case EManager.Calls.KORtoGAL:
						Backup.h = new Vehicle("FROGGER", new Vector3(-2258.629f, 387.5014f, 188.3222f), 78.8285f);
						break;
					case EManager.Calls.PCALL_DWT_BBP:
						Backup.h = new Vehicle("FROGGER", new Vector3(449.1788f, -981.1251f, 43.41048f), 50.11336f);
						break;
					case EManager.Calls.MZBtoNOO:
						Backup.h = new Vehicle("FROGGER", new Vector3(-285.9166f, -619.4257f, 50.05699f), 255f);
						break;
					case EManager.Calls.UDPtoGOC:
						Backup.h = new Vehicle("FROGGER", new Vector3(-285.9166f, -619.4257f, 50.05699f), 255f);
						break;
					case EManager.Calls.KORtoMZB:
						Backup.h = new Vehicle("FROGGER", new Vector3(-2258.629f, 387.5014f, 188.3222f), 78.8285f);
						break;
					case EManager.Calls.GALtoRMS:
						Backup.h = new Vehicle("FROGGER", new Vector3(-413.7424f, 1098.916f, 332.2538f), 345.8513f);
						break;
					default:
						goto IL_1B0;
					}
					IL_1DD:
					Backup.p = new Ped("S_M_M_Pilot_02", Backup.h.Position, 0f);
					Backup.p.WarpIntoVehicle(Backup.h, -1);
					GameFiber.Sleep(2000);
					new EManager().SyncCall();
					Backup.p.BlockPermanentEvents = true;
					Backup.hasHeliBeenAffected = true;
					Backup.a = Backup.h.AttachBlip();
					Backup.a.Sprite = 15;
					goto IL_250;
					IL_1B0:
					Backup.h = new Vehicle("FROGGER", new Vector3(-1070.663f, -2946.223f, 13.665f), 232.3167f);
					goto IL_1DD;
				}
				IL_250:
				GameFiber.Sleep(10000);
				try
				{
					if (EntityExtensions.Exists(Backup.h) && Backup.h.IsValid())
					{
						NativeFunction.CallByName<uint>("TASK_HELI_CHASE", new NativeArgument[]
						{
							Backup.p,
							EManager.vipM,
							0f,
							0f,
							80f
						});
					}
				}
				catch (Exception)
				{
				}
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("TASK_HELI_MISSION", new NativeArgument[]
			{
				Backup.p,
				Backup.h,
				0,
				0,
				449.1788f,
				-981.1251f,
				43.41048f,
				4,
				20f,
				-1f,
				-1f,
				10,
				10,
				5f,
				0
			});
			Backup.h.Dismiss();
			Backup.a.Delete();
		}

		// Token: 0x04000001 RID: 1
		private static Blip a;

		// Token: 0x04000002 RID: 2
		private static Vehicle h;

		// Token: 0x04000003 RID: 3
		private static bool hasHeliBeenAffected;

		// Token: 0x04000004 RID: 4
		private static Ped p;
	}
}
