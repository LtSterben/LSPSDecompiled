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
	// Token: 0x02000023 RID: 35
	[CalloutInfo("Airport to Union Depository", 3)]
	public class Airport_to_Union_Depository : Callout
	{
		// Token: 0x06000092 RID: 146 RVA: 0x00010238 File Offset: 0x0000E438
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = LSAirport.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, LSAirport.sPos_limo, LSAirport.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, LSAirport.sPos_fbi1, LSAirport.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, LSAirport.sPos_fbi2, LSAirport.he_fbi2);
			Airport_to_Union_Depository.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Airport_to_Union_Depository.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Airport_to_Union_Depository.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Airport_to_Union_Depository.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Airport_to_Union_Depository.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Airport_to_Union_Depository.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Airport_to_Union_Depository.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Union_Depository.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Airport_to_Union_Depository.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : LS Airport to Union Depository";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000103BC File Offset: 0x0000E5BC
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Los Santos Airport~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.LSAtoUD;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(UnionDepository.sPos_limo, this.SpawnPoint);
			EManager.departure = LSAirport.sPos_limo;
			EManager.arrival = UnionDepository.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Union Depository";
			EManager.currentCallState = EManager.CallState.Initiated;
			this.g = new Vehicle("LUXOR", new Vector3(-978f, -2997.384f, 14.548f), 59.197f);
			this.g.IsEngineOn = true;
			return base.OnCalloutAccepted();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000104A0 File Offset: 0x0000E6A0
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Airport_to_Union_Depository.p_fbi1.Delete();
			Airport_to_Union_Depository.p_fbi2.Delete();
			Airport_to_Union_Depository.p_limo.Delete();
			try
			{
				this.g.Delete();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00010510 File Offset: 0x0000E710
		public override void Process()
		{
			base.Process();
			this.isCalloutAccepted = true;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 35f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("CLEAR_ALL_HELP_MESSAGES", Array.Empty<NativeArgument>());
			Airport_to_Union_Depository.vip = new Ped(EManager.vipModel, LSAirport.wPos_vip, LSAirport.he_vip);
			Airport_to_Union_Depository.vip.MakePersistent();
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			Airport_to_Union_Depository.vip.WarpIntoVehicle(this.g, 1);
			if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
			{
				try
				{
					Airport_to_Union_Depository.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_FB;
				}
				catch (Exception)
				{
					Airport_to_Union_Depository.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
					goto IL_FB;
				}
			}
			Airport_to_Union_Depository.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			IL_FB:
			if (Airport_to_Union_Depository.prota == null || !Airport_to_Union_Depository.prota.IsValid() || !EntityExtensions.Exists(Airport_to_Union_Depository.prota))
			{
				Airport_to_Union_Depository.prota = new Vehicle("FBI2", LSAirport.sPos_prota, LSAirport.he_prota);
			}
			EManager.protaM = Airport_to_Union_Depository.prota;
			this.blBlip.Delete();
			Airport_to_Union_Depository.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Airport_to_Union_Depository.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Airport_to_Union_Depository.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
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
			Airport_to_Union_Depository.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Airport_to_Union_Depository.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Airport_to_Union_Depository.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Airport_to_Union_Depository.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Airport_to_Union_Depository.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Airport_to_Union_Depository.p_fbi1.RelationshipGroup = "POLICE";
			Airport_to_Union_Depository.p_fbi2.RelationshipGroup = "POLICE";
			Airport_to_Union_Depository.p_limo.RelationshipGroup = "POLICE";
			Airport_to_Union_Depository.vip.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Union_Depository.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Airport_to_Union_Depository.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Airport_to_Union_Depository.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Airport_to_Union_Depository.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Airport_to_Union_Depository.p_fbi1.RelationshipGroup, "COP", 1);
			Airport_to_Union_Depository.p_limo.CanAttackFriendlies = false;
			Airport_to_Union_Depository.p_fbi1.CanAttackFriendlies = false;
			Airport_to_Union_Depository.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Airport_to_Union_Depository.arBlip = new Blip(UnionDepository.sPos_prota);
			Airport_to_Union_Depository.arBlip.Color = Color.Red;
			Airport_to_Union_Depository.arBlip.EnableRoute(Color.Red);
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
				Airport_to_Union_Depository.p_limo,
				this.limo,
				Airport_to_Union_Depository.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Airport_to_Union_Depository.p_fbi1,
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
				Airport_to_Union_Depository.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", UnionDepository.sPos_limo, UnionDepository.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, UnionDepository.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, UnionDepository.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Airport_to_Union_Depository.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					UnionDepository.sPos_prota.X,
					UnionDepository.sPos_prota.Y,
					UnionDepository.sPos_prota.Z,
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
				vector..ctor(UnionDepository.sPos_prota.X, UnionDepository.sPos_prota.Y, UnionDepository.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					UnionDepository.sPos_limo.X,
					UnionDepository.sPos_limo.Y,
					UnionDepository.sPos_limo.Z,
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
				vector..ctor(UnionDepository.sPos_limo.X, UnionDepository.sPos_limo.Y, UnionDepository.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Airport_to_Union_Depository.p_limo.Tasks.ParkVehicle(UnionDepository.sPos_limo, UnionDepository.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Airport_to_Union_Depository.p_fbi1.Tasks.ParkVehicle(UnionDepository.sPos_fbi1, UnionDepository.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Airport_to_Union_Depository.p_fbi2.Tasks.ParkVehicle(UnionDepository.sPos_fbi2, UnionDepository.he_fbi2);
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
				while (Vector3.Distance(this.limo.Position, UnionDepository.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					Airport_to_Union_Depository.p_limo.Tasks.Clear();
					Airport_to_Union_Depository.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Airport_to_Union_Depository.vip.Tasks.FollowNavigationMeshToPosition(UnionDepository.wPos_vip, UnionDepository.he_vip - 180f, 0.5f).WaitForCompletion();
				Airport_to_Union_Depository.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BDB;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00011230 File Offset: 0x0000F430
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
				Airport_to_Union_Depository.arBlip.Delete();
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
				Airport_to_Union_Depository.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Union_Depository.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Union_Depository.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Airport_to_Union_Depository.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x0400019C RID: 412
		private Vector3 SpawnPoint;

		// Token: 0x0400019D RID: 413
		internal static Ped p_limo;

		// Token: 0x0400019E RID: 414
		internal static Ped vip;

		// Token: 0x0400019F RID: 415
		internal static Ped p_fbi1;

		// Token: 0x040001A0 RID: 416
		internal static Ped p_fbi2;

		// Token: 0x040001A1 RID: 417
		private Blip blBlip;

		// Token: 0x040001A2 RID: 418
		internal static Blip arBlip;

		// Token: 0x040001A3 RID: 419
		private Blip b_limo;

		// Token: 0x040001A4 RID: 420
		private Blip b_fbi1;

		// Token: 0x040001A5 RID: 421
		private Blip b_fbi2;

		// Token: 0x040001A6 RID: 422
		private float distance;

		// Token: 0x040001A7 RID: 423
		internal static Vehicle prota;

		// Token: 0x040001A8 RID: 424
		private bool isCalloutAccepted;

		// Token: 0x040001A9 RID: 425
		private Vehicle g;

		// Token: 0x040001AA RID: 426
		private Vehicle limo = EManager.limoM;

		// Token: 0x040001AB RID: 427
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x040001AC RID: 428
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
