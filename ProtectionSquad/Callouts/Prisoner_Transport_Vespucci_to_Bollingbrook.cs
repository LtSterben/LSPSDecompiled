using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Microsoft.CSharp.RuntimeBinder;
using Protection_Squad.Positions.Jails;
using Rage;
using Rage.Native;

namespace Protection_Squad.Callouts
{
	// Token: 0x02000027 RID: 39
	[CalloutInfo("Prisoner Transport - Vespucci to Bollingbrook", 3)]
	public class Prisoner_Transport_Vespucci_to_Bollingbrook : Callout
	{
		// Token: 0x060000AA RID: 170 RVA: 0x00014774 File Offset: 0x00012974
		public override bool OnBeforeCalloutDisplayed()
		{
			this.SpawnPoint = VespucciPolice.sPos_prota;
			EManager.limoM = new Vehicle("PBUS", VespucciPolice.sPos_bus, VespucciPolice.he_bus);
			EManager.fbi1M = new Vehicle("FBI2", VespucciPolice.sPos_riot, VespucciPolice.he_riot);
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo = new Ped("S_M_Y_Swat_01", EManager.limoM.Position, 0f);
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.WarpIntoVehicle(EManager.limoM, -1);
			this.p_fbi1 = new Ped("MP_M_FIBSec_01", EManager.fbi1M.Position, 0f);
			this.p_fbi1.WarpIntoVehicle(EManager.fbi1M, -1);
			if (!EntityExtensions.Exists(Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo))
			{
				return false;
			}
			if (!EntityExtensions.Exists(this.p_fbi1))
			{
				return false;
			}
			base.ShowCalloutAreaBlipBeforeAccepting(this.SpawnPoint, 25f);
			base.AddMinimumDistanceCheck(15f, this.SpawnPoint);
			base.CalloutMessage = "Prisoner Transfer : Vespucci Police Station";
			base.CalloutPosition = this.SpawnPoint;
			Functions.PlayScannerAudioUsingPosition("WE_HAVE CONVOY_ESCORT IN_OR_ON_POSITION", this.SpawnPoint);
			return base.OnBeforeCalloutDisplayed();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0001489C File Offset: 0x00012A9C
		public override bool OnCalloutAccepted()
		{
			Game.DisplaySubtitle("Go to the ~g~Vespucci Police Station~w~.", 5000);
			this.blBlip = new Blip(this.SpawnPoint);
			this.blBlip.Color = Color.LimeGreen;
			this.blBlip.EnableRoute(Color.LimeGreen);
			EManager.currentCall = EManager.Calls.PCALL_DWT_BBP;
			EManager.currentCallType = EManager.CallType.Prison;
			this.distance = Vector3.Distance(Bolingbroke.sPos_bus, this.SpawnPoint);
			EManager.arrival = Bolingbroke.sPos_prota;
			EManager.riskLevel = "~y~Medium";
			PCALL_Manager.GeneratePrisoner();
			EManager.currentDestination = "Bolingbroke Penitentiary";
			EManager.currentCallState = EManager.CallState.Initiated;
			return base.OnCalloutAccepted();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0001493A File Offset: 0x00012B3A
		public override void OnCalloutNotAccepted()
		{
			base.OnCalloutNotAccepted();
			EManager.fbi1M.Delete();
			EManager.limoM.Delete();
			this.p_fbi1.Delete();
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Delete();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0001496C File Offset: 0x00012B6C
		public override void Process()
		{
			base.Process();
			this.isCalloutAccepted = true;
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, this.SpawnPoint) > 40f)
			{
				GameFiber.Yield();
			}
			NativeFunction.CallByName<uint>("CLEAR_ALL_HELP_MESSAGES", Array.Empty<NativeArgument>());
			if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
			{
				try
				{
					Prisoner_Transport_Vespucci_to_Bollingbrook.prota = Game.LocalPlayer.Character.CurrentVehicle;
					goto IL_AC;
				}
				catch (Exception)
				{
					Prisoner_Transport_Vespucci_to_Bollingbrook.prota = new Vehicle("FBI2", VespucciPolice.sPos_prota, VespucciPolice.he_prota);
					goto IL_AC;
				}
			}
			Prisoner_Transport_Vespucci_to_Bollingbrook.prota = new Vehicle("FBI2", VespucciPolice.sPos_prota, VespucciPolice.he_prota);
			IL_AC:
			if (Prisoner_Transport_Vespucci_to_Bollingbrook.prota == null || !Prisoner_Transport_Vespucci_to_Bollingbrook.prota.IsValid() || !EntityExtensions.Exists(Prisoner_Transport_Vespucci_to_Bollingbrook.prota))
			{
				Prisoner_Transport_Vespucci_to_Bollingbrook.prota = new Vehicle("FBI2", VespucciPolice.sPos_prota, VespucciPolice.he_prota);
			}
			EManager.protaM = Prisoner_Transport_Vespucci_to_Bollingbrook.prota;
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip = new Ped(PCALL_Manager.pModel, VespucciPolice.wPos_prisoner, VespucciPolice.he_prisoner);
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip.MakePersistent();
			this.guard1 = new Ped("S_M_M_PrisGuard_01", VespucciPolice.wPos_guard1, VespucciPolice.he_guard1);
			this.guard1.MakePersistent();
			this.guard2 = new Ped("S_M_Y_Swat_01", VespucciPolice.wPos_guard2, VespucciPolice.he_guard2);
			this.guard2.MakePersistent();
			GameFiber.StartNew(new ThreadStart(new PCALL_Manager().isVIPDead));
			this.blBlip.Delete();
			this.p_fbi1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 500, true);
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			this.guard1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			this.guard2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 200, true);
			if (Prisoner_Transport_Vespucci_to_Bollingbrook.<>o__18.<>p__0 == null)
			{
				Prisoner_Transport_Vespucci_to_Bollingbrook.<>o__18.<>p__0 = CallSite<Action<CallSite, object, Ped, bool>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetEnableHandcuffs", null, typeof(Prisoner_Transport_Vespucci_to_Bollingbrook), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			Prisoner_Transport_Vespucci_to_Bollingbrook.<>o__18.<>p__0.Target(Prisoner_Transport_Vespucci_to_Bollingbrook.<>o__18.<>p__0, NativeFunction.Natives, Prisoner_Transport_Vespucci_to_Bollingbrook.vip, true);
			Game.DisplaySubtitle("Park your car at the ~y~stand-by checkpoint~w~ and wait for the prisoner.", 5000);
			int num = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				VespucciPolice.sPos_prota.X,
				VespucciPolice.sPos_prota.Y,
				VespucciPolice.sPos_prota.Z,
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
			this.limo = EManager.limoM;
			Game.FrameRender += PCALL_Manager.Game_FrameRenderRes;
			GameFiber.StartNew(new ThreadStart(new EManager().ManageRiskLevel));
			Game.DisplaySubtitle("Wait for the prisoner to enter the bus.", 5000);
			this.guard1.Tasks.FollowNavigationMeshToPosition(this.limo.GetOffsetPosition(new Vector3(4f, 0f, 0f)), 0.7f, this.limo.Heading, 0f);
			GameFiber.Sleep(2000);
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Tasks.FollowNavigationMeshToPosition(this.limo.GetOffsetPosition(new Vector3(3f, 0f, 0f)), 0.7f, this.limo.Heading, 0f);
			GameFiber.Sleep(3000);
			this.guard2.Tasks.FollowNavigationMeshToPosition(this.limo.GetOffsetPosition(new Vector3(4f, 0f, 0f)), 0.7f, this.limo.Heading, 0f);
			EManager.currentCallState = EManager.CallState.EscortInProgress;
			this.p_fbi1.RelationshipGroup = "POLICE";
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.RelationshipGroup = "POLICE";
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip.RelationshipGroup = "POLICE";
			this.guard1.RelationshipGroup = "POLICE";
			this.guard2.RelationshipGroup = "POLICE";
			while (Vector3.Distance(Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Position, this.limo.GetOffsetPosition(new Vector3(5f, 0f, 0f))) > 15f || Vector3.Distance(this.guard1.Position, this.limo.GetOffsetPosition(new Vector3(5f, 0f, 0f))) > 15f || Vector3.Distance(this.guard2.Position, this.limo.GetOffsetPosition(new Vector3(5f, 0f, 0f))) > 15f)
			{
				GameFiber.Yield();
			}
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Tasks.EnterVehicle(this.limo, 2).WaitForCompletion();
			Prisoner_Transport_Vespucci_to_Bollingbrook.vip.BlockPermanentEvents = true;
			this.guard1.Tasks.EnterVehicle(this.limo, 0).WaitForCompletion();
			this.guard2.Tasks.EnterVehicle(this.limo, 1).WaitForCompletion();
			GameFiber.StartNew(new ThreadStart(this.MakeGuardsFollowConvoy));
			this.limo.MakePersistent();
			this.fbi1.MakePersistent();
			Prisoner_Transport_Vespucci_to_Bollingbrook.prota.MakePersistent();
			this.b_limo = this.limo.AttachBlip();
			this.b_fbi1 = this.fbi1.AttachBlip();
			this.b_limo.Sprite = 58;
			this.b_fbi1.Sprite = 1;
			Game.FrameRender -= PCALL_Manager.Game_FrameRenderRes;
			Game.SetRelationshipBetweenRelationshipGroups(Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.RelationshipGroup, Game.LocalPlayer.Character.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(Game.LocalPlayer.Character.RelationshipGroup, Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups("ATTACKERS", Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.RelationshipGroup, 5);
			Game.SetRelationshipBetweenRelationshipGroups("COP", this.p_fbi1.RelationshipGroup, 1);
			Game.SetRelationshipBetweenRelationshipGroups(this.p_fbi1.RelationshipGroup, "COP", 1);
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.CanAttackFriendlies = false;
			this.p_fbi1.CanAttackFriendlies = false;
			Game.FrameRender += PCALL_Manager.Game_FrameRender;
			Game.LogTrivial("<<<< Los Santos Protection Unit >>>> Please stay close to your team members to ensure their security.");
			Game.DisplaySubtitle("Lead the convoy to the ~r~" + EManager.currentDestination + "~w~.", 6000);
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateRandomEnemies));
			GameFiber.StartNew(new ThreadStart(new EManager().GenerateEnemiesWithZone));
			Prisoner_Transport_Vespucci_to_Bollingbrook.arBlip = new Blip(Bolingbroke.sPos_prota);
			Prisoner_Transport_Vespucci_to_Bollingbrook.arBlip.Color = Color.Red;
			Prisoner_Transport_Vespucci_to_Bollingbrook.arBlip.EnableRoute(Color.Red);
			GameFiber.StartNew(new ThreadStart(new EManager().ExternalEvent));
			this.limo.IsSirenOn = true;
			this.fbi1.IsSirenOn = true;
			if (Main.muteSirens)
			{
				this.limo.IsSirenSilent = true;
				this.fbi1.IsSirenSilent = true;
			}
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo,
				this.limo,
				Prisoner_Transport_Vespucci_to_Bollingbrook.prota,
				-1,
				21f,
				1074528293,
				6f,
				-1,
				15f
			});
			NativeFunction.CallByName<uint>("TASK_VEHICLE_ESCORT", new NativeArgument[]
			{
				this.p_fbi1,
				this.fbi1,
				this.limo,
				-1,
				22f,
				1074528293,
				7f,
				-1,
				15f
			});
			GameFiber.StartNew(new ThreadStart(new EManager().StuckVehicle));
			GameFiber.StartNew(new ThreadStart(new EManager().DistanceWatcher));
			Ped ped = new Ped("S_M_Y_Construct_01", Bolingbroke.sPos_bus, Bolingbroke.he_bus);
			Game.FrameRender += EManager.Mod_Controls1;
			GameFiber.Sleep(12000);
			Game.FrameRender -= EManager.Mod_Controls1;
			GameFiber.Sleep(20000);
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, Bolingbroke.sPos_prota) > 75f)
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
			while (Vector3.Distance(Game.LocalPlayer.Character.Position, Bolingbroke.sPos_prota) > 60f)
			{
				GameFiber.Yield();
			}
			Prisoner_Transport_Vespucci_to_Bollingbrook.arBlip.Delete();
			EManager.currentCallState = EManager.CallState.Parking;
			int num2 = NativeFunction.CallByName<int>("CREATE_CHECKPOINT", new NativeArgument[]
			{
				0,
				Bolingbroke.sPos_prota.X,
				Bolingbroke.sPos_prota.Y,
				Bolingbroke.sPos_prota.Z,
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
			Game.DisplaySubtitle("Park your car at the ~y~stand-by point~w~.", 5000);
			Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Tasks.ParkVehicle(Bolingbroke.sPos_bus, Bolingbroke.he_bus);
			try
			{
				this.p_fbi1.Tasks.ParkVehicle(Bolingbroke.sPos_riot, Bolingbroke.he_riot);
				goto IL_C0F;
			}
			catch (Exception)
			{
				Game.LogTrivial("Fbi1 couldn't park.");
				goto IL_C0F;
			}
			IL_C0A:
			GameFiber.Yield();
			IL_C0F:
			if (Vector3.Distance(Prisoner_Transport_Vespucci_to_Bollingbrook.prota.Position, Bolingbroke.sPos_prota) <= 4f)
			{
				GameFiber.Sleep(2000);
				NativeFunction.CallByName<uint>("DELETE_CHECKPOINT", new NativeArgument[]
				{
					num2
				});
				while (Vector3.Distance(this.limo.Position, Bolingbroke.sPos_bus) > 5f)
				{
					GameFiber.Yield();
				}
				Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Tasks.Clear();
				Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Tasks.LeaveVehicle(256);
				EManager.currentCallState = EManager.CallState.Arrived;
				Game.DisplaySubtitle("Leave the vehicle and stay close to the prisoner.", 5000);
				Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Tasks.FollowNavigationMeshToPosition(Bolingbroke.wPos_prisoner, Bolingbroke.he_prisoner - 180f, 0.5f).WaitForCompletion();
				Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Delete();
				Game.DisplaySubtitle("~g~MISSION COMPLETE ~w~: You have successfully transported the inmate.", 5000);
				this.End();
				return;
			}
			goto IL_C0A;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00015694 File Offset: 0x00013894
		private void MakeGuardsFollowConvoy()
		{
			while (EManager.currentCallState == EManager.CallState.EscortInProgress)
			{
				if (this.guard1.IsAlive && !this.guard1.IsInVehicle(this.limo, false) && !this.guard1.IsInCombat)
				{
					this.guard1.Tasks.EnterVehicle(this.limo, 0).WaitForCompletion();
				}
				if (this.guard2.IsAlive && !this.guard2.IsInVehicle(this.limo, false) && !this.guard2.IsInCombat)
				{
					this.guard2.Tasks.EnterVehicle(this.limo, 1).WaitForCompletion();
				}
				GameFiber.Yield();
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0001574C File Offset: 0x0001394C
		public override void End()
		{
			base.End();
			EManager.currentCallState = EManager.CallState.Arrived;
			Game.FrameRender -= PCALL_Manager.Game_FrameRender;
			if (this.isCalloutAccepted)
			{
				this.limo.IsHandbrakeForced = false;
			}
			try
			{
				Prisoner_Transport_Vespucci_to_Bollingbrook.arBlip.Delete();
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
				Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				this.p_fbi1.Dismiss();
			}
			catch (Exception)
			{
			}
			try
			{
				Prisoner_Transport_Vespucci_to_Bollingbrook.vip.Dismiss();
			}
			catch (Exception)
			{
			}
			Game.LogTrivial("> Callout has ended.");
		}

		// Token: 0x040001DD RID: 477
		private Vector3 SpawnPoint;

		// Token: 0x040001DE RID: 478
		internal static Ped p_limo;

		// Token: 0x040001DF RID: 479
		internal static Ped vip;

		// Token: 0x040001E0 RID: 480
		internal Ped p_fbi1;

		// Token: 0x040001E1 RID: 481
		internal static Vehicle prota;

		// Token: 0x040001E2 RID: 482
		private Blip blBlip;

		// Token: 0x040001E3 RID: 483
		internal static Blip arBlip;

		// Token: 0x040001E4 RID: 484
		private Blip b_limo;

		// Token: 0x040001E5 RID: 485
		private Blip b_fbi1;

		// Token: 0x040001E6 RID: 486
		private float distance;

		// Token: 0x040001E7 RID: 487
		private Ped guard1;

		// Token: 0x040001E8 RID: 488
		private Ped guard2;

		// Token: 0x040001E9 RID: 489
		private bool isCalloutAccepted;

		// Token: 0x040001EA RID: 490
		private Vehicle limo = EManager.limoM;

		// Token: 0x040001EB RID: 491
		private Vehicle fbi1 = EManager.fbi1M;
	}
}
