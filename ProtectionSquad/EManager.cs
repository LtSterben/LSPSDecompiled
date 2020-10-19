using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using Microsoft.CSharp.RuntimeBinder;
using Protection_Squad.Callouts;
using Protection_Squad.Positions;
using Protection_Squad.Positions.Jails;
using Rage;
using Rage.Native;

namespace Protection_Squad
{
	// Token: 0x02000003 RID: 3
	internal class EManager
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002428 File Offset: 0x00000628
		internal void SyncCall()
		{
			if (EManager.currentCall == EManager.Calls.LSAtoARC)
			{
				EManager.vipM = Airport_to_Arcadius_Center.vip;
				EManager.p_limoM = Airport_to_Arcadius_Center.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.ARCtoGOC)
			{
				EManager.vipM = Arcadius_Center_to_Golf_Club.vip;
				EManager.p_limoM = Arcadius_Center_to_Golf_Club.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoNOO)
			{
				EManager.vipM = Airport_to_Noose_HQ.vip;
				EManager.p_limoM = Airport_to_Noose_HQ.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoGAL)
			{
				EManager.vipM = Kortz_Center_to_Observatory.vip;
				EManager.p_limoM = Kortz_Center_to_Observatory.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoGAL)
			{
				EManager.vipM = Airport_to_Observatory.vip;
				EManager.p_limoM = Airport_to_Observatory.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoUD)
			{
				EManager.vipM = Airport_to_Union_Depository.vip;
				EManager.p_limoM = Airport_to_Union_Depository.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.MZBtoNOO)
			{
				EManager.vipM = EMaze_Bank_to_Noose_HQ.vip;
				EManager.p_limoM = EMaze_Bank_to_Noose_HQ.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoMZB)
			{
				EManager.vipM = Observatory_to_Richards_Majestic.vip;
				EManager.p_limoM = Observatory_to_Richards_Majestic.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.UDPtoGOC)
			{
				EManager.vipM = Union_Depository_to_Golf_Club.vip;
				EManager.p_limoM = Union_Depository_to_Golf_Club.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.NOOtoMZB)
			{
				EManager.vipM = Noose_HQ_To_Maze_Bank.vip;
				EManager.p_limoM = Noose_HQ_To_Maze_Bank.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoMZB)
			{
				EManager.vipM = Kortz_Center_to_Maze_Bank.vip;
				EManager.p_limoM = Kortz_Center_to_Maze_Bank.p_limo;
			}
			if (EManager.currentCall == EManager.Calls.GALtoRMS)
			{
				EManager.vipM = Ambulance_Escort_Bollingbrook_to_Davis.vip;
				EManager.p_limoM = Ambulance_Escort_Bollingbrook_to_Davis.pems_2;
			}
			if (EManager.currentCall == EManager.Calls.PCALL_VSP_BBP)
			{
				EManager.vipM = Prisoner_Transport_Vespucci_to_Bollingbrook.vip;
				EManager.p_limoM = Prisoner_Transport_Vespucci_to_Bollingbrook.p_limo;
			}
			EManager.Calls calls = EManager.currentCall;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000025B0 File Offset: 0x000007B0
		internal void isVIPDead()
		{
			this.SyncCall();
			this.p_fbi1M = EManager.fbi1M.Driver;
			this.p_fbi2M = EManager.fbi2M.Driver;
			this.isMissionFailed = false;
			EManager.isPlayerDrivingLimo = false;
			this.thirdDriverDead = false;
			this.fourthDriverDead = false;
			try
			{
				while (EManager.currentCallState != EManager.CallState.Arrived)
				{
					if (EManager.vipM.IsInjured)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: The VIP got injured.", 5000);
						this.isMissionFailed = true;
					}
					if (EManager.p_limoM.IsDead && !EManager.isPlayerDrivingLimo)
					{
						Game.DisplaySubtitle("The ~y~limo driver~w~ is dead. Abandon your current vehicle and drive the limo by yourself.", 7000);
						EManager.isPlayerDrivingLimo = true;
					}
					if (Game.LocalPlayer.Character.IsDead)
					{
						Game.DisplaySubtitle("~r~MISSION FAILED ~w~: You died.", 8000);
						this.isMissionFailed = true;
					}
					if (this.p_fbi1M.IsDead && !this.thirdDriverDead)
					{
						Game.DisplayNotification("heisthud", "kiaoverlay", "Dispatch", "A guard is KIA", "The driver of the 3rd vehicle is reported KIA. Please continue to drive.");
						this.thirdDriverDead = true;
					}
					if (this.p_fbi2M.IsDead && !this.fourthDriverDead)
					{
						Game.DisplayNotification("heisthud", "kiaoverlay", "Dispatch", "A guard is KIA", "The driver of the 4th vehicle is reported KIA. Please continue to drive.");
						this.fourthDriverDead = true;
					}
					if (this.isMissionFailed)
					{
						Functions.StopCurrentCallout();
						Game.FrameRender -= EManager.Game_FrameRender;
						EManager.currentCallState = EManager.CallState.Arrived;
					}
					GameFiber.Yield();
				}
			}
			catch (Exception)
			{
				for (;;)
				{
					GameFiber.Yield();
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002744 File Offset: 0x00000944
		internal Vector3 CalculateVIPDestination()
		{
			Vector3 result = Vector3.Zero;
			if (EManager.currentCall == EManager.Calls.LSAtoARC)
			{
				result = ArcadiusCenter.sPos_limo;
				float he_limo = ArcadiusCenter.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.ARCtoGOC)
			{
				result = GolfClub.sPos_limo;
				float he_limo2 = GolfClub.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoNOO)
			{
				result = NOOSEHeadquarters.sPos_limo;
				float he_limo3 = NOOSEHeadquarters.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoGAL)
			{
				result = GalileoObservatory.sPos_limo;
				float he_limo4 = GalileoObservatory.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoGAL)
			{
				result = GalileoObservatory.sPos_limo;
				float he_limo5 = GalileoObservatory.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoUD)
			{
				result = UnionDepository.sPos_limo;
				float he_limo6 = UnionDepository.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.PCALL_DWT_BBP)
			{
				result = Bolingbroke.sPos_bus;
				float he_bus = Bolingbroke.he_bus;
			}
			if (EManager.currentCall == EManager.Calls.MZBtoNOO)
			{
				result = NOOSEHeadquarters.sPos_limo;
				float he_limo7 = NOOSEHeadquarters.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoMZB)
			{
				result = MazeBank.sPos_limo;
				float he_limo8 = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.UDPtoGOC)
			{
				result = GolfClub.sPos_limo;
				float he_limo9 = GolfClub.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.UDPtoGOC)
			{
				result = MazeBank.sPos_limo;
				float he_limo10 = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoMZB)
			{
				result = MazeBank.sPos_limo;
				float he_limo11 = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.GALtoRMS)
			{
				result = RichardsMajesticStudio.sPos_limo;
				float he_limo12 = RichardsMajesticStudio.he_limo;
			}
			EManager.Calls calls = EManager.currentCall;
			if (EManager.currentCall == EManager.Calls.PCALL_DWT_BBP)
			{
				result = Bolingbroke.sPos_bus;
				float he_bus2 = Bolingbroke.he_bus;
			}
			if (EManager.currentCall == EManager.Calls.NOOtoMZB)
			{
				result = MazeBank.sPos_limo;
				EManager.limoM.Heading = MazeBank.he_limo;
			}
			EManager.Calls calls2 = EManager.currentCall;
			return result;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000028A4 File Offset: 0x00000AA4
		internal float CalculateVIPDestinationHeading()
		{
			float result = 0f;
			if (EManager.currentCall == EManager.Calls.LSAtoARC)
			{
				Vector3 sPos_limo = ArcadiusCenter.sPos_limo;
				result = ArcadiusCenter.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.ARCtoGOC)
			{
				Vector3 sPos_limo2 = GolfClub.sPos_limo;
				result = GolfClub.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoNOO)
			{
				Vector3 sPos_limo3 = NOOSEHeadquarters.sPos_limo;
				result = NOOSEHeadquarters.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoGAL)
			{
				Vector3 sPos_limo4 = GalileoObservatory.sPos_limo;
				result = GalileoObservatory.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoGAL)
			{
				Vector3 sPos_limo5 = GalileoObservatory.sPos_limo;
				result = GalileoObservatory.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoUD)
			{
				Vector3 sPos_limo6 = UnionDepository.sPos_limo;
				result = UnionDepository.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.PCALL_DWT_BBP)
			{
				Vector3 sPos_bus = Bolingbroke.sPos_bus;
				result = Bolingbroke.he_bus;
			}
			if (EManager.currentCall == EManager.Calls.MZBtoNOO)
			{
				Vector3 sPos_limo7 = NOOSEHeadquarters.sPos_limo;
				result = NOOSEHeadquarters.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.LSAtoMZB)
			{
				Vector3 sPos_limo8 = MazeBank.sPos_limo;
				result = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.UDPtoGOC)
			{
				Vector3 sPos_limo9 = GolfClub.sPos_limo;
				result = GolfClub.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.UDPtoGOC)
			{
				Vector3 sPos_limo10 = MazeBank.sPos_limo;
				result = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.KORtoMZB)
			{
				Vector3 sPos_limo11 = MazeBank.sPos_limo;
				result = MazeBank.he_limo;
			}
			if (EManager.currentCall == EManager.Calls.GALtoRMS)
			{
				Vector3 sPos_limo12 = RichardsMajesticStudio.sPos_limo;
				result = RichardsMajesticStudio.he_limo;
			}
			EManager.Calls calls = EManager.currentCall;
			if (EManager.currentCall == EManager.Calls.PCALL_DWT_BBP)
			{
				Vector3 sPos_bus2 = Bolingbroke.sPos_bus;
				result = Bolingbroke.he_bus;
			}
			if (EManager.currentCall == EManager.Calls.NOOtoMZB)
			{
				Vector3 sPos_limo13 = MazeBank.sPos_limo;
				result = MazeBank.he_limo;
			}
			EManager.Calls calls2 = EManager.currentCall;
			return result;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000029FC File Offset: 0x00000BFC
		internal void StuckVehicle()
		{
			this.SyncCall();
			try
			{
				this.SyncCall();
				while (EManager.currentCallState != EManager.CallState.Arrived)
				{
					while (!Game.IsKeyDown(Keys.Y) && !Game.IsKeyDownRightNow(Keys.Shift))
					{
						GameFiber.Yield();
					}
					if (EManager.currentCallState == EManager.CallState.Arrived)
					{
						break;
					}
					if (EManager.currentCallState == EManager.CallState.Parking)
					{
						EManager.limoM.Driver.Tasks.Clear();
						EManager.limoM.Position = this.CalculateVIPDestination();
						EManager.limoM.Heading = this.CalculateVIPDestinationHeading();
						if (EManager.currentCall == EManager.Calls.SCALL_1)
						{
							Ambulance_Escort_Bollingbrook_to_Davis.limo.Position = DavisHospital_EMS.sPosAmbulance;
							Ambulance_Escort_Bollingbrook_to_Davis.limo.Heading = DavisHospital_EMS.he_ambulance;
						}
						EManager.limoM.IsHandbrakeForced = true;
					}
					if (EManager.currentCallState == EManager.CallState.EscortInProgress)
					{
						this.SyncCall();
						Game.DisplaySubtitle("Convoy reset.", 4000);
						if (EManager.limoM.IsAlive)
						{
							EManager.limoM.Repair();
							EManager.limoM.Position = World.GetNextPositionOnStreet(EManager.protaM.GetOffsetPosition(new Vector3(0f, -15f, 0f)));
							EManager.limoM.Heading = EManager.protaM.Heading;
						}
						if (EManager.currentCallType != EManager.CallType.Ambulance)
						{
							EManager.fbi1M.Repair();
							EManager.fbi1M.Position = World.GetNextPositionOnStreet(EManager.limoM.GetOffsetPosition(new Vector3(0f, -15f, 0f)));
							EManager.fbi1M.Heading = EManager.limoM.Heading;
						}
						if (EManager.currentCallType == EManager.CallType.Regular)
						{
							EManager.fbi2M.Repair();
							EManager.fbi2M.Position = World.GetNextPositionOnStreet(EManager.fbi1M.GetOffsetPosition(new Vector3(0f, -15f, 0f)));
							EManager.fbi2M.Heading = EManager.fbi1M.Heading;
						}
						GameFiber.Yield();
					}
					GameFiber.Yield();
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002C00 File Offset: 0x00000E00
		internal static void GenerateVIP()
		{
			int index = new Random().Next(0, EManager.vipNames.Count);
			EManager.currentVipName = EManager.vipNames.ElementAt(index);
			EManager.currentVipJobs = EManager.vipJobs.ElementAt(index);
			EManager.reason = EManager.reasonList.ElementAt(new Random().Next(0, EManager.reasonList.Count));
			if (EManager.currentVipName == "Ryan Spencer")
			{
				EManager.vipModel = "A_M_M_Business_01";
				return;
			}
			if (EManager.currentVipName == "Julia Miller")
			{
				EManager.vipModel = "A_F_Y_Business_01";
				return;
			}
			if (EManager.currentVipName == "Lauren Baxter")
			{
				EManager.vipModel = "A_F_Y_Business_01";
				return;
			}
			if (EManager.currentVipName == "Romeo Blackwell")
			{
				EManager.vipModel = "A_M_Y_Business_03";
				return;
			}
			if (EManager.currentVipName == "Michael de Santa")
			{
				EManager.vipModel = "Player_Zero";
				return;
			}
			if (EManager.currentVipName == "Sir Isaac Newton")
			{
				EManager.vipModel = "A_M_M_ProlHost_01";
				return;
			}
			if (EManager.currentVipName == "Cris Formage")
			{
				EManager.vipModel = "IG_ChrisFormage";
				return;
			}
			if (EManager.currentVipName == "Warren Rhodes")
			{
				EManager.vipModel = "A_M_Y_Business_02";
				return;
			}
			if (EManager.currentVipName == "Dave Thomas")
			{
				EManager.vipModel = "A_M_Y_Vinewood_01";
				return;
			}
			if (EManager.currentVipName == "Steven Malone")
			{
				EManager.vipModel = "A_M_Y_VinDouche_01";
				return;
			}
			if (EManager.currentVipName == "Fei Zongmeng")
			{
				EManager.vipModel = "G_M_Y_KorLieut_01";
				return;
			}
			if (EManager.currentVipName == "Lazlow")
			{
				EManager.vipModel = "ig_lazlow";
				return;
			}
			if (EManager.currentVipName == "Molly Schultz")
			{
				EManager.vipModel = "ig_molly";
				return;
			}
			if (EManager.currentVipName == "Karen Daniels")
			{
				EManager.vipModel = "ig_michelle";
				return;
			}
			EManager.vipModel = "S_M_M_FIBOffice_02";
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002DFC File Offset: 0x00000FFC
		private static string ZoneName(Vector3 p)
		{
			if (EManager.<>o__39.<>p__1 == null)
			{
				EManager.<>o__39.<>p__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(EManager)));
			}
			Func<CallSite, object, string> target = EManager.<>o__39.<>p__1.Target;
			CallSite <>p__ = EManager.<>o__39.<>p__1;
			if (EManager.<>o__39.<>p__0 == null)
			{
				EManager.<>o__39.<>p__0 = CallSite<Func<CallSite, object, float, float, float, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetNameOfZone", new Type[]
				{
					typeof(string)
				}, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			string text = target(<>p__, EManager.<>o__39.<>p__0.Target(EManager.<>o__39.<>p__0, NativeFunction.Natives, p.X, p.Y, p.Z));
			if (text != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2058742407U)
				{
					if (num <= 1111785827U)
					{
						if (num <= 449355263U)
						{
							if (num <= 103699681U)
							{
								if (num <= 26379365U)
								{
									if (num != 24879385U)
									{
										if (num == 26379365U)
										{
											if (text == "AIRP")
											{
												return "Los Santos International Airport";
											}
										}
									}
									else if (text == "GRAPES")
									{
										return "Grapeseed";
									}
								}
								else if (num != 82855592U)
								{
									if (num != 85850078U)
									{
										if (num == 103699681U)
										{
											if (text == "PALFOR")
											{
												return "Paleto Forest";
											}
										}
									}
									else if (text == "RICHM")
									{
										return "Richman";
									}
								}
								else if (text == "CHAMH")
								{
									return "Chamberlain Hills";
								}
							}
							else if (num <= 355115139U)
							{
								if (num != 153008824U)
								{
									if (num != 195837437U)
									{
										if (num == 355115139U)
										{
											if (text == "MIRR")
											{
												return "Mirror Park";
											}
										}
									}
									else if (text == "VINE")
									{
										return "Vinewood";
									}
								}
								else if (text == "HAWICK")
								{
									return "Hawick";
								}
							}
							else if (num != 419258295U)
							{
								if (num != 419713684U)
								{
									if (num == 449355263U)
									{
										if (text == "ZQ_UAR")
										{
											return "Davis Quartz";
										}
									}
								}
								else if (text == "PALETO")
								{
									return "Paleto Bay";
								}
							}
							else if (text == "GREATC")
							{
								return "Great Chaparral";
							}
						}
						else if (num <= 601498259U)
						{
							if (num <= 456692244U)
							{
								if (num != 453623384U)
								{
									if (num == 456692244U)
									{
										if (text == "ELGORL")
										{
											return "El Gordo Lighthouse";
										}
									}
								}
								else if (text == "HARMO")
								{
									return "Harmony";
								}
							}
							else if (num != 518158673U)
							{
								if (num != 546144455U)
								{
									if (num == 601498259U)
									{
										if (text == "DOWNT")
										{
											return "Downtown";
										}
									}
								}
								else if (text == "STAD")
								{
									return "Maze Bank Arena";
								}
							}
							else if (text == "DELPE")
							{
								return "Del Perro";
							}
						}
						else if (num <= 773153429U)
						{
							if (num != 663335423U)
							{
								if (num != 703978872U)
								{
									if (num == 773153429U)
									{
										if (text == "TEXTI")
										{
											return "Textile City";
										}
									}
								}
								else if (text == "PROCOB")
								{
									return "Procopio Beach";
								}
							}
							else if (text == "VESP")
							{
								return "Vespucci";
							}
						}
						else if (num != 921121456U)
						{
							if (num != 992322320U)
							{
								if (num == 1111785827U)
								{
									if (text == "NOOSE")
									{
										return "N.O.O.S.E.";
									}
								}
							}
							else if (text == "OCEANA")
							{
								return "Pacific Ocean";
							}
						}
						else if (text == "HUMLAB")
						{
							return "Humane Labs and Research";
						}
					}
					else if (num <= 1479948248U)
					{
						if (num <= 1298673708U)
						{
							if (num <= 1184512168U)
							{
								if (num != 1153185670U)
								{
									if (num == 1184512168U)
									{
										if (text == "VCANA")
										{
											return "Vespucci Canals";
										}
									}
								}
								else if (text == "CCREAK")
								{
									return "Cassidy Creek";
								}
							}
							else if (num != 1242535891U)
							{
								if (num != 1281343745U)
								{
									if (num == 1298673708U)
									{
										if (text == "ELYSIAN")
										{
											return "Elysian Island";
										}
									}
								}
								else if (text == "CHU")
								{
									return "Chumash";
								}
							}
							else if (text == "KOREAT")
							{
								return "Little Seoul";
							}
						}
						else if (num <= 1401128444U)
						{
							if (num != 1305601008U)
							{
								if (num != 1381131710U)
								{
									if (num == 1401128444U)
									{
										if (text == "SANCHIA")
										{
											return "San Chianski Mountain Range";
										}
									}
								}
								else if (text == "TONGVAH")
								{
									return "Tongva Hills";
								}
							}
							else if (text == "DELSOL")
							{
								return "Puerto Del Sol";
							}
						}
						else if (num != 1431613378U)
						{
							if (num != 1452126557U)
							{
								if (num == 1479948248U)
								{
									if (text == "CANNY")
									{
										return "Raton Canyon";
									}
								}
							}
							else if (text == "PALMPOW")
							{
								return "Palmer-Taylor Power Station";
							}
						}
						else if (text == "CYPRE")
						{
							return "Cypress Flats";
						}
					}
					else if (num <= 1790808390U)
					{
						if (num <= 1652378824U)
						{
							if (num != 1548907900U)
							{
								if (num == 1652378824U)
								{
									if (text == "BRADP")
									{
										return "Braddock Pass";
									}
								}
							}
							else if (text == "TONGVAV")
							{
								return "Tongva Valley";
							}
						}
						else if (num != 1719489300U)
						{
							if (num != 1765030291U)
							{
								if (num == 1790808390U)
								{
									if (text == "SKID")
									{
										return "Mission Row";
									}
								}
							}
							else if (text == "WINDF")
							{
								return "RON Alternates Wind Farm";
							}
						}
						else if (text == "BRADT")
						{
							return "Braddock Tunnel";
						}
					}
					else if (num <= 1911143894U)
					{
						if (num != 1795378360U)
						{
							if (num != 1872058313U)
							{
								if (num == 1911143894U)
								{
									if (text == "SanAnd")
									{
										return "San Andreas";
									}
								}
							}
							else if (text == "MTGORDO")
							{
								return "Mount Gordo";
							}
						}
						else if (text == "PBLUFF")
						{
							return "Pacific Bluffs";
						}
					}
					else if (num != 1927597590U)
					{
						if (num != 1985076860U)
						{
							if (num == 2058742407U)
							{
								if (text == "DELBE")
								{
									return "Del Perro Beach";
								}
							}
						}
						else if (text == "LAGO")
						{
							return "Lago Zancudo";
						}
					}
					else if (text == "PALCOV")
					{
						return "Paleto Cove";
					}
				}
				else if (num <= 3036913334U)
				{
					if (num <= 2547291213U)
					{
						if (num <= 2287073491U)
						{
							if (num <= 2169497423U)
							{
								if (num != 2094455912U)
								{
									if (num == 2169497423U)
									{
										if (text == "BURTON")
										{
											return "Burton";
										}
									}
								}
								else if (text == "DAVIS")
								{
									return "Davis";
								}
							}
							else if (num != 2207845886U)
							{
								if (num != 2274078571U)
								{
									if (num == 2287073491U)
									{
										if (text == "ZP_ORT")
										{
											return "Port of South Los Santos";
										}
									}
								}
								else if (text == "CHIL")
								{
									return "Vinewood Hills";
								}
							}
							else if (text == "RANCHO")
							{
								return "Rancho";
							}
						}
						else if (num <= 2461173245U)
						{
							if (num != 2336167997U)
							{
								if (num != 2457828765U)
								{
									if (num == 2461173245U)
									{
										if (text == "DTVINE")
										{
											return "Downtown Vinewood";
										}
									}
								}
								else if (text == "EAST_V")
								{
									return "East Vinewood";
								}
							}
							else if (text == "NCHU")
							{
								return "North Chumash";
							}
						}
						else if (num != 2523994844U)
						{
							if (num != 2527903623U)
							{
								if (num == 2547291213U)
								{
									if (text == "RGLEN")
									{
										return "Richman Glen";
									}
								}
							}
							else if (text == "RTRAK")
							{
								return "Redwood Lights Track";
							}
						}
						else if (text == "STRAW")
						{
							return "Strawberry";
						}
					}
					else if (num <= 2815358741U)
					{
						if (num <= 2716316114U)
						{
							if (num != 2707206025U)
							{
								if (num == 2716316114U)
								{
									if (text == "ARMYB")
									{
										return "Fort Zancudo";
									}
								}
							}
							else if (text == "LACT")
							{
								return "Land Act Reservoir";
							}
						}
						else if (num != 2734390602U)
						{
							if (num != 2781827306U)
							{
								if (num == 2815358741U)
								{
									if (text == "DESRT")
									{
										return "Grand Senora Desert";
									}
								}
							}
							else if (text == "PALHIGH")
							{
								return "Palomino Highlands";
							}
						}
						else if (text == "EBURO")
						{
							return "El Burro Heights";
						}
					}
					else if (num <= 2887547778U)
					{
						if (num != 2828291657U)
						{
							if (num != 2844092847U)
							{
								if (num == 2887547778U)
								{
									if (text == "MTCHIL")
									{
										return "Mount Chiliad";
									}
								}
							}
							else if (text == "ALAMO")
							{
								return "Alamo Sea";
							}
						}
						else if (text == "SLAB")
						{
							return "Stab City";
						}
					}
					else if (num != 2907672417U)
					{
						if (num != 2960458393U)
						{
							if (num == 3036913334U)
							{
								if (text == "LEGSQU")
								{
									return "Legion Square";
								}
							}
						}
						else if (text == "GALFISH")
						{
							return "Galilee";
						}
					}
					else if (text == "LOSPUER")
					{
						return "La Puerta";
					}
				}
				else if (num <= 3590084878U)
				{
					if (num <= 3324259431U)
					{
						if (num <= 3233463663U)
						{
							if (num != 3171315002U)
							{
								if (num == 3233463663U)
								{
									if (text == "MOVIE")
									{
										return "Richards Majestic";
									}
								}
							}
							else if (text == "MURRI")
							{
								return "Murrieta Heights";
							}
						}
						else if (num != 3263494647U)
						{
							if (num != 3270360039U)
							{
								if (num == 3324259431U)
								{
									if (text == "TERMINA")
									{
										return "Terminal";
									}
								}
							}
							else if (text == "HORS")
							{
								return "Vinewood Racetrack";
							}
						}
						else if (text == "CMSW")
						{
							return "Chiliad Mountain State Wilderness";
						}
					}
					else if (num <= 3464048059U)
					{
						if (num != 3415255848U)
						{
							if (num != 3446966466U)
							{
								if (num == 3464048059U)
								{
									if (text == "ZANCUDO")
									{
										return "Zancudo River";
									}
								}
							}
							else if (text == "CALAFB")
							{
								return "Calafia Bridge";
							}
						}
						else if (text == "BEACH")
						{
							return "Vespucci Beach";
						}
					}
					else if (num != 3474815123U)
					{
						if (num != 3515545285U)
						{
							if (num == 3590084878U)
							{
								if (text == "WVINE")
								{
									return "West Vinewood";
								}
							}
						}
						else if (text == "TATAMO")
						{
							return "Tataviam Mountains";
						}
					}
					else if (text == "MTJOSE")
					{
						return "Mount Josiah";
					}
				}
				else if (num <= 4076068080U)
				{
					if (num <= 3708773937U)
					{
						if (num != 3592116603U)
						{
							if (num != 3695191777U)
							{
								if (num == 3708773937U)
								{
									if (text == "MORN")
									{
										return "Morningwood";
									}
								}
							}
							else if (text == "BHAMCA")
							{
								return "Banham Canyon";
							}
						}
						else if (text == "LDAM")
						{
							return "Land Act Dam";
						}
					}
					else if (num != 3989457552U)
					{
						if (num != 3999718073U)
						{
							if (num == 4076068080U)
							{
								if (text == "BANNING")
								{
									return "Banning";
								}
							}
						}
						else if (text == "golf")
						{
							return "GWC and Golfing Society";
						}
					}
					else if (text == "PBOX")
					{
						return "Pillbox Hill";
					}
				}
				else if (num <= 4151881961U)
				{
					if (num != 4083182070U)
					{
						if (num != 4135511409U)
						{
							if (num == 4151881961U)
							{
								if (text == "LMESA")
								{
									return "La Mesa";
								}
							}
						}
						else if (text == "JAIL")
						{
							return "Bolingbroke Penitentiary";
						}
					}
					else if (text == "ROCKF")
					{
						return "Rockford Hills";
					}
				}
				else if (num != 4214586325U)
				{
					if (num != 4239449422U)
					{
						if (num == 4255991879U)
						{
							if (text == "ALTA")
							{
								return "Alta";
							}
						}
					}
					else if (text == "SANDY")
					{
						return "Sandy Shores";
					}
				}
				else if (text == "BANHAMC")
				{
					return "Banham Canyon";
				}
			}
			return "null";
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00003DCC File Offset: 0x00001FCC
		internal void DistanceWatcher()
		{
			this.SyncCall();
			while (EManager.currentCallState != EManager.CallState.Arrived)
			{
				if (Vector3.Distance(EManager.limoM.Position, EManager.protaM.Position) > 40f && EManager.currentCallState != EManager.CallState.Parking && !EManager.isPlayerDrivingLimo)
				{
					Game.DisplaySubtitle("Stay close to the convoy to keep the team formation.", 3000);
					GameFiber.Wait(3000);
				}
				GameFiber.Yield();
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003E36 File Offset: 0x00002036
		internal void ExternalEvent()
		{
			for (;;)
			{
				GameFiber.Yield();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003E40 File Offset: 0x00002040
		internal void GenerateRandomEnemies()
		{
			EManager.enemyCreated = false;
			this.SyncCall();
			while (EManager.currentCallState != EManager.CallState.Parking)
			{
				try
				{
					GameFiber.Sleep(new Random().Next(50000, 100000));
					if (new Random().Next(1, 4) == 1 && !EManager.enemyCreated && Vector3.Distance(EManager.arrival, EManager.limoM.Position) > 200f && Vector3.Distance(EManager.departure, EManager.limoM.Position) > 170f)
					{
						Game.LogTrivial("!");
						EManager.enemyCreated = true;
						Vehicle v = new Vehicle(EManager.vA.ElementAt(new Random().Next(1, EManager.vA.Count)), World.GetNextPositionOnStreet(EManager.limoM.Position + new Vector3(70f, 20f, 0f)));
						Ped p1 = new Ped("G_M_Y_Lost_01", Game.LocalPlayer.Character.Position + new Vector3(50f, 50f, 0f), 0f);
						p1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 45, true);
						p1.WarpIntoVehicle(v, -1);
						Ped p2 = new Ped("G_M_Y_Lost_02", Game.LocalPlayer.Character.Position + new Vector3(50f, 50f, 0f), 0f);
						p2.WarpIntoVehicle(v, 0);
						p2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 50, true);
						p1.Tasks.DriveToPosition(EManager.limoM.Position, 50f, 262710);
						p1.RelationshipGroup = "ATTACKERS";
						p2.RelationshipGroup = "ATTACKERS";
						Game.SetRelationshipBetweenRelationshipGroups(p2.RelationshipGroup, EManager.p_limoM.RelationshipGroup, 5);
						GameFiber.StartNew(delegate()
						{
							this.watcherA(v, p1, p2);
						});
						GameFiber.Sleep(60000);
						EManager.enemyCreated = false;
					}
				}
				catch (Exception)
				{
				}
			}
			GameFiber.Hibernate();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000411C File Offset: 0x0000231C
		internal void GenerateEnemiesWithZone()
		{
			EManager.enemyCreated = false;
			this.SyncCall();
			while (EManager.currentCallState != EManager.CallState.Parking)
			{
				try
				{
					if (EManager.riskLevel == "~r~High")
					{
						GameFiber.Wait(18000);
						if (EManager.riskLevel == "~r~High" && !EManager.enemyCreated)
						{
							EManager.enemyCreated = true;
							Vehicle v = new Vehicle(EManager.vA.ElementAt(new Random().Next(1, EManager.vA.Count)), World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position + new Vector3(50f, 50f, 0f)));
							Ped p1 = new Ped("G_M_Y_BallaOrig_01", Game.LocalPlayer.Character.Position + new Vector3(50f, 50f, 0f), 0f);
							p1.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 55, true);
							p1.WarpIntoVehicle(v, -1);
							p1.MakePersistent();
							Ped p2 = new Ped("G_M_Y_BallaSout_01", Game.LocalPlayer.Character.Position + new Vector3(50f, 50f, 0f), 0f);
							p2.WarpIntoVehicle(v, 0);
							p2.MakePersistent();
							p2.Inventory.GiveNewWeapon(EManager.vWeaponsList.ElementAt(new Random().Next(1, EManager.vWeaponsList.Count)), 100, true);
							p1.Tasks.DriveToPosition(EManager.limoM.Position, 50f, 262710);
							p1.RelationshipGroup = "ATTACKERS";
							p2.RelationshipGroup = "ATTACKERS";
							Game.SetRelationshipBetweenRelationshipGroups(p2.RelationshipGroup, EManager.p_limoM.RelationshipGroup, 5);
							GameFiber.StartNew(delegate()
							{
								this.watcherA(v, p1, p2);
							});
							GameFiber.Sleep(60000);
							EManager.enemyCreated = false;
						}
					}
				}
				catch (Exception)
				{
					continue;
				}
				GameFiber.Yield();
			}
			GameFiber.Hibernate();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000043D8 File Offset: 0x000025D8
		private void watcherA(Vehicle v, Ped p1, Ped p2)
		{
			Blip blip = v.AttachBlip();
			blip.Sprite = 270;
			blip.Color = Color.Red;
			if (!Main.shBlips)
			{
				blip.Alpha = 0f;
			}
			try
			{
				while (Vector3.Distance(p1.Position, EManager.limoM.Position) > 28f && EManager.currentCallState != EManager.CallState.Arrived && Vector3.Distance(v.Position, EManager.limoM.Position) < 350f)
				{
					p1.Tasks.DriveToPosition(EManager.limoM.Position, 35f, 262710);
					GameFiber.Sleep(2000);
				}
				NativeFunction.CallByName<uint>("TASK_VEHICLE_CHASE", new NativeArgument[]
				{
					p1,
					EManager.limoM
				});
			}
			catch (Exception)
			{
			}
			try
			{
				while (p1.IsAlive && p2.IsAlive && EManager.currentCallState != EManager.CallState.Arrived && Vector3.Distance(v.Position, EManager.limoM.Position) < 300f)
				{
					GameFiber.Yield();
				}
			}
			catch (Exception)
			{
			}
			EManager.enemyCreated = false;
			try
			{
				blip.Delete();
			}
			catch (Exception)
			{
			}
			try
			{
				if (EntityExtensions.Exists(p1) && p1.IsValid() && p1.IsAlive)
				{
					p1.Dismiss();
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (EntityExtensions.Exists(p2) && p2.IsValid() && p2.IsAlive)
				{
					p2.Dismiss();
				}
			}
			catch (Exception)
			{
			}
			try
			{
				if (EntityExtensions.Exists(v) && v.IsValid())
				{
					v.Dismiss();
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000045B0 File Offset: 0x000027B0
		internal void ManageRiskLevel()
		{
			this.SyncCall();
			while (EManager.currentCallState != EManager.CallState.Arrived)
			{
				if (EManager.dangerAreas.Contains(EManager.ZoneName(Game.LocalPlayer.Character.Position)))
				{
					EManager.riskLevel = "~r~High";
				}
				else
				{
					EManager.riskLevel = "~y~Medium";
				}
				GameFiber.Sleep(6000);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00004610 File Offset: 0x00002810
		internal static void Mod_Controls1(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0.915f,
				0f,
				0.2f,
				0.17f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("If one of the vehicles of the convoy is stuck, press ~b~Shift~w~ + ~b~Y~w~.", 0.003f, 0.83f, 0.5f);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000046C4 File Offset: 0x000028C4
		internal static void Mod_Controls2(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0.915f,
				0f,
				0.2f,
				0.17f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("If the limo is having trouble to park correctly, press ~b~Shift~w~ + ~b~Y~w~.", 0.003f, 0.83f, 0.5f);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004778 File Offset: 0x00002978
		internal static void Mod_Credits(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0.935f,
				0f,
				0.15f,
				0.28f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("~g~LS : Protection Squad", 0.003f, 0.875f, 0.5f);
			EManager.Text("Coded by ~b~Connor.S", 0.033f, 0.875f, 0.5f);
			EManager.Text("Version ~y~2.0~w~ (30/07/2020)", 0.063f, 0.875f, 0.5f);
			EManager.Text("~o~Release", 0.1f, 0.875f, 0.5f);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004878 File Offset: 0x00002A78
		public static void Game_FrameRender(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.274f,
				0.2f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("VIP : ~b~" + EManager.currentVipName, 0.003f, 0.003f, 0.5f);
			if (Main.shRisk)
			{
				EManager.Text("Distance : " + ((int)Vector3.Distance(Game.LocalPlayer.Character.Position, EManager.arrival)).ToString() + " yards", 0.063f, 0.003f, 0.5f);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00004984 File Offset: 0x00002B84
		public static void Game_FrameRender_SP1(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.274f,
				0.2f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("Inmate : ~b~" + SCALL_Manager.currentpName, 0.003f, 0.003f, 0.5f);
			if (Main.shRisk)
			{
				EManager.Text("Risk Level : " + EManager.riskLevel, 0.033f, 0.003f, 0.5f);
			}
			EManager.Text("Distance : " + ((int)Vector3.Distance(Game.LocalPlayer.Character.Position, EManager.arrival)).ToString() + " yards", 0.063f, 0.003f, 0.5f);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00004AB4 File Offset: 0x00002CB4
		public static void Game_FrameRenderRes(object sender, GraphicsEventArgs e)
		{
			NativeFunction.CallByName<uint>("DRAW_RECT", new NativeArgument[]
			{
				0f,
				0f,
				0.45f,
				0.3f,
				(int)Color.Black.R,
				(int)Color.Black.G,
				(int)Color.Black.B,
				105
			});
			EManager.Text("VIP : ~b~" + EManager.currentVipName, 0.003f, 0.003f, 0.5f);
			EManager.Text("Profession : " + EManager.currentVipJobs, 0.033f, 0.003f, 0.5f);
			EManager.Text("Destination : " + EManager.currentDestination, 0.063f, 0.003f, 0.5f);
			EManager.Text(EManager.reason, 0.103f, 0.001f, 0.5f);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00004BD4 File Offset: 0x00002DD4
		internal static void Text(string text, float x, float y, float scale)
		{
			if (EManager.<>o__52.<>p__0 == null)
			{
				EManager.<>o__52.<>p__0 = CallSite<Action<CallSite, object, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextFont", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__0.Target(EManager.<>o__52.<>p__0, NativeFunction.Natives, 4);
			if (EManager.<>o__52.<>p__1 == null)
			{
				EManager.<>o__52.<>p__1 = CallSite<Action<CallSite, object, float, float>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextScale", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			EManager.<>o__52.<>p__1.Target(EManager.<>o__52.<>p__1, NativeFunction.Natives, scale, scale);
			NativeFunction.CallByName<uint>("SET_TEXT_COLOUR", new NativeArgument[]
			{
				(int)Color.White.R,
				(int)Color.White.G,
				(int)Color.White.B,
				255
			});
			if (EManager.<>o__52.<>p__2 == null)
			{
				EManager.<>o__52.<>p__2 = CallSite<Action<CallSite, object, float, float>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextWrap", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__2.Target(EManager.<>o__52.<>p__2, NativeFunction.Natives, 0f, 1f);
			if (EManager.<>o__52.<>p__3 == null)
			{
				EManager.<>o__52.<>p__3 = CallSite<Action<CallSite, object, bool>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextCentre", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__3.Target(EManager.<>o__52.<>p__3, NativeFunction.Natives, false);
			if (EManager.<>o__52.<>p__4 == null)
			{
				EManager.<>o__52.<>p__4 = CallSite<Action<CallSite, object, int, int, int, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextDropshadow", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__4.Target(EManager.<>o__52.<>p__4, NativeFunction.Natives, 2, 2, 0, 0, 0);
			if (EManager.<>o__52.<>p__5 == null)
			{
				EManager.<>o__52.<>p__5 = CallSite<Action<CallSite, object, int, int, int, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextEdge", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__5.Target(EManager.<>o__52.<>p__5, NativeFunction.Natives, 1, 0, 0, 0, 205);
			if (EManager.<>o__52.<>p__6 == null)
			{
				EManager.<>o__52.<>p__6 = CallSite<Action<CallSite, object, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetTextLeading", null, typeof(EManager), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				}));
			}
			EManager.<>o__52.<>p__6.Target(EManager.<>o__52.<>p__6, NativeFunction.Natives, 1);
			NativeFunction.CallByHash<uint>(2736978246810207435UL, new NativeArgument[]
			{
				"STRING"
			});
			NativeFunction.CallByHash<uint>(2736978246810207435UL, new NativeArgument[]
			{
				text
			});
			NativeFunction.CallByHash<uint>(7789129354908300458UL, new NativeArgument[]
			{
				text
			});
			NativeFunction.CallByHash<uint>(14772192000654010967UL, new NativeArgument[]
			{
				y,
				x
			});
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00004FD6 File Offset: 0x000031D6
		public static double ConvertMilesToMeters(double distance)
		{
			return Math.Round(distance * 1609.34, 2);
		}

		// Token: 0x04000005 RID: 5
		public static EManager.Calls currentCall = EManager.Calls.None;

		// Token: 0x04000006 RID: 6
		public static EManager.CallType currentCallType = EManager.CallType.None;

		// Token: 0x04000007 RID: 7
		public static EManager.CallState currentCallState = EManager.CallState.None;

		// Token: 0x04000008 RID: 8
		public static string riskLevel = "~y~Medium";

		// Token: 0x04000009 RID: 9
		public static List<string> vA = new List<string>(new string[]
		{
			"EMPEROR",
			"INTRUDER",
			"PRIMO2",
			"BISON",
			"CHINO2",
			"BAGGER",
			"BALLER"
		});

		// Token: 0x0400000A RID: 10
		public static List<string> vipNames = new List<string>(new string[]
		{
			"Frank Javignano",
			"Sir Isaac Newton",
			"Michael de Santa",
			"Dave Thomas",
			"Romeo Blackwell",
			"Ryan Spencer",
			"Julia Miller",
			"Fei Zongmeng",
			"Cris Formage",
			"Steven McAlone",
			"Ewan Craig",
			"Bill Vaughn",
			"Michael Graves",
			"Oscar Horton",
			"Dan Terry",
			"Christopher Francis",
			"Kameron Charles",
			"Warren Rhodes",
			"Lauren Baxter",
			"Yi Hyung Joon",
			"Karen Daniels",
			"Lazlow",
			"Molly Schultz"
		});

		// Token: 0x0400000B RID: 11
		public static List<string> vipJobs = new List<string>(new string[]
		{
			"YouTuber",
			"Funds Merryweather",
			"Film Producer",
			"Professional Football Player",
			"CEO of AirEmu",
			"CEO of Albany",
			"Running for President",
			"Chinese Secretary of Foreign Affairs",
			"Leader of Epsilon Program",
			"CEO of Benefactor",
			"CEO of Badger",
			"Mayor of Los Santos",
			"Running for President",
			"President of the National Security Council",
			"US Trade Representative",
			"State Governor of San Andreas",
			"Department of Defense Secretary",
			"Billionaire",
			"Second Lady of the United States",
			"South Korean Prime Minister",
			"IAA Executive",
			"TV Presenter & radio host",
			"Senior Vice President of Merryweather"
		});

		// Token: 0x0400000C RID: 12
		public static List<string> reasonList = new List<string>(new string[]
		{
			"~r~Received threatning phone calls 2 days ago",
			"~r~FBI recommends high degree of caution",
			"~r~Important popularity decrease last month",
			"~r~Targeted by renowned criminals",
			"~g~Popularity increased over the month",
			"~y~Is going to do an important speech",
			"~r~Is going to criticize an powerful brand",
			"~r~Often buys crack to LS gangs",
			"~r~Targeted by foreign terrorist groups",
			"~r~Popularity decreasing",
			"~g~Popularity increased over the month",
			"~g~Shows support to a charity association",
			"~g~Appreciated by the current POTUS",
			"~r~Known to be corrupted",
			"~r~Targeted by violent LS gangs",
			"~r~Despised by powerful CEOs",
			"~r~Had an assassination attempt in the past",
			"~g~Is well-covered by secret services"
		});

		// Token: 0x0400000D RID: 13
		public static List<string> vWeaponsList = new List<string>(new string[]
		{
			"WEAPON_PISTOL",
			"WEAPON_COMBATPISTOL",
			"WEAPON_APPISTOL",
			"WEAPON_PISTOL50",
			"WEAPON_ASSAULTSMG",
			"WEAPON_ADVANCEDRIFLE",
			"WEAPON_PUMPSHOTGUN",
			"WEAPON_BULLPUPSHOTGUN",
			"WEAPON_STUNGUN",
			"WEAPON_HEAVYPISTOL",
			"WEAPON_MUSKET",
			"WEAPON_ASSAULTRIFLE",
			"WEAPON_ASSAULTSMG",
			"WEAPON_COMBATPDW"
		});

		// Token: 0x0400000E RID: 14
		internal static string currentVipName;

		// Token: 0x0400000F RID: 15
		internal static string currentVipJobs;

		// Token: 0x04000010 RID: 16
		internal static string currentDestination;

		// Token: 0x04000011 RID: 17
		private static string reason;

		// Token: 0x04000012 RID: 18
		private static List<string> dangerAreas = new List<string>(new string[]
		{
			"Chamberlain Hills",
			"Davis",
			"East Vinewood",
			"El Burro Heights",
			"Harmony",
			"Hawick",
			"Little Seoul",
			"La Mesa",
			"Murrieta Heights",
			"Rancho",
			"Sandy Shores",
			"Strawberry"
		});

		// Token: 0x04000013 RID: 19
		internal static string vipModel;

		// Token: 0x04000014 RID: 20
		private static bool enemyCreated;

		// Token: 0x04000015 RID: 21
		internal static Vehicle limoM;

		// Token: 0x04000016 RID: 22
		internal static Vehicle fbi1M;

		// Token: 0x04000017 RID: 23
		internal static Vehicle fbi2M;

		// Token: 0x04000018 RID: 24
		public static Vector3 departure;

		// Token: 0x04000019 RID: 25
		public static Vector3 arrival;

		// Token: 0x0400001A RID: 26
		internal static bool isPlayerDrivingLimo;

		// Token: 0x0400001B RID: 27
		internal static Vehicle protaM;

		// Token: 0x0400001C RID: 28
		internal static Ped p_limoM;

		// Token: 0x0400001D RID: 29
		internal static Ped vipM;

		// Token: 0x0400001E RID: 30
		private Ped p_fbi1M;

		// Token: 0x0400001F RID: 31
		private Ped p_fbi2M;

		// Token: 0x04000020 RID: 32
		private bool isMissionFailed;

		// Token: 0x04000021 RID: 33
		private bool thirdDriverDead;

		// Token: 0x04000022 RID: 34
		private bool fourthDriverDead;

		// Token: 0x0200002A RID: 42
		public enum Calls
		{
			// Token: 0x040001F8 RID: 504
			LSAtoARC,
			// Token: 0x040001F9 RID: 505
			ARCtoGOC,
			// Token: 0x040001FA RID: 506
			LSAtoNOO,
			// Token: 0x040001FB RID: 507
			KORtoGAL,
			// Token: 0x040001FC RID: 508
			LSAtoGAL,
			// Token: 0x040001FD RID: 509
			LSAtoUD,
			// Token: 0x040001FE RID: 510
			PCALL_DWT_BBP,
			// Token: 0x040001FF RID: 511
			MZBtoNOO,
			// Token: 0x04000200 RID: 512
			LSAtoMZB,
			// Token: 0x04000201 RID: 513
			UDPtoGOC,
			// Token: 0x04000202 RID: 514
			NOOtoMZB,
			// Token: 0x04000203 RID: 515
			KORtoMZB,
			// Token: 0x04000204 RID: 516
			GALtoRMS,
			// Token: 0x04000205 RID: 517
			SCALL_1,
			// Token: 0x04000206 RID: 518
			PCALL_VSP_BBP,
			// Token: 0x04000207 RID: 519
			Custom,
			// Token: 0x04000208 RID: 520
			None
		}

		// Token: 0x0200002B RID: 43
		public enum CallType
		{
			// Token: 0x0400020A RID: 522
			Regular,
			// Token: 0x0400020B RID: 523
			Prison,
			// Token: 0x0400020C RID: 524
			Ambulance,
			// Token: 0x0400020D RID: 525
			Special,
			// Token: 0x0400020E RID: 526
			None
		}

		// Token: 0x0200002C RID: 44
		public enum CallState
		{
			// Token: 0x04000210 RID: 528
			Initiated,
			// Token: 0x04000211 RID: 529
			EscortInProgress,
			// Token: 0x04000212 RID: 530
			Parking,
			// Token: 0x04000213 RID: 531
			Arrived,
			// Token: 0x04000214 RID: 532
			None
		}
	}
}
