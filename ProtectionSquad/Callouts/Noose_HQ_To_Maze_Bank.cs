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
	// Token: 0x02000025 RID: 37
	[CalloutInfo("Noose HQ To Maze Bank", 3)]
	public class Noose_HQ_To_Maze_Bank : Callout
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00012510 File Offset: 0x00010710
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = NOOSEHeadquarters.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, NOOSEHeadquarters.sPos_limo, NOOSEHeadquarters.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, NOOSEHeadquarters.sPos_fbi1, NOOSEHeadquarters.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, NOOSEHeadquarters.sPos_fbi2, NOOSEHeadquarters.he_fbi2);
			Noose_HQ_To_Maze_Bank.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Noose_HQ_To_Maze_Bank.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Noose_HQ_To_Maze_Bank.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Noose_HQ_To_Maze_Bank.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Noose_HQ_To_Maze_Bank.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Noose_HQ_To_Maze_Bank.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Noose_HQ_To_Maze_Bank.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Noose_HQ_To_Maze_Bank.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Noose_HQ_To_Maze_Bank.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : NOOSE HQ to Maze Bank";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00012694 File Offset: 0x00010894
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~NOOSE Headquarters~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.NOOtoMZB;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(MazeBank.sPos_limo, this.SpawnPoint);
			EManager.departure = NOOSEHeadquarters.sPos_limo;
			EManager.arrival = MazeBank.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Maze Bank";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00012740 File Offset: 0x00010940
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Noose_HQ_To_Maze_Bank.p_fbi1.Delete();
			Noose_HQ_To_Maze_Bank.p_fbi2.Delete();
			Noose_HQ_To_Maze_Bank.p_limo.Delete();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00012790 File Offset: 0x00010990
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
					Noose_HQ_To_Maze_Bank.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Noose_HQ_To_Maze_Bank.prota = new Vehicle("FBI2", NOOSEHeadquarters.sPos_prota, NOOSEHeadquarters.he_prota);
					goto IL_AC;
				}
			}
			Noose_HQ_To_Maze_Bank.prota = new Vehicle("FBI2", NOOSEHeadquarters.sPos_prota, NOOSEHeadquarters.he_prota);
			IL_AC:
			if (Noose_HQ_To_Maze_Bank.prota == null || !Noose_HQ_To_Maze_Bank.prota.IsValid() || !EntityExtensions.Exists(Noose_HQ_To_Maze_Bank.prota))
			{
				Noose_HQ_To_Maze_Bank.prota = new Vehicle("FBI2", NOOSEHeadquarters.sPos_prota, NOOSEHeadquarters.he_prota);
			}
			EManager.protaM = Noose_HQ_To_Maze_Bank.prota;
			this.blBlip.Delete();
			Noose_HQ_To_Maze_Bank.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Noose_HQ_To_Maze_Bank.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Noose_HQ_To_Maze_Bank.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				NOOSEHeadquarters.sPos_prota.X,
				NOOSEHeadquarters.sPos_prota.Y,
				NOOSEHeadquarters.sPos_prota.Z,
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
			Noose_HQ_To_Maze_Bank.vip = new Ped(EManager.vipModel, NOOSEHeadquarters.wPos_vip, NOOSEHeadquarters.he_vip);
			Noose_HQ_To_Maze_Bank.vip.MakePersistent();
			Noose_HQ_To_Maze_Bank.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			Noose_HQ_To_Maze_Bank.vip.IsVisible = true;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Noose_HQ_To_Maze_Bank.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Noose_HQ_To_Maze_Bank.p_fbi1.RelationshipGroup = "POLICE";
			Noose_HQ_To_Maze_Bank.p_fbi2.RelationshipGroup = "POLICE";
			Noose_HQ_To_Maze_Bank.p_limo.RelationshipGroup = "POLICE";
			Noose_HQ_To_Maze_Bank.vip.RelationshipGroup = "POLICE";
			Noose_HQ_To_Maze_Bank.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Noose_HQ_To_Maze_Bank.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Noose_HQ_To_Maze_Bank.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Game.SetRelationshipBetweenRelationshipGroups(Noose_HQ_To_Maze_Bank.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Noose_HQ_To_Maze_Bank.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Noose_HQ_To_Maze_Bank.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Noose_HQ_To_Maze_Bank.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Noose_HQ_To_Maze_Bank.p_fbi1.RelationshipGroup, "COP", 1);
			Noose_HQ_To_Maze_Bank.p_limo.CanAttackFriendlies = false;
			Noose_HQ_To_Maze_Bank.p_fbi1.CanAttackFriendlies = false;
			Noose_HQ_To_Maze_Bank.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Noose_HQ_To_Maze_Bank.arBlip = new Blip(MazeBank.sPos_prota);
			Noose_HQ_To_Maze_Bank.arBlip.Color = Color.Red;
			Noose_HQ_To_Maze_Bank.arBlip.EnableRoute(Color.Red);
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
				Noose_HQ_To_Maze_Bank.p_limo,
				this.limo,
				Noose_HQ_To_Maze_Bank.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Noose_HQ_To_Maze_Bank.p_fbi1,
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
				Noose_HQ_To_Maze_Bank.p_fbi2,
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
			Noose_HQ_To_Maze_Bank.arBlip.Delete();
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
				Noose_HQ_To_Maze_Bank.p_limo.Tasks.ParkVehicle(MazeBank.sPos_limo, MazeBank.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Noose_HQ_To_Maze_Bank.p_fbi1.Tasks.ParkVehicle(MazeBank.sPos_fbi1, MazeBank.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Noose_HQ_To_Maze_Bank.p_fbi2.Tasks.ParkVehicle(MazeBank.sPos_fbi2, MazeBank.he_fbi2);
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
					Noose_HQ_To_Maze_Bank.p_limo.Tasks.Clear();
					Noose_HQ_To_Maze_Bank.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Noose_HQ_To_Maze_Bank.vip.Tasks.FollowNavigationMeshToPosition(MazeBank.wPos_vip, MazeBank.he_vip - 180f, 0.5f).WaitForCompletion();
				Noose_HQ_To_Maze_Bank.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BCA;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000134A0 File Offset: 0x000116A0
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
				Noose_HQ_To_Maze_Bank.arBlip.Delete();
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
				Noose_HQ_To_Maze_Bank.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Noose_HQ_To_Maze_Bank.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Noose_HQ_To_Maze_Bank.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Noose_HQ_To_Maze_Bank.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x040001BD RID: 445
		private Vector3 SpawnPoint;

		// Token: 0x040001BE RID: 446
		internal static Ped p_limo;

		// Token: 0x040001BF RID: 447
		internal static Ped vip;

		// Token: 0x040001C0 RID: 448
		internal static Ped p_fbi1;

		// Token: 0x040001C1 RID: 449
		internal static Ped p_fbi2;

		// Token: 0x040001C2 RID: 450
		private Blip blBlip;

		// Token: 0x040001C3 RID: 451
		internal static Blip arBlip;

		// Token: 0x040001C4 RID: 452
		private Blip b_limo;

		// Token: 0x040001C5 RID: 453
		private Blip b_fbi1;

		// Token: 0x040001C6 RID: 454
		private Blip b_fbi2;

		// Token: 0x040001C7 RID: 455
		private float distance;

		// Token: 0x040001C8 RID: 456
		internal static Vehicle prota;

		// Token: 0x040001C9 RID: 457
		private bool isCalloutAccepted;

		// Token: 0x040001CA RID: 458
		private Vehicle limo = EManager.limoM;

		// Token: 0x040001CB RID: 459
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x040001CC RID: 460
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
