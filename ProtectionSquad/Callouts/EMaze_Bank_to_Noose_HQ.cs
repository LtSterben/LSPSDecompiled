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
	// Token: 0x02000024 RID: 36
	[CalloutInfo("Maze Bank to Noose HQ", 3)]
	public class EMaze_Bank_to_Noose_HQ : Callout
	{
		// Token: 0x06000098 RID: 152 RVA: 0x000113E0 File Offset: 0x0000F5E0
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = MazeBank.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, MazeBank.sPos_limo, MazeBank.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, MazeBank.sPos_fbi1, MazeBank.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, MazeBank.sPos_fbi2, MazeBank.he_fbi2);
			EMaze_Bank_to_Noose_HQ.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			EMaze_Bank_to_Noose_HQ.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			EMaze_Bank_to_Noose_HQ.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			EMaze_Bank_to_Noose_HQ.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			EMaze_Bank_to_Noose_HQ.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			EMaze_Bank_to_Noose_HQ.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(EMaze_Bank_to_Noose_HQ.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(EMaze_Bank_to_Noose_HQ.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(EMaze_Bank_to_Noose_HQ.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Maze Bank to NOOSE Headquarters";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00011564 File Offset: 0x0000F764
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Maze Bank~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.MZBtoNOO;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(NOOSEHeadquarters.sPos_limo, this.SpawnPoint);
			EManager.departure = MazeBank.sPos_limo;
			EManager.arrival = NOOSEHeadquarters.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "NOOSE Headquarters";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0001160C File Offset: 0x0000F80C
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			EMaze_Bank_to_Noose_HQ.p_fbi1.Delete();
			EMaze_Bank_to_Noose_HQ.p_fbi2.Delete();
			EMaze_Bank_to_Noose_HQ.p_limo.Delete();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0001165C File Offset: 0x0000F85C
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
					EMaze_Bank_to_Noose_HQ.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					EMaze_Bank_to_Noose_HQ.prota = new Vehicle("FBI2", MazeBank.sPos_prota, MazeBank.he_prota);
					goto IL_AC;
				}
			}
			EMaze_Bank_to_Noose_HQ.prota = new Vehicle("FBI2", MazeBank.sPos_prota, MazeBank.he_prota);
			IL_AC:
			if (EMaze_Bank_to_Noose_HQ.prota == null || !EMaze_Bank_to_Noose_HQ.prota.IsValid() || !EntityExtensions.Exists(EMaze_Bank_to_Noose_HQ.prota))
			{
				EMaze_Bank_to_Noose_HQ.prota = new Vehicle("FBI2", MazeBank.sPos_prota, MazeBank.he_prota);
			}
			EManager.protaM = EMaze_Bank_to_Noose_HQ.prota;
			EMaze_Bank_to_Noose_HQ.vip = new Ped(EManager.vipModel, MazeBank.wPos_vip, MazeBank.he_vip);
			EMaze_Bank_to_Noose_HQ.vip.MakePersistent();
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			this.blBlip.Delete();
			EMaze_Bank_to_Noose_HQ.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			EMaze_Bank_to_Noose_HQ.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			EMaze_Bank_to_Noose_HQ.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
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
			EMaze_Bank_to_Noose_HQ.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(EMaze_Bank_to_Noose_HQ.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			EMaze_Bank_to_Noose_HQ.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			EMaze_Bank_to_Noose_HQ.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			EMaze_Bank_to_Noose_HQ.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			EMaze_Bank_to_Noose_HQ.p_fbi1.RelationshipGroup = "POLICE";
			EMaze_Bank_to_Noose_HQ.p_fbi2.RelationshipGroup = "POLICE";
			EMaze_Bank_to_Noose_HQ.p_limo.RelationshipGroup = "POLICE";
			EMaze_Bank_to_Noose_HQ.vip.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(EMaze_Bank_to_Noose_HQ.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, EMaze_Bank_to_Noose_HQ.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", EMaze_Bank_to_Noose_HQ.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", EMaze_Bank_to_Noose_HQ.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(EMaze_Bank_to_Noose_HQ.p_fbi1.RelationshipGroup, "COP", 1);
			EMaze_Bank_to_Noose_HQ.p_limo.CanAttackFriendlies = false;
			EMaze_Bank_to_Noose_HQ.p_fbi1.CanAttackFriendlies = false;
			EMaze_Bank_to_Noose_HQ.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			EMaze_Bank_to_Noose_HQ.arBlip = new Blip(NOOSEHeadquarters.sPos_prota);
			EMaze_Bank_to_Noose_HQ.arBlip.Color = Color.Red;
			EMaze_Bank_to_Noose_HQ.arBlip.EnableRoute(Color.Red);
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
				EMaze_Bank_to_Noose_HQ.p_limo,
				this.limo,
				EMaze_Bank_to_Noose_HQ.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				EMaze_Bank_to_Noose_HQ.p_fbi1,
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
				EMaze_Bank_to_Noose_HQ.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", NOOSEHeadquarters.sPos_limo, NOOSEHeadquarters.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, NOOSEHeadquarters.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, NOOSEHeadquarters.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			EMaze_Bank_to_Noose_HQ.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
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
					50,
					0
				});
				vector..ctor(NOOSEHeadquarters.sPos_prota.X, NOOSEHeadquarters.sPos_prota.Y, NOOSEHeadquarters.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					NOOSEHeadquarters.sPos_limo.X,
					NOOSEHeadquarters.sPos_limo.Y,
					NOOSEHeadquarters.sPos_limo.Z,
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
				vector..ctor(NOOSEHeadquarters.sPos_limo.X, NOOSEHeadquarters.sPos_limo.Y, NOOSEHeadquarters.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				EMaze_Bank_to_Noose_HQ.p_limo.Tasks.ParkVehicle(NOOSEHeadquarters.sPos_limo, NOOSEHeadquarters.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				EMaze_Bank_to_Noose_HQ.p_fbi1.Tasks.ParkVehicle(NOOSEHeadquarters.sPos_fbi1, NOOSEHeadquarters.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				EMaze_Bank_to_Noose_HQ.p_fbi2.Tasks.ParkVehicle(NOOSEHeadquarters.sPos_fbi2, NOOSEHeadquarters.he_fbi2);
				goto IL_BC4;
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi2 couldn't park.");
				goto IL_BC4;
			}
			IL_BBF:
			GameFiber.Yield();
			IL_BC4:
			if (Vector3.Distance(Game.LocalPlayer.Character.Position, vector) <= 4f)
			{
				Game.FrameRender -= EManager.Mod_Controls2;
				GameFiber.Sleep(2000);
				NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
				{
					num2
				});
				while (Vector3.Distance(this.limo.Position, NOOSEHeadquarters.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					EMaze_Bank_to_Noose_HQ.p_limo.Tasks.Clear();
					EMaze_Bank_to_Noose_HQ.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				EMaze_Bank_to_Noose_HQ.vip.Tasks.FollowNavigationMeshToPosition(NOOSEHeadquarters.wPos_vip, NOOSEHeadquarters.he_vip - 180f, 0.5f).WaitForCompletion();
				EMaze_Bank_to_Noose_HQ.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BBF;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00012360 File Offset: 0x00010560
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
				EMaze_Bank_to_Noose_HQ.arBlip.Delete();
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
				EMaze_Bank_to_Noose_HQ.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				EMaze_Bank_to_Noose_HQ.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				EMaze_Bank_to_Noose_HQ.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				EMaze_Bank_to_Noose_HQ.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x040001AD RID: 429
		private Vector3 SpawnPoint;

		// Token: 0x040001AE RID: 430
		internal static Ped p_limo;

		// Token: 0x040001AF RID: 431
		internal static Ped vip;

		// Token: 0x040001B0 RID: 432
		internal static Ped p_fbi1;

		// Token: 0x040001B1 RID: 433
		internal static Ped p_fbi2;

		// Token: 0x040001B2 RID: 434
		private Blip blBlip;

		// Token: 0x040001B3 RID: 435
		internal static Blip arBlip;

		// Token: 0x040001B4 RID: 436
		private Blip b_limo;

		// Token: 0x040001B5 RID: 437
		private Blip b_fbi1;

		// Token: 0x040001B6 RID: 438
		private Blip b_fbi2;

		// Token: 0x040001B7 RID: 439
		private float distance;

		// Token: 0x040001B8 RID: 440
		internal static Vehicle prota;

		// Token: 0x040001B9 RID: 441
		private bool isCalloutAccepted;

		// Token: 0x040001BA RID: 442
		private Vehicle limo = EManager.limoM;

		// Token: 0x040001BB RID: 443
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x040001BC RID: 444
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
