﻿using K_Relay.Util;
using Lib_K_Relay;
using Lib_K_Relay.GameData;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Utilities;
using MetroFramework.Drawing;
using MetroFramework.Forms;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K_Relay
{
    public partial class FrmMainMetro : MetroForm
    {
        private Proxy _proxy;

        public FrmMainMetro()
        {
            InitializeComponent();
            tabMain.SelectedIndex = 0;
            Console.SetOut(new MetroTextBoxStreamWriter(tbxLog));

            if (StealthConfig.Default.StealthEnabled)
            {
                Shown += (sender, e) => Hide();
            }
        }

        private async void FrmMainMetro_Load(object sender, EventArgs e)
        {
            ReconnectHandler.ChangeDefault += ChangeServer;

            await Task.Run(() =>
            {
                GameData.Load();
				InitAccounts();
                InitPackets();
                InitSettings();
            });

            _proxy = new Proxy();
            _proxy.ProxyListenStarted += _ => SetStatus("Running", Color.Green);
            _proxy.ProxyListenStopped += _ => SetStatus("Stopped", Color.Red);
            _proxy.StealthStateChanged += enabled => Invoke(new Action(() => Visible = !enabled));
            InitPlugins();

            if (GameData.Servers.Map.Where(s => s.Value.Name == (string)lstServers.SelectedItem).Any())
            {
                Proxy.DefaultServer = GameData.Servers.ByName((string)lstServers.SelectedItem).Address;
            }
            else
            {
                PluginUtils.Log("K Relay", "Default server wasn't found, using USWest.");
            }

            PluginUtils.Log("K Relay", "Initialization complete.");

            btnToggleProxy.Enabled = true;
            if (Config.Default.StartProxyByDefault)
            {
                btnToggleProxy_Click(null, null);
            }
        }

        private void m_themeManager_OnStyleChanged(object sender, EventArgs e)
        {
            menuInfo.BackColor = menuPlugins.BackColor = menuAccounts.BackColor = menuPackets.BackColor = menuSettings.BackColor = MetroPaint.GetStyleColor(Style);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnToggleProxy_Click(object sender, EventArgs e)
        {
            if (btnToggleProxy.Text == "Start Proxy")
            {
                btnToggleProxy.Text = "Stop Proxy";
                SetStatus("Starting...", Color.Black);

                _proxy.Start();
            }
            else
            {
                btnToggleProxy.Text = "Start Proxy";
                SetStatus("Stopping...", Color.Black);
                _proxy.Stop();
            }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog()
            {
                DefaultExt = ".txt",
                AddExtension = true,
                FileName = "K_Relay Log",
                SupportMultiDottedExtensions = true,
                Filter = "Text Files (*.txt)|*.txt"
            };

            if (s.ShowDialog() == DialogResult.OK)
            {
                FileStream logFile = File.Open(s.FileName, FileMode.Create);
                using (StreamWriter sw = new StreamWriter(logFile))
                {
                    sw.Write(tbxLog.Text);
                }
                logFile.Close();
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbxLog.Clear();
        }

		private void btnAddAccount_Click(object sender, EventArgs e)
		{
			// TODO add button
		}

		private void btnDelAccount_Click(object sender, EventArgs e)
		{
			// TODO add button
		}

		private void btnSelectAccount_Click(object sender, EventArgs e)
		{
			// TODO add button
		}

		private void btnClearAccount_Click(object sender, EventArgs e)
		{
			// TODO add button
		}

		private void SetStatus(string status, Color color)
        {
            Invoke(new MethodInvoker(() =>
            {
                lblStatus.ForeColor = color;
                lblStatus.Text = status;
            }));
        }
    }
}
