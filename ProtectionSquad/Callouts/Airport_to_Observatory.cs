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
	// Token: 0x02000020 RID: 32
	[CalloutInfo("Airport to Observatory", 3)]
	public class Airport_to_Observatory : Callout
	{
		// Token: 0x06000080 RID: 128 RVA: 0x0000CB8C File Offset: 0x0000AD8C
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = LSAirport.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, LSAirport.sPos_limo, LSAirport.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, LSAirport.sPos_fbi1, LSAirport.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, LSAirport.sPos_fbi2, LSAirport.he_fbi2);
			Airport_to_Observatory.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Airport_to_Observatory.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Airport_to_Observatory.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Airport_to_Observatory.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Airport_to_Observatory.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Airport_to_Observatory.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Airport_to_Observatory.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Observatory.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Observatory.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Los Santos Airport to Galileo Observatory";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000CD10 File Offset: 0x0000AF10
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Los Santos Int. Airport.~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.LSAtoGAL;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(GalileoObservatory.sPos_limo, this.SpawnPoint);
			EManager.departure = LSAirport.sPos_limo;
			EManager.arrival = GalileoObservatory.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Galileo Observatory";
			EManager.currentCallState = EManager.CallState.Initiated;
			this.g = new Vehicle("LUXOR", new Vector3(-978f, -2997.384f, 14.548f), 59.197f);
			this.g.IsEngineOn = true;
			return base.OnCalloutAccepted();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000CDF4 File Offset: 0x0000AFF4
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Airport_to_Observatory.p_fbi1.Delete();
			Airport_to_Observatory.p_fbi2.Delete();
			Airport_to_Observatory.p_limo.Delete();
			try
			{
				this.g.Delete();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000CE64 File Offset: 0x0000B064
		public override void Process()
		{
			base.Process();
			this.isCalloutAccepted = true;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 35f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("CLEAR_ALL_HELP_MESSAGES", Array.Empty<NativeArgument>());
			Airport_to_Observatory.vip = new Ped(EManager.vipModel, LSAirport.wPos_vip, LSAirport.he_vip);
			Airport_to_Observatory.vip.MakePersistent();
			Airport_to_Observatory.vip.WarpIntoVehicle(this.g, 1);
			if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
			{
				try
				{
					Airport_to_Observatory.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_E5;
				}
				catch (Exception)
				{
					Airport_to_Observatory.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
					goto IL_E5;
				}
			}
			Airport_to_Observatory.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			IL_E5:
			if (Airport_to_Observatory.prota == null || !Airport_to_Observatory.prota.IsValid() || !EntityExtensions.Exists(Airport_to_Observatory.prota))
			{
				Airport_to_Observatory.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			}
			EManager.protaM = Airport_to_Observatory.prota;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			this.blBlip.Delete();
			Airport_to_Observatory.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Airport_to_Observatory.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Airport_to_Observatory.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
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
			Airport_to_Observatory.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Airport_to_Observatory.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Airport_to_Observatory.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Airport_to_Observatory.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Airport_to_Observatory.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Airport_to_Observatory.p_fbi1.RelationshipGroup = "POLICE";
			Airport_to_Observatory.p_fbi2.RelationshipGroup = "POLICE";
			Airport_to_Observatory.p_limo.RelationshipGroup = "POLICE";
			Airport_to_Observatory.vip.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Observatory.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Airport_to_Observatory.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Airport_to_Observatory.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Airport_to_Observatory.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Observatory.p_fbi1.RelationshipGroup, "COP", 1);
			Airport_to_Observatory.p_limo.CanAttackFriendlies = false;
			Airport_to_Observatory.p_fbi1.CanAttackFriendlies = false;
			Airport_to_Observatory.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Airport_to_Observatory.arBlip = new Blip(GalileoObservatory.sPos_prota);
			Airport_to_Observatory.arBlip.Color = Color.Red;
			Airport_to_Observatory.arBlip.EnableRoute(Color.Red);
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
				Airport_to_Observatory.p_limo,
				this.limo,
				Airport_to_Observatory.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Airport_to_Observatory.p_fbi1,
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
				Airport_to_Observatory.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", GalileoObservatory.sPos_limo, GalileoObservatory.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, GalileoObservatory.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, GalileoObservatory.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Airport_to_Observatory.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					GalileoObservatory.sPos_prota.X,
					GalileoObservatory.sPos_prota.Y,
					GalileoObservatory.sPos_prota.Z,
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
				vector..ctor(GalileoObservatory.sPos_prota.X, GalileoObservatory.sPos_prota.Y, GalileoObservatory.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					GalileoObservatory.sPos_limo.X,
					GalileoObservatory.sPos_limo.Y,
					GalileoObservatory.sPos_limo.Z,
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
				vector..ctor(GalileoObservatory.sPos_limo.X, GalileoObservatory.sPos_limo.Y, GalileoObservatory.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Airport_to_Observatory.p_limo.Tasks.ParkVehicle(GalileoObservatory.sPos_limo, GalileoObservatory.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Airport_to_Observatory.p_fbi1.Tasks.ParkVehicle(GalileoObservatory.sPos_fbi1, GalileoObservatory.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Airport_to_Observatory.p_fbi2.Tasks.ParkVehicle(GalileoObservatory.sPos_fbi2, GalileoObservatory.he_fbi2);
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
				while (Vector3.Distance(this.limo.Position, GalileoObservatory.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					Airport_to_Observatory.p_limo.Tasks.Clear();
					Airport_to_Observatory.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Airport_to_Observatory.vip.Tasks.FollowNavigationMeshToPosition(GalileoObservatory.wPos_vip, GalileoObservatory.he_vip - 180f, 0.5f).WaitForCompletion();
				Airport_to_Observatory.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BDB;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000DB84 File Offset: 0x0000BD84
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
				Airport_to_Observatory.arBlip.Delete();
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
				Airport_to_Observatory.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Observatory.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Observatory.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Observatory.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x04000169 RID: 361
		private Vector3 SpawnPoint;

		// Token: 0x0400016A RID: 362
		internal static Ped p_limo;

		// Token: 0x0400016B RID: 363
		internal static Ped vip;

		// Token: 0x0400016C RID: 364
		internal static Ped p_fbi1;

		// Token: 0x0400016D RID: 365
		internal static Ped p_fbi2;

		// Token: 0x0400016E RID: 366
		private Blip blBlip;

		// Token: 0x0400016F RID: 367
		internal static Blip arBlip;

		// Token: 0x04000170 RID: 368
		private Blip b_limo;

		// Token: 0x04000171 RID: 369
		private Blip b_fbi1;

		// Token: 0x04000172 RID: 370
		private Blip b_fbi2;

		// Token: 0x04000173 RID: 371
		private float distance;

		// Token: 0x04000174 RID: 372
		internal static Vehicle prota;

		// Token: 0x04000175 RID: 373
		private bool isCalloutAccepted;

		// Token: 0x04000176 RID: 374
		private Vehicle g;

		// Token: 0x04000177 RID: 375
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000178 RID: 376
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x04000179 RID: 377
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
