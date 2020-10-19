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
	// Token: 0x0200001F RID: 31
	[CalloutInfo("Airport to Arcadius Center", 3)]
	public class Airport_to_Arcadius_Center : Callout
	{
		// Token: 0x0600007A RID: 122 RVA: 0x0000B9D0 File Offset: 0x00009BD0
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = LSAirport.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, LSAirport.sPos_limo, LSAirport.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, LSAirport.sPos_fbi1, LSAirport.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, LSAirport.sPos_fbi2, LSAirport.he_fbi2);
			Airport_to_Arcadius_Center.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Airport_to_Arcadius_Center.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Airport_to_Arcadius_Center.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Airport_to_Arcadius_Center.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Airport_to_Arcadius_Center.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Airport_to_Arcadius_Center.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Airport_to_Arcadius_Center.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Arcadius_Center.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Arcadius_Center.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : LS Airport to Arcadius Center";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000BB54 File Offset: 0x00009D54
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Los Santos Airport~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.LSAtoARC;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(ArcadiusCenter.sPos_limo, this.SpawnPoint);
			EManager.departure = LSAirport.sPos_limo;
			EManager.arrival = ArcadiusCenter.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Arcadius Center";
			EManager.currentCallState = EManager.CallState.Initiated;
			this.g = new Vehicle("LUXOR", new Vector3(-978f, -2997.384f, 14.548f), 59.197f);
			this.g.IsEngineOn = true;
			return base.OnCalloutAccepted();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000BC38 File Offset: 0x00009E38
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Airport_to_Arcadius_Center.p_fbi1.Delete();
			Airport_to_Arcadius_Center.p_fbi2.Delete();
			Airport_to_Arcadius_Center.p_limo.Delete();
			try
			{
				this.g.Delete();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000BCA8 File Offset: 0x00009EA8
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
					Airport_to_Arcadius_Center.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Airport_to_Arcadius_Center.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
					goto IL_AC;
				}
			}
			Airport_to_Arcadius_Center.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			IL_AC:
			if (Airport_to_Arcadius_Center.prota == null || !Airport_to_Arcadius_Center.prota.IsValid() || !EntityExtensions.Exists(Airport_to_Arcadius_Center.prota))
			{
				Airport_to_Arcadius_Center.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			}
			EManager.protaM = Airport_to_Arcadius_Center.prota;
			this.blBlip.Delete();
			Airport_to_Arcadius_Center.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Airport_to_Arcadius_Center.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Airport_to_Arcadius_Center.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
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
			Airport_to_Arcadius_Center.vip = new Ped(EManager.vipModel, LSAirport.wPos_vip, LSAirport.he_vip);
			Airport_to_Arcadius_Center.vip.MakePersistent();
			Airport_to_Arcadius_Center.vip.WarpIntoVehicle(this.g, 1);
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			Game.FrameRender += EManager.Game_FrameRenderRes;
			GameFiber.StartNew(new ThreadStart(new EManager().ManageRiskLevel));
			this.g.IsEngineOn = false;
			Game.DisplaySubtitle("Wait for the VIP to take his seat.", 5000);
			GameFiber.Sleep(4000);
			Airport_to_Arcadius_Center.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 2f, this.limo.Heading, 0f);
			Airport_to_Arcadius_Center.vip.IsVisible = true;
			while (Vector3.Distance(Airport_to_Arcadius_Center.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Airport_to_Arcadius_Center.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Airport_to_Arcadius_Center.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Airport_to_Arcadius_Center.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Airport_to_Arcadius_Center.p_fbi1.RelationshipGroup = "POLICE";
			Airport_to_Arcadius_Center.p_fbi2.RelationshipGroup = "POLICE";
			Airport_to_Arcadius_Center.p_limo.RelationshipGroup = "POLICE";
			Airport_to_Arcadius_Center.vip.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Arcadius_Center.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Airport_to_Arcadius_Center.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Airport_to_Arcadius_Center.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Airport_to_Arcadius_Center.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Arcadius_Center.p_fbi1.RelationshipGroup, "COP", 1);
			Airport_to_Arcadius_Center.p_limo.CanAttackFriendlies = false;
			Airport_to_Arcadius_Center.p_fbi1.CanAttackFriendlies = false;
			Airport_to_Arcadius_Center.p_fbi2.CanAttackFriendlies = false;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~Arcadius Center~w~.", 6000);
			Game.FrameRender += EManager.Game_FrameRender;
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Airport_to_Arcadius_Center.arBlip = new Blip(ArcadiusCenter.sPos_prota);
			Airport_to_Arcadius_Center.arBlip.Color = Color.Red;
			Airport_to_Arcadius_Center.arBlip.EnableRoute(Color.Red);
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
				Airport_to_Arcadius_Center.p_limo,
				this.limo,
				Airport_to_Arcadius_Center.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Airport_to_Arcadius_Center.p_fbi1,
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
				Airport_to_Arcadius_Center.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", ArcadiusCenter.sPos_limo, ArcadiusCenter.he_limo);
			this.g.Delete();
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, ArcadiusCenter.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, ArcadiusCenter.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Airport_to_Arcadius_Center.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
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
					50,
					0
				});
				vector..ctor(ArcadiusCenter.sPos_prota.X, ArcadiusCenter.sPos_prota.Y, ArcadiusCenter.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					ArcadiusCenter.sPos_limo.X,
					ArcadiusCenter.sPos_limo.Y,
					ArcadiusCenter.sPos_limo.Z,
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
				vector..ctor(ArcadiusCenter.sPos_limo.X, ArcadiusCenter.sPos_limo.Y, ArcadiusCenter.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Airport_to_Arcadius_Center.p_limo.Tasks.ParkVehicle(ArcadiusCenter.sPos_limo, ArcadiusCenter.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Airport_to_Arcadius_Center.p_fbi1.Tasks.ParkVehicle(ArcadiusCenter.sPos_fbi1, ArcadiusCenter.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Airport_to_Arcadius_Center.p_fbi2.Tasks.ParkVehicle(ArcadiusCenter.sPos_fbi2, ArcadiusCenter.he_fbi2);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi2 couldn't park.");
			}
			Game.FrameRender -= EManager.Mod_Controls2;
			if (!EManager.isPlayerDrivingLimo)
			{
				Airport_to_Arcadius_Center.p_limo.Tasks.Clear();
				Airport_to_Arcadius_Center.p_limo.Tasks.LeaveVehicle(256);
			}
			Airport_to_Arcadius_Center.arBlip = new Blip(ArcadiusCenter.sPos_prota);
			Airport_to_Arcadius_Center.arBlip.Color = Color.Red;
			Airport_to_Arcadius_Center.arBlip.EnableRoute(Color.Red);
			EManager.currentCallState = EManager.CallState.Arrived;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, vector) > 4f)
			{
				GameFiber.Yield();
			}
			GameFiber.Sleep(3000);
			NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
			{
				num2
			});
			Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
			Airport_to_Arcadius_Center.vip.Tasks.FollowNavigationMeshToPosition(ArcadiusCenter.wPos_vip, ArcadiusCenter.he_vip - 180f, 0.5f).WaitForCompletion();
			Airport_to_Arcadius_Center.vip.Delete();
			Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
			this.End();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000C9DC File Offset: 0x0000ABDC
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
				Airport_to_Arcadius_Center.arBlip.Delete();
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
				Airport_to_Arcadius_Center.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Arcadius_Center.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Arcadius_Center.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Arcadius_Center.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x04000158 RID: 344
		private Vector3 SpawnPoint;

		// Token: 0x04000159 RID: 345
		internal static Ped p_limo;

		// Token: 0x0400015A RID: 346
		internal static Ped vip;

		// Token: 0x0400015B RID: 347
		internal static Ped p_fbi1;

		// Token: 0x0400015C RID: 348
		internal static Ped p_fbi2;

		// Token: 0x0400015D RID: 349
		private Blip blBlip;

		// Token: 0x0400015E RID: 350
		internal static Blip arBlip;

		// Token: 0x0400015F RID: 351
		private Blip b_limo;

		// Token: 0x04000160 RID: 352
		private Blip b_fbi1;

		// Token: 0x04000161 RID: 353
		private Blip b_fbi2;

		// Token: 0x04000162 RID: 354
		private float distance;

		// Token: 0x04000163 RID: 355
		internal static Vehicle prota;

		// Token: 0x04000164 RID: 356
		private bool isCalloutAccepted;

		// Token: 0x04000165 RID: 357
		private Vehicle g;

		// Token: 0x04000166 RID: 358
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000167 RID: 359
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x04000168 RID: 360
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
