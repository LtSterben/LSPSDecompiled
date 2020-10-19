using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using Protection_Squad.Callouts;
using Rage;
using Rage.Native;

namespace Protection_Squad
{
	// Token: 0x02000008 RID: 8
	internal class SCALL_Manager
	{
		// Token: 0x0600002F RID: 47 RVA: 0x00005D78 File Offset: 0x00003F78
		internal void isVIPDead_EMS()
		{
			this.isMissionFailed = false;
			try
			{
				while (EManager.currentCallState != EManager.CallState.Arrived)
				{
					if (Ambulance_Escort_Bollingbrook_to_Davis.vip.IsInjured)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: The VIP is dead.", 5000);
						this.isMissionFailed = true;
					}
					if (Ambulance_Escort_Bollingbrook_to_Davis.pems_1.IsDead)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: The ambulance driver is dead.", 5000);
						this.isMissionFailed = true;
					}
					if (this.isMissionFailed)
					{
						Functions.StopCurrentCallout();
						EManager.currentCallState = EManager.CallState.Arrived;
					}
					GameFiber.Yield();
				}
			}
			catch (Exception)
			{
				for (;;)
				{
					GameFiber.Yield();
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00005E0C File Offset: 0x0000400C
		internal static void GeneratePrisoner_EMS()
		{
			int index = new Random().Next(0, SCALL_Manager.pNames.Count);
			SCALL_Manager.currentpName = SCALL_Manager.pNames.ElementAt(index);
			SCALL_Manager.jNumber = new Random().Next(2, 50);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00005E54 File Offset: 0x00004054
		internal void DistanceWatcher_EMS()
		{
			while (EManager.currentCallState != EManager.CallState.Arrived)
			{
				if (Vector3.Distance(Ambulance_Escort_Bollingbrook_to_Davis.limo.Position, Ambulance_Escort_Bollingbrook_to_Davis.prota.Position) > 40f && EManager.currentCallState != EManager.CallState.Parking)
				{
					Game.DisplaySubtitle("Stay close to the convoy to keep the team formation.", 3000);
					GameFiber.Wait(3000);
				}
				GameFiber.Yield();
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00005EB4 File Offset: 0x000040B4
		public static void Game_FrameRenderRes_EMS(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.47f,
				0.21f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text(" >  ~y~Inmate Card", 0.003f, 0.003f, 0.5f);
			EManager.Text("Name : ~b~Ramzi Howell", 0.033f, 0.003f, 0.5f);
			EManager.Text("~r~Injured during a brawl between inmates", 0.063f, 0.003f, 0.5f);
		}

		// Token: 0x04000037 RID: 55
		private bool isMissionFailed;

		// Token: 0x04000038 RID: 56
		public static List<string> pNames = new List<string>(new string[]
		{
			"Bob Ramzi",
			"Marcus Freedom",
			"Isaac Newton",
			"Kirk Coleman",
			"Bob White",
			"Ramzi Howell",
			"Ed Poole",
			"Lamarcus Terrell",
			"Dwayne Henry",
			"Devan Hopper",
			"Sam Russell"
		});

		// Token: 0x04000039 RID: 57
		internal static string currentpName;

		// Token: 0x0400003A RID: 58
		private static int jNumber;
	}
}
