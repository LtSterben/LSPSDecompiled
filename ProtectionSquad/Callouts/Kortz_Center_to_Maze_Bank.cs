using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Protection_Squad.Positions;
using Rage;
using Rage.Native;

namespace Protection_Squad.Callouts
{
	// Token: 0x0200001E RID: 30
	[CalloutInfo("Kortz Center to Maze Bank", 3)]
	public class Kortz_Center_to_Maze_Bank : Callout
	{
		// Token: 0x06000074 RID: 116 RVA: 0x0000A890 File Offset: 0x00008A90
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = KortzCenter.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, KortzCenter.sPos_limo, KortzCenter.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, KortzCenter.sPos_fbi1, KortzCenter.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, KortzCenter.sPos_fbi2, KortzCenter.he_fbi2);
			Kortz_Center_to_Maze_Bank.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Kortz_Center_to_Maze_Bank.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Kortz_Center_to_Maze_Bank.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Kortz_Center_to_Maze_Bank.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Kortz_Center_to_Maze_Bank.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Kortz_Center_to_Maze_Bank.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Kortz_Center_to_Maze_Bank.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Kortz_Center_to_Maze_Bank.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Kortz_Center_to_Maze_Bank.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Kortz Center to Maze Bank";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000AA14 File Offset: 0x00008C14
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Kortz Center~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.KORtoMZB;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(MazeBank.sPos_limo, this.SpawnPoint);
			EManager.departure = KortzCenter.sPos_limo;
			EManager.arrival = MazeBank.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Maze Bank";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000AAC0 File Offset: 0x00008CC0
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Kortz_Center_to_Maze_Bank.p_fbi1.Delete();
			Kortz_Center_to_Maze_Bank.p_fbi2.Delete();
			Kortz_Center_to_Maze_Bank.p_limo.Delete();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000AB10 File Offset: 0x00008D10
		public override void Process()
		{
			base.Process();
			this.isCalloutAccepted = true;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 35f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("CLEAR_ALL_HELP_MESSAGES", Array.Empty<NativeArgument>());
			if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
			{
				try
				{
					Kortz_Center_to_Maze_Bank.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Kortz_Center_to_Maze_Bank.prota = new Vehicle("FBI2", KortzCenter.sPos_prota, KortzCenter.he_prota);
					goto IL_AC;
				}
			}
			Kortz_Center_to_Maze_Bank.prota = new Vehicle("FBI2", KortzCenter.sPos_prota, KortzCenter.he_prota);
			IL_AC:
			if (Kortz_Center_to_Maze_Bank.prota == null || !Kortz_Center_to_Maze_Bank.prota.IsValid() || !EntityExtensions.Exists(Kortz_Center_to_Maze_Bank.prota))
			{
				Kortz_Center_to_Maze_Bank.prota = new Vehicle("FBI2", KortzCenter.sPos_prota, KortzCenter.he_prota);
			}
			EManager.protaM = Kortz_Center_to_Maze_Bank.prota;
			this.blBlip.Delete();
			Kortz_Center_to_Maze_Bank.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Kortz_Center_to_Maze_Bank.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Kortz_Center_to_Maze_Bank.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				KortzCenter.sPos_prota.X,
				KortzCenter.sPos_prota.Y,
				KortzCenter.sPos_prota.Z,
				0,
				0,
				0,
				5f,
				(int)Color.Yellow.R,
				(int)Color.Yellow.G,
				(int)Color.Yellow.B,
				30,
				0
			});
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 3f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
			{
				num
			});
			UI.canReturn = false;
			UI.EscortInfo();
			this.fbi1 = EManager.fbi1M;
			this.fbi2 = EManager.fbi2M;
			this.limo = EManager.limoM;
			Game.FrameRender += EManager.Game_FrameRenderRes;
			GameFiber.StartNew(new ThreadStart(new EManager().ManageRiskLevel));
			Game.DisplaySubtitle("Wait for the VIP to take his seat.", 5000);
			GameFiber.Sleep(4000);
			Kortz_Center_to_Maze_Bank.vip = new Ped(EManager.vipModel, KortzCenter.wPos_vip, KortzCenter.he_vip);
			Kortz_Center_to_Maze_Bank.vip.MakePersistent();
			Kortz_Center_to_Maze_Bank.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			Kortz_Center_to_Maze_Bank.vip.IsVisible = true;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Kortz_Center_to_Maze_Bank.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Kortz_Center_to_Maze_Bank.p_fbi1.RelationshipGroup = "POLICE";
			Kortz_Center_to_Maze_Bank.p_fbi2.RelationshipGroup = "POLICE";
			Kortz_Center_to_Maze_Bank.p_limo.RelationshipGroup = "POLICE";
			Kortz_Center_to_Maze_Bank.vip.RelationshipGroup = "POLICE";
			Kortz_Center_to_Maze_Bank.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Kortz_Center_to_Maze_Bank.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Kortz_Center_to_Maze_Bank.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Game.SetRelationshipBetweenRelationshipGroups(Kortz_Center_to_Maze_Bank.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Kortz_Center_to_Maze_Bank.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Kortz_Center_to_Maze_Bank.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Kortz_Center_to_Maze_Bank.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Kortz_Center_to_Maze_Bank.p_fbi1.RelationshipGroup, "COP", 1);
			Kortz_Center_to_Maze_Bank.p_limo.CanAttackFriendlies = false;
			Kortz_Center_to_Maze_Bank.p_fbi1.CanAttackFriendlies = false;
			Kortz_Center_to_Maze_Bank.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Kortz_Center_to_Maze_Bank.arBlip = new Blip(MazeBank.sPos_prota);
			Kortz_Center_to_Maze_Bank.arBlip.Color = Color.Red;
			Kortz_Center_to_Maze_Bank.arBlip.EnableRoute(Color.Red);
			GameFiber.StartNew(new ThreadStart(new EManager().ExternalEvent));
			this.limo.IsSirenOn = true;
			this.fbi1.IsSirenOn = true;
			this.fbi2.IsSirenOn = true;
			if (Main.muteSirens)
			{
				this.limo.IsSirenOn = true;
				this.fbi1.IsSirenSilent = true;
				this.fbi2.IsSirenSilent = true;
			}
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Kortz_Center_to_Maze_Bank.p_limo,
				this.limo,
				Kortz_Center_to_Maze_Bank.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Kortz_Center_to_Maze_Bank.p_fbi1,
				this.fbi1,
				this.limo,
				-1,
				22f,
				1074528293,
				7f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Kortz_Center_to_Maze_Bank.p_fbi2,
				this.fbi2,
				this.fbi1,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			GameFiber.StartNew(new ThreadStart(new EManager().StuckVehicle));
			GameFiber.StartNew(new ThreadStart(new EManager().DistanceWatcher));
			Ped ped = new Ped("S_M_Y_Construct_01", MazeBank.sPos_limo, MazeBank.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, MazeBank.sPos_prota) > 75f)
			{
				GameFiber.Yield();
			}
			Vehicle[] nearbyVehicles = ped.GetNearbyVehicles(14);
			try
			{
				foreach (Vehicle vehicle in nearbyVehicles)
				{
					if (!vehicle.IsEngineOn)
					{
						vehicle.Delete();
					}
					GameFiber.Yield();
				}
			}
			catch (Exception)
			{
			}
			ped.Delete();
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, MazeBank.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Kortz_Center_to_Maze_Bank.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					MazeBank.sPos_prota.X,
					MazeBank.sPos_prota.Y,
					MazeBank.sPos_prota.Z,
					0,
					0,
					0,
					5f,
					(int)Color.Yellow.R,
					(int)Color.Yellow.G,
					(int)Color.Yellow.B,
					50,
					0
				});
				vector..ctor(MazeBank.sPos_prota.X, MazeBank.sPos_prota.Y, MazeBank.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					MazeBank.sPos_limo.X,
					MazeBank.sPos_limo.Y,
					MazeBank.sPos_limo.Z,
					0,
					0,
					0,
					5f,
					(int)Color.Yellow.R,
					(int)Color.Yellow.G,
					(int)Color.Yellow.B,
					50,
					0
				});
				vector..ctor(MazeBank.sPos_limo.X, MazeBank.sPos_limo.Y, MazeBank.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Kortz_Center_to_Maze_Bank.p_limo.Tasks.ParkVehicle(MazeBank.sPos_limo, MazeBank.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Kortz_Center_to_Maze_Bank.p_fbi1.Tasks.ParkVehicle(MazeBank.sPos_fbi1, MazeBank.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Kortz_Center_to_Maze_Bank.p_fbi2.Tasks.ParkVehicle(MazeBank.sPos_fbi2, MazeBank.he_fbi2);
				goto IL_BCF;
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi2 couldn't park.");
				goto IL_BCF;
			}
			IL_BCA:
			GameFiber.Yield();
			IL_BCF:
			if (Vector3.Distance(Game.LocalPlayer.Character.Position, vector) <= 4f)
			{
				Game.FrameRender -= EManager.Mod_Controls2;
				GameFiber.Sleep(2000);
				NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
				{
					num2
				});
				while (Vector3.Distance(this.limo.Position, MazeBank.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					Kortz_Center_to_Maze_Bank.p_limo.Tasks.Clear();
					Kortz_Center_to_Maze_Bank.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Kortz_Center_to_Maze_Bank.vip.Tasks.FollowNavigationMeshToPosition(MazeBank.wPos_vip, MazeBank.he_vip - 180f, 0.5f).WaitForCompletion();
				Kortz_Center_to_Maze_Bank.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BCA;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000B820 File Offset: 0x00009A20
		public override void End()
		{
			base.End();
			EManager.currentCallState = EManager.CallState.Arrived;
			Game.FrameRender -= EManager.Game_FrameRender;
			if (this.isCalloutAccepted)
			{
				this.limo.IsHandbrakeForced = false;
			}
			try
			{
				Kortz_Center_to_Maze_Bank.arBlip.Delete();
			}
			catch (Exception)
			{
			}
			try
			{
				this.b_limo.Delete();
			}
			catch (Exception)
			{
			}
			try
			{
				this.b_fbi1.Delete();
			}
			catch (Exception)
			{
			}
			try
			{
				this.b_fbi2.Delete();
			}
			catch (Exception)
			{
			}
			try
			{
				EManager.limoM.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				EManager.fbi1M.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				EManager.fbi2M.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Kortz_Center_to_Maze_Bank.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Kortz_Center_to_Maze_Bank.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Kortz_Center_to_Maze_Bank.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Kortz_Center_to_Maze_Bank.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x04000148 RID: 328
		private Vector3 SpawnPoint;

		// Token: 0x04000149 RID: 329
		internal static Ped p_limo;

		// Token: 0x0400014A RID: 330
		internal static Ped vip;

		// Token: 0x0400014B RID: 331
		internal static Ped p_fbi1;

		// Token: 0x0400014C RID: 332
		internal static Ped p_fbi2;

		// Token: 0x0400014D RID: 333
		private Blip blBlip;

		// Token: 0x0400014E RID: 334
		internal static Blip arBlip;

		// Token: 0x0400014F RID: 335
		private Blip b_limo;

		// Token: 0x04000150 RID: 336
		private Blip b_fbi1;

		// Token: 0x04000151 RID: 337
		private Blip b_fbi2;

		// Token: 0x04000152 RID: 338
		private float distance;

		// Token: 0x04000153 RID: 339
		internal static Vehicle prota;

		// Token: 0x04000154 RID: 340
		private bool isCalloutAccepted;

		// Token: 0x04000155 RID: 341
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000156 RID: 342
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x04000157 RID: 343
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
