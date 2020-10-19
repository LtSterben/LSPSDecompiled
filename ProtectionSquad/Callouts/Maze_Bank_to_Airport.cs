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
	// Token: 0x02000021 RID: 33
	[CalloutInfo("Maze Bank to Airport", 3)]
	public class Maze_Bank_to_Airport : Callout
	{
		// Token: 0x06000086 RID: 134 RVA: 0x0000DD34 File Offset: 0x0000BF34
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = LSAirport.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, LSAirport.sPos_limo, LSAirport.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, LSAirport.sPos_fbi1, LSAirport.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, LSAirport.sPos_fbi2, LSAirport.he_fbi2);
			Maze_Bank_to_Airport.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Maze_Bank_to_Airport.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Maze_Bank_to_Airport.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Maze_Bank_to_Airport.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Maze_Bank_to_Airport.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Maze_Bank_to_Airport.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Maze_Bank_to_Airport.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Maze_Bank_to_Airport.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Maze_Bank_to_Airport.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Los Santos Airport to Maze Bank";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Los Santos Int. Airport.~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.LSAtoMZB;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(MazeBank.sPos_limo, this.SpawnPoint);
			EManager.departure = LSAirport.sPos_limo;
			EManager.arrival = MazeBank.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Maze Bank";
			EManager.currentCallState = EManager.CallState.Initiated;
			this.g = new Vehicle("LUXOR", new Vector3(-978f, -2997.384f, 14.548f), 59.197f);
			this.g.IsEngineOn = true;
			return base.OnCalloutAccepted();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000DF9C File Offset: 0x0000C19C
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Maze_Bank_to_Airport.p_fbi1.Delete();
			Maze_Bank_to_Airport.p_fbi2.Delete();
			Maze_Bank_to_Airport.p_limo.Delete();
			try
			{
				this.g.Delete();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000E00C File Offset: 0x0000C20C
		public override void Process()
		{
			base.Process();
			this.isCalloutAccepted = true;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 35f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("CLEAR_ALL_HELP_MESSAGES", Array.Empty<NativeArgument>());
			Maze_Bank_to_Airport.vip = new Ped(EManager.vipModel, LSAirport.wPos_vip, LSAirport.he_vip);
			Maze_Bank_to_Airport.vip.MakePersistent();
			Maze_Bank_to_Airport.vip.WarpIntoVehicle(this.g, 1);
			if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
			{
				try
				{
					Maze_Bank_to_Airport.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_E5;
				}
				catch (Exception)
				{
					Maze_Bank_to_Airport.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
					goto IL_E5;
				}
			}
			Maze_Bank_to_Airport.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			IL_E5:
			if (Maze_Bank_to_Airport.prota == null || !Maze_Bank_to_Airport.prota.IsValid() || !EntityExtensions.Exists(Maze_Bank_to_Airport.prota))
			{
				Maze_Bank_to_Airport.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			}
			EManager.protaM = Maze_Bank_to_Airport.prota;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			this.blBlip.Delete();
			Maze_Bank_to_Airport.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Maze_Bank_to_Airport.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Maze_Bank_to_Airport.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				LSAirport.sPos_prota.X,
				LSAirport.sPos_prota.Y,
				LSAirport.sPos_prota.Z,
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
			Maze_Bank_to_Airport.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Maze_Bank_to_Airport.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Maze_Bank_to_Airport.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Maze_Bank_to_Airport.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Maze_Bank_to_Airport.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Maze_Bank_to_Airport.p_fbi1.RelationshipGroup = "POLICE";
			Maze_Bank_to_Airport.p_fbi2.RelationshipGroup = "POLICE";
			Maze_Bank_to_Airport.p_limo.RelationshipGroup = "POLICE";
			Maze_Bank_to_Airport.vip.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(Maze_Bank_to_Airport.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Maze_Bank_to_Airport.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Maze_Bank_to_Airport.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Maze_Bank_to_Airport.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Maze_Bank_to_Airport.p_fbi1.RelationshipGroup, "COP", 1);
			Maze_Bank_to_Airport.p_limo.CanAttackFriendlies = false;
			Maze_Bank_to_Airport.p_fbi1.CanAttackFriendlies = false;
			Maze_Bank_to_Airport.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Maze_Bank_to_Airport.arBlip = new Blip(MazeBank.sPos_prota);
			Maze_Bank_to_Airport.arBlip.Color = Color.Red;
			Maze_Bank_to_Airport.arBlip.EnableRoute(Color.Red);
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
				Maze_Bank_to_Airport.p_limo,
				this.limo,
				Maze_Bank_to_Airport.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Maze_Bank_to_Airport.p_fbi1,
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
				Maze_Bank_to_Airport.p_fbi2,
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
			this.g.Delete();
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
			Maze_Bank_to_Airport.arBlip.Delete();
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
				Maze_Bank_to_Airport.p_limo.Tasks.ParkVehicle(MazeBank.sPos_limo, MazeBank.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Maze_Bank_to_Airport.p_fbi1.Tasks.ParkVehicle(MazeBank.sPos_fbi1, MazeBank.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Maze_Bank_to_Airport.p_fbi2.Tasks.ParkVehicle(MazeBank.sPos_fbi2, MazeBank.he_fbi2);
				goto IL_BE0;
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi2 couldn't park.");
				goto IL_BE0;
			}
			IL_BDB:
			GameFiber.Yield();
			IL_BE0:
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
					Maze_Bank_to_Airport.p_limo.Tasks.Clear();
					Maze_Bank_to_Airport.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Maze_Bank_to_Airport.vip.Tasks.FollowNavigationMeshToPosition(MazeBank.wPos_vip, MazeBank.he_vip - 180f, 0.5f).WaitForCompletion();
				Maze_Bank_to_Airport.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BDB;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000ED2C File Offset: 0x0000CF2C
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
				Maze_Bank_to_Airport.arBlip.Delete();
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
				Maze_Bank_to_Airport.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Maze_Bank_to_Airport.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Maze_Bank_to_Airport.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Maze_Bank_to_Airport.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x0400017A RID: 378
		private Vector3 SpawnPoint;

		// Token: 0x0400017B RID: 379
		internal static Ped p_limo;

		// Token: 0x0400017C RID: 380
		internal static Ped vip;

		// Token: 0x0400017D RID: 381
		internal static Ped p_fbi1;

		// Token: 0x0400017E RID: 382
		internal static Ped p_fbi2;

		// Token: 0x0400017F RID: 383
		private Blip blBlip;

		// Token: 0x04000180 RID: 384
		internal static Blip arBlip;

		// Token: 0x04000181 RID: 385
		private Blip b_limo;

		// Token: 0x04000182 RID: 386
		private Blip b_fbi1;

		// Token: 0x04000183 RID: 387
		private Blip b_fbi2;

		// Token: 0x04000184 RID: 388
		private float distance;

		// Token: 0x04000185 RID: 389
		internal static Vehicle prota;

		// Token: 0x04000186 RID: 390
		private bool isCalloutAccepted;

		// Token: 0x04000187 RID: 391
		private Vehicle g;

		// Token: 0x04000188 RID: 392
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000189 RID: 393
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x0400018A RID: 394
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
