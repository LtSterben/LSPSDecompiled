using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Protection_Squad
{
	// Token: 0x02000004 RID: 4
	internal class IniFile
	{
		// Token: 0x0600001A RID: 26
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

		// Token: 0x0600001B RID: 27
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

		// Token: 0x0600001C RID: 28 RVA: 0x000053B0 File Offset: 0x000035B0
		public IniFile(string IniPath = null)
		{
			this.Path = new FileInfo(IniPath ?? (this.EXE + ".ini")).FullName.ToString();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00005404 File Offset: 0x00003604
		public string Read(string Key, string Section = null)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			IniFile.GetPrivateProfileString(Section ?? this.EXE, Key, "", stringBuilder, 255, this.Path);
			return stringBuilder.ToString();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00005445 File Offset: 0x00003645
		public void Write(string Key, string Value, string Section = null)
		{
			IniFile.WritePrivateProfileString(Section ?? this.EXE, Key, Value, this.Path);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00005460 File Offset: 0x00003660
		public void DeleteKey(string Key, string Section = null)
		{
			this.Write(Key, null, Section ?? this.EXE);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00005475 File Offset: 0x00003675
		public void DeleteSection(string Section = null)
		{
			this.Write(null, null, Section ?? this.EXE);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000548A File Offset: 0x0000368A
		public bool KeyExists(string Key, string Section = null)
		{
			return this.Read(Key, Section).Length > 0;
		}

		// Token: 0x04000023 RID: 35
		private string Path;

		// Token: 0x04000024 RID: 36
		private string EXE = Assembly.GetExecutingAssembly().GetName().Name;
	}
}
