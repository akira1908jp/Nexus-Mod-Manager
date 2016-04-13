using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Nexus.Client.Commands;
using Nexus.Client.Util;

namespace Nexus.Client.Games.Witcher3
{
    /// <summary>
	/// Launches Witcher3.
    /// </summary>
    public class Witcher3Launcher : GameLauncherBase
    {
        #region Constructors

        /// <summary>
        /// A simple constructor that initializes the object with the given dependencies.
        /// </summary>
        /// <param name="p_gmdGameMode">>The game mode currently being managed.</param>
        /// <param name="p_eifEnvironmentInfo">The application's envrionment info.</param>
        public Witcher3Launcher(IGameMode p_gmdGameMode, IEnvironmentInfo p_eifEnvironmentInfo)
            : base(p_gmdGameMode, p_eifEnvironmentInfo)
        {
        }

        #endregion

        /// <summary>
        /// Initializes the game launch commands.
        /// </summary>
        protected override void SetupCommands()
        {
            Trace.TraceInformation("Launch Commands:");
            Trace.Indent();

            ClearLaunchCommands();

            string strCommand = GetPlainLaunchCommand();
            Trace.TraceInformation("Plain Command: {0} (IsNull={1})", strCommand, (strCommand == null));
            Image imgIcon = File.Exists(strCommand) ? Icon.ExtractAssociatedIcon(strCommand).ToBitmap() : null;
			AddLaunchCommand(new Command("PlainLaunch", "Launch Witcher3", "Launches default Witcher3.", imgIcon, LaunchWitcher3Plain, true));

            strCommand = GetCustomLaunchCommand();
            Trace.TraceInformation("Custom Command: {0} (IsNull={1})", strCommand, (strCommand == null));
            imgIcon = File.Exists(strCommand) ? Icon.ExtractAssociatedIcon(strCommand).ToBitmap() : null;
            AddLaunchCommand(new Command("CustomLaunch", "Launch Custom Witcher3", "Launches Witcher3 with custom command.", imgIcon, LaunchWitcher3Custom, true));

			DefaultLaunchCommand = new Command("Launch Witcher3", "Launches Witcher3.", LaunchGame);

            Trace.Unindent();
        }

        #region Launch Commands

        #region Custom Command

        /// <summary>
        /// Launches the game with a custom command.
        /// </summary>
        private void LaunchWitcher3Custom()
        {
			Trace.TraceInformation("Launching Witcher3 (Custom)...");
            Trace.Indent();

            string strCommand = GetCustomLaunchCommand();
            string strCommandArgs = EnvironmentInfo.Settings.CustomLaunchCommandArguments[GameMode.ModeId];
            if (String.IsNullOrEmpty(strCommand))
            {
                Trace.TraceError("No custom launch command has been set.");
                Trace.Unindent();
                OnGameLaunched(false, "No custom launch command has been set.");
                return;
            }
            Launch(strCommand, strCommandArgs);
        }

        /// <summary>
        /// Gets the custom launch command.
        /// </summary>
        /// <returns>The custom launch command.</returns>
        private string GetCustomLaunchCommand()
        {
            string strCommand = EnvironmentInfo.Settings.CustomLaunchCommands[GameMode.ModeId];
            if (!String.IsNullOrEmpty(strCommand))
            {
                strCommand = Environment.ExpandEnvironmentVariables(strCommand);
                strCommand = FileUtil.StripInvalidPathChars(strCommand);
                if (!Path.IsPathRooted(strCommand))
                    strCommand = Path.Combine(GameMode.GameModeEnvironmentInfo.ExecutablePath, strCommand);
            }
            return strCommand;
        }

        #endregion

        #region Vanilla Launch

        /// <summary>
        /// Launches the game in DX mode.
        /// </summary>
        private void LaunchWitcher3Plain()
        {
			Trace.TraceInformation("Launching Witcher3 (Default)...");
            Trace.Indent();
            string strCommand = GetPlainLaunchCommand();
            Trace.TraceInformation("Command: " + strCommand);
            Launch(strCommand, null);
        }

        /// <summary>
        /// Gets the default launch command.
        /// </summary>
        /// <returns>The default launch command.</returns>
        private string GetPlainLaunchCommand()
        {
			string strCommand = Path.Combine(Path.Combine(Path.Combine(GameMode.ExecutablePath, "bin"), "x64"), "Witcher3.exe");
            return strCommand;
        }

        #endregion

        /// <summary>
        /// Launches the game
        /// </summary>
        private void LaunchGame()
        {
            if (!String.IsNullOrEmpty(EnvironmentInfo.Settings.CustomLaunchCommands[GameMode.ModeId]))
				LaunchWitcher3Custom();
            else
                LaunchWitcher3Plain();
        }

        #endregion
    }
}
