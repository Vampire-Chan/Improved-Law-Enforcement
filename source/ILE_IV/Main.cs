using System;
using System.Windows.Forms;

using IVSDKDotNet;
using IVSDKDotNet.Enums;
using static IVSDKDotNet.Native.Natives;

namespace ILE_IV
{
    public class Main : Script
    {

        #region Variables
        //private int playerPed;
        #endregion

        #region Constructor
        public Main()
        {
            // Subscribe to script events
            Initialized += Main_Initialized;
            Tick += Main_Tick;
            KeyDown += Main_KeyDown;
        }
        #endregion

        private void Main_Initialized(object sender, EventArgs e)
        {

        }

        // Runs every frame when in-game
        private void Main_Tick(object sender, EventArgs e)
        {
            
        }
        
        //todo
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad7)
            {
               
            }
        }
    }
}
