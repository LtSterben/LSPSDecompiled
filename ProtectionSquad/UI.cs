using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace Protection_Squad
{
	// Token: 0x02000009 RID: 9
	internal class UI
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00006014 File Offset: 0x00004214
		public static void EscortInfo()
		{
			GameFiber.StartNew(new ThreadStart(UI.PrivateGenerateMenu));
			UI.canReturn = false;
			while (!UI.canReturn)
			{
				GameFiber.Yield();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000603C File Offset: 0x0000423C
		private static void PrivateGenerateMenu()
		{
			UI._menuPool = new MenuPool();
			UI.myMenu = new UIMenu("Escort Data", "~r~LSPS Convoy - Infos");
			Sprite bannerType = new Sprite("commonmenu", "bgd_interaction", new Point(100, 20), new Size(200, 500), 0f, Color.Red);
			UI.myMenu.SetBannerType(bannerType);
			UI._menuPool.Add(UI.myMenu);
			UI.isHeliRequested = false;
			UI.canReturn = false;
			Game.FrameRender += UI.Process;
			UI.myMenu.RefreshIndex();
			UI.myMenu.Visible = true;
			string text = "null";
			if (EManager.currentCallType == EManager.CallType.Regular)
			{
				text = "Escort Type : VIP Escort";
			}
			if (EManager.currentCallType == EManager.CallType.Ambulance)
			{
				text = "Escort Type : Ambulance Escort";
			}
			if (EManager.currentCallType == EManager.CallType.Prison)
			{
				text = "Escort Type : Prisoner Transfer";
			}
			if (EManager.currentCallType == EManager.CallType.Special)
			{
				text = "Escort Type : Special Mission";
			}
			if (EManager.currentCallType == EManager.CallType.Regular)
			{
				UI.myMenu.AddItem(new UIMenuItem("VIP : " + EManager.currentVipName, "This is the VIP you'll be transporting."));
				UI.myMenu.AddItem(new UIMenuItem("VIP Job : " + EManager.currentVipJobs, "Adjust the vehicles & heli backup according to the level of importance of the VIP."));
			}
			UI.myMenu.AddItem(new UIMenuItem(text));
			if (EManager.currentCallType == EManager.CallType.Regular)
			{
				UI.myMenu.AddItem(UI.limoList = new UIMenuListItem("Limo Model", UI.limoModel, 0, "Change the model of the limo."));
			}
			if (EManager.currentCallType == EManager.CallType.Regular || EManager.currentCallType == EManager.CallType.Prison)
			{
				UI.myMenu.AddItem(UI.fbi1List = new UIMenuListItem("1st Rear Vehicle", UI.fbi1Model, 0, "Change the model of the first rear vehicle."));
			}
			if (EManager.currentCallType == EManager.CallType.Regular)
			{
				UI.myMenu.AddItem(UI.fbi2List = new UIMenuListItem("2nd Rear Vehicle", UI.fbi1Model, 0, "Change the model of the second rear vehicle."));
			}
			UI.myMenu.AddItem(UI.heliCbx = new UIMenuCheckboxItem("Request Helicopter ?", false, "Request an heli that will follow the convoy to provide extra security."));
			UI.myMenu.AddItem(UI.cValidate = new UIMenuItem("Validate", "Start the convoy."));
			UI.myMenu.OnMenuClose += new MenuCloseEvent(UI.OnMenuClose);
			UI.myMenu.OnItemSelect += new ItemSelectEvent(UI.OnItemSelect);
			UI.myMenu.OnListChange += new ListChangedEvent(UI.OnListChange);
			UI.myMenu.OnCheckboxChange += new CheckboxChangeEvent(UI.OnCheckboxChange);
			UI.myMenu.ResetKey(5);
			UI.myMenu.OnIndexChange += new IndexChangedEvent(UI.OnItemChange);
			UI.myMenu.RefreshIndex();
			UI.myMenu.SetKey(0, 172);
			UI.myMenu.SetKey(1, 173);
			UI.myMenu.SetKey(2, 174);
			UI.myMenu.SetKey(3, 175);
			UI.myMenu.SetKey(4, 176);
			UI.myMenu.SetKey(5, 304);
			UI.myMenu.MouseControlsEnabled = false;
			UI.myMenu.AllowCameraMovement = true;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000054A4 File Offset: 0x000036A4
		private static void OnMenuClose(UIMenu sender)
		{
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00006344 File Offset: 0x00004544
		private static void OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
		{
			if (sender != UI.myMenu)
			{
				return;
			}
			if (listItem == UI.limoList)
			{
				GameFiber.StartNew(delegate()
				{
					UI.Limo_E(newIndex);
				});
			}
			if (listItem == UI.fbi1List)
			{
				GameFiber.StartNew(delegate()
				{
					UI.FBI1_E(newIndex);
				});
			}
			if (listItem == UI.fbi2List)
			{
				GameFiber.StartNew(delegate()
				{
					UI.FBI2_E(newIndex);
				});
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000063B8 File Offset: 0x000045B8
		private static void Limo_E(int newIndex)
		{
			Vector3 position = EManager.limoM.Position;
			float heading = EManager.limoM.Heading;
			Ped driver = EManager.limoM.Driver;
			EManager.limoM.Delete();
			if (UI.<>o__17.<>p__0 == null)
			{
				UI.<>o__17.<>p__0 = CallSite<Func<CallSite, Type, object, Vector3, float, Vehicle>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof(UI), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			EManager.limoM = UI.<>o__17.<>p__0.Target(UI.<>o__17.<>p__0, typeof(Vehicle), UI.CorLimoModel[newIndex], position, heading);
			driver.WarpIntoVehicle(EManager.limoM, -1);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000647C File Offset: 0x0000467C
		private static void FBI1_E(int newIndex)
		{
			EManager.fbi1M.MakePersistent();
			Vector3 position = EManager.fbi1M.Position;
			float heading = EManager.fbi1M.Heading;
			Ped driver = EManager.fbi1M.Driver;
			EManager.fbi1M.Delete();
			if (UI.<>o__18.<>p__0 == null)
			{
				UI.<>o__18.<>p__0 = CallSite<Func<CallSite, Type, object, Vector3, float, Vehicle>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof(UI), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			EManager.fbi1M = UI.<>o__18.<>p__0.Target(UI.<>o__18.<>p__0, typeof(Vehicle), UI.CorFBI1Model[newIndex], position, heading);
			driver.WarpIntoVehicle(EManager.fbi1M, -1);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000654C File Offset: 0x0000474C
		private static void FBI2_E(int newIndex)
		{
			Vector3 position = EManager.fbi2M.Position;
			float heading = EManager.fbi2M.Heading;
			Ped driver = EManager.fbi2M.Driver;
			EManager.fbi2M.Delete();
			if (UI.<>o__19.<>p__0 == null)
			{
				UI.<>o__19.<>p__0 = CallSite<Func<CallSite, Type, object, Vector3, float, Vehicle>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof(UI), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			EManager.fbi2M = UI.<>o__19.<>p__0.Target(UI.<>o__19.<>p__0, typeof(Vehicle), UI.CorFBI1Model[newIndex], position, heading);
			driver.WarpIntoVehicle(EManager.fbi2M, -1);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000054A4 File Offset: 0x000036A4
		private static void OnItemChange(UIMenu sender, int newIndex)
		{
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00006610 File Offset: 0x00004810
		private static void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked)
		{
			if (sender != UI.myMenu || checkboxItem != UI.heliCbx)
			{
				return;
			}
			Game.DisplayNotification("~r~Helicopter : ~b~" + Checked.ToString());
			UI.isHeliRequested = Checked;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00006640 File Offset: 0x00004840
		private static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
		{
			if (sender != UI.myMenu)
			{
				return;
			}
			UI.myMenu.Visible = true;
			if (selectedItem == UI.cValidate)
			{
				UI._menuPool.CloseAllMenus();
				UI.myMenu.OnCheckboxChange -= new CheckboxChangeEvent(UI.OnCheckboxChange);
				UI.myMenu.OnIndexChange -= new IndexChangedEvent(UI.OnItemChange);
				if (UI.isHeliRequested)
				{
					GameFiber.StartNew(new ThreadStart(Backup.RequestHeli));
				}
				UI.canReturn = true;
				UI.myMenu.OnListChange -= new ListChangedEvent(UI.OnListChange);
				Game.FrameRender -= UI.Process;
				UI.myMenu.Visible = false;
				UI.myMenu.OnItemSelect -= new ItemSelectEvent(UI.OnItemSelect);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00006709 File Offset: 0x00004909
		private static void Process(object sender, GraphicsEventArgs e)
		{
			UI._menuPool.ProcessMenus();
		}

		// Token: 0x0400003B RID: 59
		private static UIMenuItem cValidate;

		// Token: 0x0400003C RID: 60
		private static UIMenuCheckboxItem heliCbx;

		// Token: 0x0400003D RID: 61
		private static UIMenu myMenu;

		// Token: 0x0400003E RID: 62
		private static MenuPool _menuPool;

		// Token: 0x0400003F RID: 63
		public static bool isHeliRequested = false;

		// Token: 0x04000040 RID: 64
		public static bool canReturn = false;

		// Token: 0x04000041 RID: 65
		private static UIMenuListItem limoList;

		// Token: 0x04000042 RID: 66
		[Dynamic(new bool[]
		{
			false,
			true
		})]
		private static List<dynamic> CorLimoModel = new List<object>
		{
			"COGNOSCENTI",
			"COGNOSCENTI2",
			"COG55",
			"COG552",
			"SCHAFTER6",
			"RIOT"
		};

		// Token: 0x04000043 RID: 67
		[Dynamic(new bool[]
		{
			false,
			true
		})]
		private static List<dynamic> CorFBI1Model = new List<object>
		{
			"FBI2",
			"FBI",
			"POLICE4",
			"RIOT",
			"POLICEB",
			"SHERIFF2",
			"SHERIFF",
			"POLICE3",
			"tribike2"
		};

		// Token: 0x04000044 RID: 68
		[Dynamic(new bool[]
		{
			false,
			true
		})]
		private static List<dynamic> limoModel = new List<object>
		{
			"Cognoscenti",
			"Armored Cognoscenti",
			"Cognoscenti 55",
			"Armored Cognoscenti 55",
			"Schafter LWB (Armored)",
			"Riot Van"
		};

		// Token: 0x04000045 RID: 69
		[Dynamic(new bool[]
		{
			false,
			true
		})]
		private static List<dynamic> fbi1Model = new List<object>
		{
			"Unmarked SUV",
			"Unmarked FIB",
			"Unmarked Cruiser",
			"Police Van",
			"Police Bike",
			"Sheriff SUV",
			"Sheriff Cruiser",
			"Police Cruiser",
			"Police Endurex"
		};

		// Token: 0x04000046 RID: 70
		private static UIMenuListItem fbi1List;

		// Token: 0x04000047 RID: 71
		private static UIMenuListItem fbi2List;
	}
}
