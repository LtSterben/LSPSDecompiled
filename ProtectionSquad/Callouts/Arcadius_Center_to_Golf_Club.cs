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
	// Token: 0x0200001B RID: 27
	[CalloutInfo("Arcadius Center to Golf Club", 3)]
	public class Arcadius_Center_to_Golf_Club : Callout
	{
		// Token: 0x06000062 RID: 98 RVA: 0x000074E4 File Offset: 0x000056E4
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = ArcadiusCenter.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, ArcadiusCenter.sPos_limo, ArcadiusCenter.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, ArcadiusCenter.sPos_fbi1, ArcadiusCenter.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, ArcadiusCenter.sPos_fbi2, ArcadiusCenter.he_fbi2);
			Arcadius_Center_to_Golf_Club.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Arcadius_Center_to_Golf_Club.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Arcadius_Center_to_Golf_Club.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Arcadius_Center_to_Golf_Club.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Arcadius_Center_to_Golf_Club.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Arcadius_Center_to_Golf_Club.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Arcadius_Center_to_Golf_Club.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Arcadius_Center_to_Golf_Club.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Arcadius_Center_to_Golf_Club.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Arcadius Center to Golf Club";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00007668 File Offset: 0x00005868
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Arcadius Center~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.ARCtoGOC;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(GolfClub.sPos_limo, this.SpawnPoint);
			EManager.departure = ArcadiusCenter.sPos_limo;
			EManager.arrival = GolfClub.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Los Santos Golf Club";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00007710 File Offset: 0x00005910
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Arcadius_Center_to_Golf_Club.p_fbi1.Delete();
			Arcadius_Center_to_Golf_Club.p_fbi2.Delete();
			Arcadius_Center_to_Golf_Club.p_limo.Delete();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00007760 File Offset: 0x00005960
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
					Arcadius_Center_to_Golf_Club.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Arcadius_Center_to_Golf_Club.prota = new Vehicle("FBI2", ArcadiusCenter.sPos_prota, ArcadiusCenter.he_prota);
					goto IL_AC;
				}
			}
			Arcadius_Center_to_Golf_Club.prota = new Vehicle("FBI2", ArcadiusCenter.sPos_prota, ArcadiusCenter.he_prota);
			IL_AC:
			if (Arcadius_Center_to_Golf_Club.prota == null || !Arcadius_Center_to_Golf_Club.prota.IsValid() || !EntityExtensions.Exists(Arcadius_Center_to_Golf_Club.prota))
			{
				Arcadius_Center_to_Golf_Club.prota = new Vehicle("FBI2", ArcadiusCenter.sPos_prota, ArcadiusCenter.he_prota);
			}
			EManager.protaM = Arcadius_Center_to_Golf_Club.prota;
			this.blBlip.Delete();
			Arcadius_Center_to_Golf_Club.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Arcadius_Center_to_Golf_Club.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Arcadius_Center_to_Golf_Club.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				ArcadiusCenter.sPos_prota.X,
				ArcadiusCenter.sPos_prota.Y,
				ArcadiusCenter.sPos_prota.Z,
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
			Arcadius_Center_to_Golf_Club.vip = new Ped(EManager.vipModel, ArcadiusCenter.wPos_vip, ArcadiusCenter.he_vip);
			Arcadius_Center_to_Golf_Club.vip.MakePersistent();
			Arcadius_Center_to_Golf_Club.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			Arcadius_Center_to_Golf_Club.vip.IsVisible = true;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Arcadius_Center_to_Golf_Club.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Arcadius_Center_to_Golf_Club.p_fbi1.RelationshipGroup = "POLICE";
			Arcadius_Center_to_Golf_Club.p_fbi2.RelationshipGroup = "POLICE";
			Arcadius_Center_to_Golf_Club.p_limo.RelationshipGroup = "POLICE";
			Arcadius_Center_to_Golf_Club.vip.RelationshipGroup = "POLICE";
			Arcadius_Center_to_Golf_Club.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Arcadius_Center_to_Golf_Club.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Arcadius_Center_to_Golf_Club.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Game.SetRelationshipBetweenRelationshipGroups(Arcadius_Center_to_Golf_Club.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Arcadius_Center_to_Golf_Club.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Arcadius_Center_to_Golf_Club.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Arcadius_Center_to_Golf_Club.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Arcadius_Center_to_Golf_Club.p_fbi1.RelationshipGroup, "COP", 1);
			Arcadius_Center_to_Golf_Club.p_limo.CanAttackFriendlies = false;
			Arcadius_Center_to_Golf_Club.p_fbi1.CanAttackFriendlies = false;
			Arcadius_Center_to_Golf_Club.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Arcadius_Center_to_Golf_Club.arBlip = new Blip(GolfClub.sPos_prota);
			Arcadius_Center_to_Golf_Club.arBlip.Color = Color.Red;
			Arcadius_Center_to_Golf_Club.arBlip.EnableRoute(Color.Red);
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
				Arcadius_Center_to_Golf_Club.p_limo,
				this.limo,
				Arcadius_Center_to_Golf_Club.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Arcadius_Center_to_Golf_Club.p_fbi1,
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
				Arcadius_Center_to_Golf_Club.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", GolfClub.sPos_limo, GolfClub.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, GolfClub.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, GolfClub.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Arcadius_Center_to_Golf_Club.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					GolfClub.sPos_prota.X,
					GolfClub.sPos_prota.Y,
					GolfClub.sPos_prota.Z,
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
				vector..ctor(GolfClub.sPos_prota.X, GolfClub.sPos_prota.Y, GolfClub.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					GolfClub.sPos_limo.X,
					GolfClub.sPos_limo.Y,
					GolfClub.sPos_limo.Z,
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
				vector..ctor(GolfClub.sPos_limo.X, GolfClub.sPos_limo.Y, GolfClub.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Arcadius_Center_to_Golf_Club.p_limo.Tasks.ParkVehicle(GolfClub.sPos_limo, GolfClub.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Arcadius_Center_to_Golf_Club.p_fbi1.Tasks.ParkVehicle(GolfClub.sPos_fbi1, GolfClub.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Arcadius_Center_to_Golf_Club.p_fbi2.Tasks.ParkVehicle(GolfClub.sPos_fbi2, GolfClub.he_fbi2);
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
				while (Vector3.Distance(this.limo.Position, GolfClub.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					Arcadius_Center_to_Golf_Club.p_limo.Tasks.Clear();
					Arcadius_Center_to_Golf_Club.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Arcadius_Center_to_Golf_Club.vip.Tasks.FollowNavigationMeshToPosition(GolfClub.wPos_vip, GolfClub.he_vip - 180f, 0.5f).WaitForCompletion();
				Arcadius_Center_to_Golf_Club.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BCA;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00008470 File Offset: 0x00006670
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
				Arcadius_Center_to_Golf_Club.arBlip.Delete();
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
				Arcadius_Center_to_Golf_Club.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Arcadius_Center_to_Golf_Club.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Arcadius_Center_to_Golf_Club.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Arcadius_Center_to_Golf_Club.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x04000118 RID: 280
		private Vector3 SpawnPoint;

		// Token: 0x04000119 RID: 281
		internal static Ped p_limo;

		// Token: 0x0400011A RID: 282
		internal static Ped vip;

		// Token: 0x0400011B RID: 283
		internal static Ped p_fbi1;

		// Token: 0x0400011C RID: 284
		internal static Ped p_fbi2;

		// Token: 0x0400011D RID: 285
		private Blip blBlip;

		// Token: 0x0400011E RID: 286
		internal static Blip arBlip;

		// Token: 0x0400011F RID: 287
		private Blip b_limo;

		// Token: 0x04000120 RID: 288
		private Blip b_fbi1;

		// Token: 0x04000121 RID: 289
		private Blip b_fbi2;

		// Token: 0x04000122 RID: 290
		private float distance;

		// Token: 0x04000123 RID: 291
		internal static Vehicle prota;

		// Token: 0x04000124 RID: 292
		private bool isCalloutAccepted;

		// Token: 0x04000125 RID: 293
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000126 RID: 294
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x04000127 RID: 295
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
