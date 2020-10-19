using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

namespace Protection_Squad
{
	// Token: 0x02000007 RID: 7
	internal class PCALL_Manager
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00005898 File Offset: 0x00003A98
		internal static void GeneratePrisoner()
		{
			int index = new Random().Next(0, PCALL_Manager.pNames.Count);
			PCALL_Manager.currentpName = PCALL_Manager.pNames.ElementAt(index);
			PCALL_Manager.currentpCrime = PCALL_Manager.pCrimes.ElementAt(index);
			PCALL_Manager.pModel = PCALL_Manager.lModels.ElementAt(new Random().Next(0, PCALL_Manager.lModels.Count));
			PCALL_Manager.jNumber = new Random().Next(1, 14);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00005914 File Offset: 0x00003B14
		internal void isVIPDead()
		{
			this.isMissionFailed = false;
			try
			{
				while (EManager.currentCallState != EManager.CallState.Arrived)
				{
					if (EManager.vipM.IsInjured)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: The inmate got injured.", 5000);
						this.isMissionFailed = true;
					}
					if (EManager.p_limoM.IsDead)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: The limo driver is dead.", 5000);
						this.isMissionFailed = true;
					}
					if (Game.LocalPlayer.Character.IsDead)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: You died.", 8000);
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

		// Token: 0x0600002B RID: 43 RVA: 0x000059D8 File Offset: 0x00003BD8
		public static void Game_FrameRenderRes(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.47f,
				0.3f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text(" >  ~y~Inmate Card", 0.003f, 0.003f, 0.5f);
			EManager.Text("Name : ~b~" + PCALL_Manager.currentpName, 0.033f, 0.003f, 0.5f);
			EManager.Text("Sentenced to ~b~" + PCALL_Manager.jNumber.ToString() + " years ~w~in jail", 0.063f, 0.003f, 0.5f);
			EManager.Text("for ~r~" + PCALL_Manager.currentpCrime + ".", 0.093f, 0.003f, 0.5f);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00005B04 File Offset: 0x00003D04
		public static void Game_FrameRender(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.274f,
				0.2f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("Inmate : ~b~" + PCALL_Manager.currentpName, 0.003f, 0.003f, 0.5f);
			if (Main.shRisk)
			{
				EManager.Text("Risk Level : " + EManager.riskLevel, 0.033f, 0.003f, 0.5f);
			}
			EManager.Text("Distance : " + ((int)Vector3.Distance(Game.LocalPlayer.Character.Position, EManager.arrival)).ToString() + " yards", 0.063f, 0.003f, 0.5f);
		}

		// Token: 0x0400002E RID: 46
		public static List<string> vA = new List<string>(new string[]
		{
			"EMPEROR",
			"INTRUDER",
			"PRIMO2",
			"BISON",
			"CHINO2",
			"BALLER"
		});

		// Token: 0x0400002F RID: 47
		public static List<string> lModels = new List<string>(new string[]
		{
			"S_M_Y_Prisoner_01",
			"U_M_Y_Prisoner_01"
		});

		// Token: 0x04000030 RID: 48
		public static List<string> pNames = new List<string>(new string[]
		{
			"El Cabron",
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

		// Token: 0x04000031 RID: 49
		public static List<string> pCrimes = new List<string>(new string[]
		{
			"Manslaughter in the first degree",
			"Placing a hazardous substance in a sports stadium",
			"Unlawful disposal of methamphetamine laboratory material",
			"Criminal sale of marijuana in the fifth degree",
			"Tampering with a juror in the second degree",
			"Aggravated unpermitted use of indoor pyrotechnics",
			"Strangulation in the first degree",
			"Unlawful duplication of computer related material",
			"Money laundering in the first degree",
			"Reckless assault of a child",
			"Murder on a police officer"
		});

		// Token: 0x04000032 RID: 50
		internal static string currentpName;

		// Token: 0x04000033 RID: 51
		internal static string currentpCrime;

		// Token: 0x04000034 RID: 52
		internal static string pModel;

		// Token: 0x04000035 RID: 53
		private static int jNumber;

		// Token: 0x04000036 RID: 54
		private bool isMissionFailed;
	}
}
