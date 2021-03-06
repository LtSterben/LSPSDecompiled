﻿using System;
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
	// Token: 0x0200001C RID: 28
	[CalloutInfo("Observatory to Richards Majestic", 3)]
	public class Observatory_to_Richards_Majestic : Callout
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00008620 File Offset: 0x00006820
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = GalileoObservatory.sPos_prota;
			EManager.limoM = new Vehicle(Main.vPatrol1, GalileoObservatory.sPos_limo, GalileoObservatory.he_limo);
			EManager.fbi1M = new Vehicle(Main.vPatrol2, GalileoObservatory.sPos_fbi1, GalileoObservatory.he_fbi1);
			EManager.fbi2M = new Vehicle(Main.vPatrol3, GalileoObservatory.sPos_fbi2, GalileoObservatory.he_fbi2);
			Observatory_to_Richards_Majestic.p_limo = new Ped("S_M_M_FIBOffice_01", EManager.limoM.Position, 0f);
			Observatory_to_Richards_Majestic.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			Observatory_to_Richards_Majestic.p_fbi1 = new Ped("S_M_M_FIBOffice_02", EManager.fbi1M.Position, 0f);
			Observatory_to_Richards_Majestic.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			Observatory_to_Richards_Majestic.p_fbi2 = new Ped("U_M_M_JewelSec_01", EManager.fbi2M.Position, 0f);
			Observatory_to_Richards_Majestic.p_fbi2.WarpIntoVehicle(EManager.fbi2M, -1);
			if (!EntityExtensions.Exists(Observatory_to_Richards_Majestic.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Observatory_to_Richards_Majestic.p_fbi1))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Observatory_to_Richards_Majestic.p_fbi2))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Protection : Galileo Observatory to Richards Majestic";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000087A4 File Offset: 0x000069A4
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Galileo Observatory~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.GALtoRMS;
			EManager.currentCallType = EManager.CallType.Regular;
			this.distance = Vector3.Distance(RichardsMajesticStudio.sPos_limo, this.SpawnPoint);
			EManager.departure = GalileoObservatory.sPos_limo;
			EManager.arrival = RichardsMajesticStudio.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Richards Majestic Studio";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00008850 File Offset: 0x00006A50
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.fbi2M.Delete();
			EManager.limoM.Delete();
			Observatory_to_Richards_Majestic.p_fbi1.Delete();
			Observatory_to_Richards_Majestic.p_fbi2.Delete();
			Observatory_to_Richards_Majestic.p_limo.Delete();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000088A0 File Offset: 0x00006AA0
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
					Observatory_to_Richards_Majestic.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Observatory_to_Richards_Majestic.prota = new Vehicle("FBI2", GalileoObservatory.sPos_prota, GalileoObservatory.he_prota);
					goto IL_AC;
				}
			}
			Observatory_to_Richards_Majestic.prota = new Vehicle("FBI2", GalileoObservatory.sPos_prota, GalileoObservatory.he_prota);
			IL_AC:
			if (Observatory_to_Richards_Majestic.prota == null || !Observatory_to_Richards_Majestic.prota.IsValid() || !EntityExtensions.Exists(Observatory_to_Richards_Majestic.prota))
			{
				Observatory_to_Richards_Majestic.prota = new Vehicle("FBI2", GalileoObservatory.sPos_prota, GalileoObservatory.he_prota);
			}
			EManager.protaM = Observatory_to_Richards_Majestic.prota;
			this.blBlip.Delete();
			Observatory_to_Richards_Majestic.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Observatory_to_Richards_Majestic.p_fbi2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 300, true);
			Observatory_to_Richards_Majestic.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the VIP.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
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
			Observatory_to_Richards_Majestic.vip = new Ped(EManager.vipModel, GalileoObservatory.wPos_vip, GalileoObservatory.he_vip);
			Observatory_to_Richards_Majestic.vip.MakePersistent();
			Observatory_to_Richards_Majestic.vip.Tasks.FollowNavigationMeshToPosition(this.limo.Position, 0.7f, this.limo.Heading, 0f);
			Observatory_to_Richards_Majestic.vip.IsVisible = true;
			GameFiber.StartNew(new ThreadStart(new EManager().isVIPDead));
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			while (Vector3.Distance(Observatory_to_Richards_Majestic.vip.Position, this.limo.Position) > 15f)
			{
				GameFiber.Yield();
			}
			Observatory_to_Richards_Majestic.p_fbi1.RelationshipGroup = "POLICE";
			Observatory_to_Richards_Majestic.p_fbi2.RelationshipGroup = "POLICE";
			Observatory_to_Richards_Majestic.p_limo.RelationshipGroup = "POLICE";
			Observatory_to_Richards_Majestic.vip.RelationshipGroup = "POLICE";
			Observatory_to_Richards_Majestic.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Observatory_to_Richards_Majestic.vip.BlockPermanentEvents = true;
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			this.fbi2.MakePersistent();
			Observatory_to_Richards_Majestic.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_fbi2 = this.fbi2.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			this.b_fbi2.Sprite = 1;
			Game.FrameRender -= EManager.Game_FrameRenderRes;
			Game.SetRelationshipBetweenRelationshipGroups(Observatory_to_Richards_Majestic.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Observatory_to_Richards_Majestic.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Observatory_to_Richards_Majestic.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Observatory_to_Richards_Majestic.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Observatory_to_Richards_Majestic.p_fbi1.RelationshipGroup, "COP", 1);
			Observatory_to_Richards_Majestic.p_limo.CanAttackFriendlies = false;
			Observatory_to_Richards_Majestic.p_fbi1.CanAttackFriendlies = false;
			Observatory_to_Richards_Majestic.p_fbi2.CanAttackFriendlies = false;
			Game.FrameRender += EManager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Observatory_to_Richards_Majestic.arBlip = new Blip(RichardsMajesticStudio.sPos_prota);
			Observatory_to_Richards_Majestic.arBlip.Color = Color.Red;
			Observatory_to_Richards_Majestic.arBlip.EnableRoute(Color.Red);
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
				Observatory_to_Richards_Majestic.p_limo,
				this.limo,
				Observatory_to_Richards_Majestic.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Observatory_to_Richards_Majestic.p_fbi1,
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
				Observatory_to_Richards_Majestic.p_fbi2,
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
			Ped ped = new Ped("S_M_Y_Construct_01", RichardsMajesticStudio.sPos_limo, RichardsMajesticStudio.he_limo);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, RichardsMajesticStudio.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, RichardsMajesticStudio.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Observatory_to_Richards_Majestic.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2;
			Vector3 vector;
			if (!EManager.isPlayerDrivingLimo)
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					RichardsMajesticStudio.sPos_prota.X,
					RichardsMajesticStudio.sPos_prota.Y,
					RichardsMajesticStudio.sPos_prota.Z,
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
				vector..ctor(RichardsMajesticStudio.sPos_prota.X, RichardsMajesticStudio.sPos_prota.Y, RichardsMajesticStudio.sPos_prota.Z);
			}
			else
			{
				num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
				{
					0,
					RichardsMajesticStudio.sPos_limo.X,
					RichardsMajesticStudio.sPos_limo.Y,
					RichardsMajesticStudio.sPos_limo.Z,
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
				vector..ctor(RichardsMajesticStudio.sPos_limo.X, RichardsMajesticStudio.sPos_limo.Y, RichardsMajesticStudio.sPos_limo.Z);
			}
			Game.DisplaySubtitle("Park the car at the ~y~stand-by point~w~.", 5000);
			if (!EManager.isPlayerDrivingLimo)
			{
				Observatory_to_Richards_Majestic.p_limo.Tasks.ParkVehicle(RichardsMajesticStudio.sPos_limo, RichardsMajesticStudio.he_limo);
			}
			Game.FrameRender += EManager.Mod_Controls2;
			try
			{
				Observatory_to_Richards_Majestic.p_fbi1.Tasks.ParkVehicle(RichardsMajesticStudio.sPos_fbi1, RichardsMajesticStudio.he_fbi1);
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
			}
			GameFiber.Sleep(4000);
			try
			{
				Observatory_to_Richards_Majestic.p_fbi2.Tasks.ParkVehicle(RichardsMajesticStudio.sPos_fbi2, RichardsMajesticStudio.he_fbi2);
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
				while (Vector3.Distance(this.limo.Position, RichardsMajesticStudio.sPos_limo) > 5f)
				{
					GameFiber.Yield();
				}
				if (!EManager.isPlayerDrivingLimo)
				{
					Observatory_to_Richards_Majestic.p_limo.Tasks.Clear();
					Observatory_to_Richards_Majestic.p_limo.Tasks.LeaveVehicle(256);
				}
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
				Observatory_to_Richards_Majestic.vip.Tasks.FollowNavigationMeshToPosition(RichardsMajesticStudio.wPos_vip, RichardsMajesticStudio.he_vip - 180f, 0.5f).WaitForCompletion();
				Observatory_to_Richards_Majestic.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the VIP.", 5000);
				this.End();
				return;
			}
			goto IL_BCA;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000095B0 File Offset: 0x000077B0
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
				Observatory_to_Richards_Majestic.arBlip.Delete();
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
				Observatory_to_Richards_Majestic.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Observatory_to_Richards_Majestic.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Observatory_to_Richards_Majestic.p_fbi2.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Observatory_to_Richards_Majestic.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x04000128 RID: 296
		private Vector3 SpawnPoint;

		// Token: 0x04000129 RID: 297
		internal static Ped p_limo;

		// Token: 0x0400012A RID: 298
		internal static Ped vip;

		// Token: 0x0400012B RID: 299
		internal static Ped p_fbi1;

		// Token: 0x0400012C RID: 300
		internal static Ped p_fbi2;

		// Token: 0x0400012D RID: 301
		private Blip blBlip;

		// Token: 0x0400012E RID: 302
		internal static Blip arBlip;

		// Token: 0x0400012F RID: 303
		private Blip b_limo;

		// Token: 0x04000130 RID: 304
		private Blip b_fbi1;

		// Token: 0x04000131 RID: 305
		private Blip b_fbi2;

		// Token: 0x04000132 RID: 306
		private float distance;

		// Token: 0x04000133 RID: 307
		internal static Vehicle prota;

		// Token: 0x04000134 RID: 308
		private bool isCalloutAccepted;

		// Token: 0x04000135 RID: 309
		private Vehicle limo = EManager.limoM;

		// Token: 0x04000136 RID: 310
		private Vehicle fbi1 = EManager.fbi1M;

		// Token: 0x04000137 RID: 311
		private Vehicle fbi2 = EManager.fbi2M;
	}
}
