using System;

namespace GoldFish
{
	class ProgParams
	{
		public bool updateRepo;
		public int buildType;
		public string workingDir { get; private set; }

		public static bool LoadFromCmdLine(ref ProgParams progParams, string[] args)
		{
			/*Atlieka, kas būtina*/
			return true;
		}
	}

	class Program
	{
		static void Main(string[] args)
		{			
			ProgParams progParams = new ProgParams();
			//surenkami darbo parametrai iš komandinės eilutės
			if (!ProgParams.LoadFromCmdLine(ref progParams, args))
			{
				Environment.ExitCode = 1;
				return;
			}

			if (progParams.buildType == 1)
			{
				Console.WriteLine("Naudojamas senas versijos numeris");
				return;
			}
			else
			{				
				new Agent(progParams).Work();
			}
		}
	}

	class Agent
	{
		//darbo parametrai
		ProgParams progParams;

		//konstruktorius
		internal Agent(ProgParams progParams)
		{
			this.progParams = progParams;
		}

		string GetPrevVersion()
		{
			if (progParams.updateRepo) CheckOutFilesInRepo();
			return new VerInfo.GetCurrent(progParams.workingDir);
		}

		internal void Work()
		{
			string version = GetPrevVersion();			
			Update(version, progParams.buildType, progParams.workingDir, progParams.updateRepo);
			UpdateNativeRes(version, progParams.buildType, progParams.workingDir, progParams.updateRepo);
		}

		private void UpdateNativeRes(string version, int buildType, string workingDir, bool updateRepo)
		{
			string newVersion = GetNewVersion(version, buildType);
			string file = System.IO.Path.Combine(workingDir, "unmanagedRes", "version.h");

			try
			{
				UnmanagedFileUpdater(file).Update(newVersion);
			}
			catch (Exception)
			{
				Console.WriteLine("Neatnaujintas versijos failas");
				Environment.ExitCode = 1;
			}
		}

		private void Update(string version, int buildType, string workingDir, bool updateRepo)
		{
			string newVersion = GetNewVersion(version, progParams.buildType);
			string file = System.IO.Path.Combine(workingDir, "managedRes", "GlobalAssemblyInfo.cs");

			try
			{
				ManagedFileUpdater().Update(file, newVersion);
			}
			catch (Exception)
			{
				Console.WriteLine("Neatnaujintas versijos failas");
				Environment.ExitCode = 1;
			}
		}

		private string GetNewVersion(string version, int buildType)
		{
			return NewVerFactory.GetNewVer(version, buildType);
		}
	}
}
