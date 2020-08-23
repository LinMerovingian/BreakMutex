using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BreakMutexApp
{
	#region BreakMutexMain
	public partial class BreakMutexMain : Form
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BreakMutexMain()
		{
			InitializeComponent();
		}

		private void ExceptionDlg(Exception ex, string msg = "")
		{
			MessageBox.Show($"Exception!!{(string.IsNullOrEmpty(msg) ? string.Empty : $"\n{msg}")}\n[{ex.GetType().ToString()}]\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void InfoDlg(string msg)
		{
			MessageBox.Show(msg, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ErrorDlg(string msg)
		{
			MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// プロセス読込
		/// </summary>
		/// <returns></returns>
		private async Task<bool> LoadProcessList()
		{
			try
			{
				var GList = new ArrayList();
				await Task.Run(() =>
				{
					using (var mc = new ManagementClass("Win32_Process"))
					using (var moc = mc.GetInstances())
					{
						foreach (var mo in moc)
						{
							GList.Add(new GridData
							{
								ProcessID = int.Parse(mo["ProcessId"].ToString()),
								ProcessName = mo["Name"]?.ToString(),
								ProcessPath = mo["ExecutablePath"]?.ToString(),
							});
							mo.Dispose();
						}
					}
				});
				GridProcessList.DataSource = GList;
				return true;
			}
			catch (Exception ex)
			{
				ExceptionDlg(ex);
				return false;
			}
		}

		/// <summary>
		/// Reload Click Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BtnReload_Click(object sender, EventArgs e)
		{
			if (await LoadProcessList())
				InfoDlg("Success.");
		}

		/// <summary>
		/// Break Click Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BtnBreak_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtMutexName.Text))
			{
				ErrorDlg("Non mutex name.");
				return;
			}
			if (GridProcessList.SelectedRows == null)
			{
				ErrorDlg("Not selected process ID.");
				return;
			}
			var flg = true;
			foreach (DataGridViewRow item in GridProcessList.SelectedRows)
				flg &= SafeNativeMethods.CloseRemote((uint)((GridData)(item.DataBoundItem)).ProcessID, txtMutexName.Text);
			if (flg)
				InfoDlg("Success.");
			else
				ErrorDlg("Failed.");
		}

		/// <summary>
		/// Load Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BreakMutexMain_Load(object sender, EventArgs e)
		{
			await LoadProcessList();
		}
	}
	#endregion

	#region GridData
	class GridData
	{
		/// <summary>
		/// Processs ID
		/// </summary>
		public int ProcessID { get; set; }

		/// <summary>
		/// プロセス名
		/// </summary>
		public string ProcessName { get; set; }

		/// <summary>
		/// プロセスパス
		/// </summary>
		public string ProcessPath { get; set; }
	}
	#endregion
}
