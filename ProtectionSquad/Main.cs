using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using LSPD_First_Response.Mod.API;
using Microsoft.CSharp.RuntimeBinder;
using Protection_Squad.Callouts;
using Rage;
using Rage.Native;

namespace Protection_Squad
{
	// Token: 0x02000005 RID: 5
	public class Main : Plugin
	{
		// Token: 0x06000023 RID: 35 RVA: 0x000054A4 File Offset: 0x000036A4
		public override void Finally()
		{
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000054A8 File Offset: 0x000036A8
		public override void Initialize()
		{
			Game.Console.Print("Loading LSPS config files...");
			string productVersion = FileVersionInfo.GetVersionInfo("RAGEPluginHook.exe").ProductVersion;
			GameConsole console = Game.Console;
			string str = "RPH version ";
			string str2 = productVersion;
			string str3 = " + game version ";
			Version productVersion2 = Game.ProductVersion;
			console.Print(str + str2 + str3 + ((productVersion2 != null) ? productVersion2.ToString() : null));
			if (!File.Exists("Plugins/LSPDFR/ProtectionSquad.ini"))
			{
				Game.DisplayNotification("~r~Unable to find the file : ProtectionSquad.ini. Please reinstall the plugin.");
				Game.LogTrivial("Unable to find ProtectionSquad.ini on Plugins/LSPDFR.");
				throw new FileNotFoundException();
			}
			Game.Console.Print("Fetching config files...");
			InitializationFile initializationFile = new InitializationFile("Plugins/LSPDFR/ProtectionSquad.ini");
			initializationFile.Create();
			Game.Console.Print("Reading .ini file...");
			Main.vPatrol1 = initializationFile.ReadString("MOTORCADE SETTINGS", "limoModel", null);
			Main.vPatrol2 = initializationFile.ReadString("MOTORCADE SETTINGS", "firstBackupVehicleModel", null);
			Main.vPatrol3 = initializationFile.ReadString("MOTORCADE SETTINGS", "secondBackupVehicleModel", null);
			Main.pedModel1 = initializationFile.ReadString("MOTORCADE SETTINGS", "limoDriver", null);
			Main.pedModel2 = initializationFile.ReadString("MOTORCADE SETTINGS", "firstBackupDriver", null);
			Main.pedModel1 = initializationFile.ReadString("MOTORCADE SETTINGS", "secondBackupDriver", null);
			Main.muteSirens = Convert.ToBoolean(initializationFile.ReadString("MOTORCADE SETTINGS", "muteSirens", null));
			Main.shRisk = Convert.ToBoolean(initializationFile.ReadString("DIFFICULTY", "DisplayDangerLevel", null));
			Main.shBlips = Convert.ToBoolean(initializationFile.ReadString("DIFFICULTY", "ShowBlips", null));
			Game.Console.Print(" ");
			Game.Console.Print("                                                 [Los Santos Protection Squad 2.0]");
			Game.Console.Print(" ");
			Game.Console.Print("-> Plugin initialized.");
			Game.Console.Print(" ");
			Functions.OnOnDutyStateChanged += new Functions.OnDutyStateChangedEventHandler(Main.Functions_OnOnDutyStateChanged);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00005684 File Offset: 0x00003884
		private static void Functions_OnOnDutyStateChanged(bool onDuty)
		{
			if (onDuty)
			{
				Main.RegisterCallouts();
				Game.Console.Print("_____________________________________________________________________");
				Game.Console.Print("                                                 [Los Santos Protection Squad 2.0]");
				Game.Console.Print("Your candidacy as a motorcade driver has been accepted.");
				Game.Console.Print("© Connor.S - 30/07/2020");
				Game.Console.Print("---------------------------------------------------------------------------------------------------------");
				Game.FrameRender += EManager.Mod_Credits;
				if (Main.<>o__12.<>p__0 == null)
				{
					Main.<>o__12.<>p__0 = CallSite<Action<CallSite, object, string, bool>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "RequestStreamedTextureDict", null, typeof(Main), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Main.<>o__12.<>p__0.Target(Main.<>o__12.<>p__0, NativeFunction.Natives, "heisthud", true);
				GameFiber.Sleep(8000);
				Game.FrameRender -= EManager.Mod_Credits;
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00005784 File Offset: 0x00003984
		public static void RegisterCallouts()
		{
			Functions.RegisterCallout(typeof(Airport_to_Arcadius_Center));
			Functions.RegisterCallout(typeof(Airport_to_Noose_HQ));
			Functions.RegisterCallout(typeof(Airport_to_Observatory));
			Functions.RegisterCallout(typeof(Airport_to_Union_Depository));
			Functions.RegisterCallout(typeof(Ambulance_Escort_Bollingbrook_to_Davis));
			Functions.RegisterCallout(typeof(Kortz_Center_to_Maze_Bank));
			Functions.RegisterCallout(typeof(Kortz_Center_to_Observatory));
			Functions.RegisterCallout(typeof(Arcadius_Center_to_Golf_Club));
			Functions.RegisterCallout(typeof(Maze_Bank_to_Airport));
			Functions.RegisterCallout(typeof(EMaze_Bank_to_Noose_HQ));
			Functions.RegisterCallout(typeof(Noose_HQ_To_Maze_Bank));
			Functions.RegisterCallout(typeof(Observatory_to_Richards_Majestic));
			Functions.RegisterCallout(typeof(Prisoner_Transport_Vespucci_to_Bollingbrook));
			Functions.RegisterCallout(typeof(Union_Depository_to_Golf_Club));
		}

		// Token: 0x04000025 RID: 37
		public static string vPatrol1;

		// Token: 0x04000026 RID: 38
		public static string vPatrol2;

		// Token: 0x04000027 RID: 39
		public static string vPatrol3;

		// Token: 0x04000028 RID: 40
		public static bool shBlips;

		// Token: 0x04000029 RID: 41
		public static bool shRisk;

		// Token: 0x0400002A RID: 42
		public static bool muteSirens;

		// Token: 0x0400002B RID: 43
		public static string pedModel1;

		// Token: 0x0400002C RID: 44
		public static string pedModel2;

		// Token: 0x0400002D RID: 45
		public static string pedModel3;
	}
}
