using System;
using System.Drawing;
using System.Threading;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Protection_Squad.Positions;
using Rage;
using Rage.Native;

namespace Protection_Squad.Callouts
{
	// Token: 0x02000028 RID: 40
	[CalloutInfo("Ambulance Escort - Bollingbrook to Davis", 3)]
	public class Ambulance_Escort_Bollingbrook_to_Davis : Callout
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x0001589C File Offset: 0x00013A9C
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = Bolingbroke_EMS.sPosProta;
			Ambulance_Escort_Bollingbrook_to_Davis.limo = new Vehicle("AMBULANCE", Bolingbroke_EMS.sPosAmbulance, Bolingbroke_EMS.he_ambulance);
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1 = new Ped("S_M_M_Paramedic_01", Ambulance_Escort_Bollingbrook_to_Davis.limo.Position, 0f);
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.WarpIntoVehicle(Ambulance_Escort_Bollingbrook_to_Davis.limo, -1);
			if (!EntityExtensions.Exists(Ambulance_Escort_Bollingbrook_to_Davis.limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(Ambulance_Escort_Bollingbrook_to_Davis.pems_1))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Ambulance Escort : State Penitentiary";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00015970 File Offset: 0x00013B70
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Bolingbroke State Penitentiary~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.SCALL_1;
			EManager.currentCallType = EManager.CallType.Ambulance;
			this.distance = Vector3.Distance(DavisHospital_EMS.sPosAmbulance, this.SpawnPoint);
			EManager.departure = Bolingbroke_EMS.sPosAmbulance;
			EManager.arrival = DavisHospital_EMS.sPosProta;
			EManager.riskLevel = "~y~Medium";
			EManager.GenerateVIP();
			EManager.currentDestination = "Davis Hospital";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00015A19 File Offset: 0x00013C19
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			Ambulance_Escort_Bollingbrook_to_Davis.limo.Delete();
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.Delete();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00015A38 File Offset: 0x00013C38
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
					Ambulance_Escort_Bollingbrook_to_Davis.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Ambulance_Escort_Bollingbrook_to_Davis.prota = new Vehicle("POLICE4", Bolingbroke_EMS.sPosProta, Bolingbroke_EMS.he_prota);
					goto IL_AC;
				}
			}
			Ambulance_Escort_Bollingbrook_to_Davis.prota = new Vehicle("POLICE4", Bolingbroke_EMS.sPosProta, Bolingbroke_EMS.he_prota);
			IL_AC:
			if (Ambulance_Escort_Bollingbrook_to_Davis.prota == null || !Ambulance_Escort_Bollingbrook_to_Davis.prota.IsValid() || !EntityExtensions.Exists(Ambulance_Escort_Bollingbrook_to_Davis.prota))
			{
				Ambulance_Escort_Bollingbrook_to_Davis.prota = new Vehicle("POLICE4", Bolingbroke_EMS.sPosProta, Bolingbroke_EMS.he_prota);
			}
			EManager.protaM = Ambulance_Escort_Bollingbrook_to_Davis.prota;
			this.blBlip.Delete();
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the injured inmate.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				Bolingbroke_EMS.sPosProta.X,
				Bolingbroke_EMS.sPosProta.Y,
				Bolingbroke_EMS.sPosProta.Z,
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
			SCALL_Manager.GeneratePrisoner_EMS();
			Game.FrameRender += SCALL_Manager.Game_FrameRenderRes_EMS;
			GameFiber.StartNew(new ThreadStart(new EManager().ManageRiskLevel));
			Game.DisplaySubtitle("Wait for the inmate to get in the ambulance.", 5000);
			GameFiber.Sleep(6000);
			Ambulance_Escort_Bollingbrook_to_Davis.vip = new Ped("S_M_Y_Prisoner_01", Bolingbroke_EMS.wPos_prisoner, Bolingbroke_EMS.he_prisoner);
			Ambulance_Escort_Bollingbrook_to_Davis.vip.MakePersistent();
			GameFiber.StartNew(new ThreadStart(new SCALL_Manager().isVIPDead_EMS));
			Ambulance_Escort_Bollingbrook_to_Davis.vip.Tasks.EnterVehicle(Ambulance_Escort_Bollingbrook_to_Davis.limo, 1).WaitForCompletion();
			Ambulance_Escort_Bollingbrook_to_Davis.pems_2 = new Ped("S_M_M_Paramedic_01", Bolingbroke_EMS.wPos_prisoner, Bolingbroke_EMS.he_prisoner);
			Ambulance_Escort_Bollingbrook_to_Davis.pems_2.MakePersistent();
			Ambulance_Escort_Bollingbrook_to_Davis.pems_2.Tasks.EnterVehicle(Ambulance_Escort_Bollingbrook_to_Davis.limo, 0).WaitForCompletion();
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			Ambulance_Escort_Bollingbrook_to_Davis.vip.BlockPermanentEvents = true;
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.BlockPermanentEvents = true;
			Ambulance_Escort_Bollingbrook_to_Davis.pems_2.BlockPermanentEvents = true;
			Ambulance_Escort_Bollingbrook_to_Davis.limo.MakePersistent();
			Ambulance_Escort_Bollingbrook_to_Davis.prota.MakePersistent();
			Ambulance_Escort_Bollingbrook_to_Davis.limo.IsSirenOn = true;
			Ambulance_Escort_Bollingbrook_to_Davis.limo.IsSirenSilent = true;
			this.b_limo = Ambulance_Escort_Bollingbrook_to_Davis.limo.AttachBlip();
			this.b_limo.Sprite = 58;
			Game.FrameRender -= SCALL_Manager.Game_FrameRenderRes_EMS;
			Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup = "POLICE";
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.RelationshipGroup = "POLICE";
			Ambulance_Escort_Bollingbrook_to_Davis.pems_2.RelationshipGroup = "POLICE";
			Game.SetRelationshipBetweenRelationshipGroups(Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Ambulance_Escort_Bollingbrook_to_Davis.vip.RelationshipGroup, "COP", 1);
			Game.FrameRender += EManager.Game_FrameRender_SP1;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the team to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Ambulance_Escort_Bollingbrook_to_Davis.arBlip = new Blip(DavisHospital_EMS.sPosProta);
			Ambulance_Escort_Bollingbrook_to_Davis.arBlip.Color = Color.Red;
			Ambulance_Escort_Bollingbrook_to_Davis.arBlip.EnableRoute(Color.Red);
			GameFiber.StartNew(new ThreadStart(new EManager().ExternalEvent));
			Ambulance_Escort_Bollingbrook_to_Davis.limo.IsSirenOn = true;
			if (Main.muteSirens)
			{
				Ambulance_Escort_Bollingbrook_to_Davis.limo.IsSirenSilent = true;
			}
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Ambulance_Escort_Bollingbrook_to_Davis.pems_1,
				Ambulance_Escort_Bollingbrook_to_Davis.limo,
				Ambulance_Escort_Bollingbrook_to_Davis.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			GameFiber.StartNew(new ThreadStart(new EManager().StuckVehicle));
			GameFiber.StartNew(new ThreadStart(new SCALL_Manager().DistanceWatcher_EMS));
			Ped ped = new Ped("S_M_Y_Construct_01", DavisHospital_EMS.sPosAmbulance, DavisHospital_EMS.he_ambulance);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, DavisHospital_EMS.sPosProta) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, DavisHospital_EMS.sPosProta) > 23f)
			{
				GameFiber.Yield();
			}
			Ambulance_Escort_Bollingbrook_to_Davis.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				DavisHospital_EMS.sPosProta.X,
				DavisHospital_EMS.sPosProta.Y,
				DavisHospital_EMS.sPosProta.Z,
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
			Game.DisplaySubtitle("Park your car at the ~y~stand-by point~w~.", 5000);
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.Tasks.ParkVehicle(DavisHospital_EMS.sPosAmbulance, DavisHospital_EMS.he_ambulance);
			Game.FrameRender += EManager.Mod_Controls2;
			while (Vector3.Distance(Ambulance_Escort_Bollingbrook_to_Davis.prota.Position, DavisHospital_EMS.sPosProta) > 4f)
			{
				GameFiber.Yield();
			}
			Game.FrameRender -= EManager.Mod_Controls2;
			GameFiber.Sleep(2000);
			NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
			{
				num2
			});
			while (Vector3.Distance(Ambulance_Escort_Bollingbrook_to_Davis.limo.Position, DavisHospital_EMS.sPosAmbulance) > 5f)
			{
				GameFiber.Yield();
			}
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.Tasks.Clear();
			Ambulance_Escort_Bollingbrook_to_Davis.pems_1.Tasks.LeaveVehicle(256);
			EManager.currentCallState = EManager.CallState.Arrived;
			Game.DisplaySubtitle("Leave the vehicle and stay close to the VIP.", 5000);
			Ambulance_Escort_Bollingbrook_to_Davis.vip.Tasks.FollowNavigationMeshToPosition(DavisHospital_EMS.wPos_prisoner, DavisHospital_EMS.he_prisoner - 180f, 0.5f).WaitForCompletion();
			Ambulance_Escort_Bollingbrook_to_Davis.vip.Delete();
			Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the injured inmate.", 5000);
			this.End();
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0001631C File Offset: 0x0001451C
		public override void End()
		{
			base.End();
			EManager.currentCallState = EManager.CallState.Arrived;
			Game.FrameRender -= EManager.Game_FrameRender_SP1;
			if (this.isCalloutAccepted)
			{
				Ambulance_Escort_Bollingbrook_to_Davis.limo.IsHandbrakeForced = false;
			}
			try
			{
				Ambulance_Escort_Bollingbrook_to_Davis.arBlip.Delete();
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
				Ambulance_Escort_Bollingbrook_to_Davis.limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Ambulance_Escort_Bollingbrook_to_Davis.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Ambulance_Escort_Bollingbrook_to_Davis.pems_1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Ambulance_Escort_Bollingbrook_to_Davis.pems_2.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x040001EC RID: 492
		private Vector3 SpawnPoint;

		// Token: 0x040001ED RID: 493
		internal static Ped vip;

		// Token: 0x040001EE RID: 494
		internal static Ped pems_1;

		// Token: 0x040001EF RID: 495
		internal static Ped pems_2;

		// Token: 0x040001F0 RID: 496
		private Blip blBlip;

		// Token: 0x040001F1 RID: 497
		internal static Blip arBlip;

		// Token: 0x040001F2 RID: 498
		private Blip b_limo;

		// Token: 0x040001F3 RID: 499
		private float distance;

		// Token: 0x040001F4 RID: 500
		internal static Vehicle prota;

		// Token: 0x040001F5 RID: 501
		internal static Vehicle limo;

		// Token: 0x040001F6 RID: 502
		private bool isCalloutAccepted;
	}
}
