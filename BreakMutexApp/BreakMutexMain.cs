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
			lblStatus.Text = string.Empty;
		}

		/// <summary>
		/// 例外ダイアログ
		/// </summary>
		/// <param name="ex">例外</param>
		/// <param name="msg">メッセージ</param>
		private void ExceptionDlg(Exception ex, string msg = "")
		{
			MessageBox.Show($"Exception!!{(string.IsNullOrEmpty(msg) ? string.Empty : $"\n{msg}")}\n[{ex.GetType().ToString()}]\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// ダイアログ
		/// </summary>
		/// <param name="msg">メッセージ</param>
		/// <param name="icon">アイコン</param>
		private void Dlg(string msg, MessageBoxIcon icon)
		{
			MessageBox.Show(msg, "Message", MessageBoxButtons.OK, icon);
		}

		/// <summary>
		/// プロセス一覧読込
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
							if (!string.IsNullOrWhiteSpace(txtSearchName.Text) && mo["Name"] != null && mo["Name"].ToString().IndexOf(txtSearchName.Text) == -1)
							{
								mo.Dispose();
								continue;
							}
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
				GridProcessList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
			lblStatus.Text = $"Reload {(await LoadProcessList() ? "Success." : "Failed.")}";
		}

		/// <summary>
		/// Break Click Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BtnBreak_Click(object sender, EventArgs e)
		{
			Enabled = false;
			var flg = true;
			if (string.IsNullOrWhiteSpace(txtMutexName.Text))
			{
				Dlg("Non mutex name.", MessageBoxIcon.Error);
				flg = false;
			}
			if (GridProcessList.SelectedRows == null)
			{
				Dlg("Not selected process ID.", MessageBoxIcon.Error);
				flg = false;
			}
			
			if (flg)
			foreach (DataGridViewRow item in GridProcessList.SelectedRows)
				flg &= await Task.Run(() => SafeNativeMethods.CloseRemote((uint)((GridData)(item.DataBoundItem)).ProcessID, txtMutexName.Text));

			lblStatus.Text = flg ? "Success." : "Failed.";
			Enabled = true;
		}

		/// <summary>
		/// Load Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void BreakMutexMain_Load(object sender, EventArgs e)
		{
			Enabled = false;
			await LoadProcessList();
			Enabled = true;
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
