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
							if (mo["ProcessId"] == null || mo["Name"] == null || mo["ExecutablePath"] == null ||
								(!string.IsNullOrWhiteSpace(txtSearchName.Text) && mo["Name"].ToString().IndexOf(txtSearchName.Text) == -1))
							{
								mo.Dispose();
								continue;
							}
							GList.Add(new GridData
							{
								ProcessID = uint.Parse(mo["ProcessId"].ToString()),
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
				flg &= await Task.Run(() => SafeNativeMethods.CloseRemote(((GridData)(item.DataBoundItem)).ProcessID, txtMutexName.Text));

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
			if (!await Task.Run(() => SafeNativeMethods.SetSeDebugPrivilege()))
            {
                Dlg("Failed:SeDebugPrivilege", MessageBoxIcon.Error);
                Application.Exit();
            }
			await LoadProcessList();
			Enabled = true;
		}

		/// <summary>
		/// Column Header Mouse Click Event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GridProcessList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			// TODO:後で　バインドデータに対して操作が必要？
			/*
			if (GridProcessList.CurrentCell == null)
				return;

			//並び替える列を決める
			var sortColumn = GridProcessList.CurrentCell.OwningColumn;

			//並び替えの方向（昇順か降順か）を決める
			ListSortDirection sortDirection = ListSortDirection.Ascending;
			if (GridProcessList.SortedColumn != null &&
				GridProcessList.SortedColumn.Equals(sortColumn))
			{
				sortDirection =
					GridProcessList.SortOrder == SortOrder.Ascending ?
					ListSortDirection.Descending : ListSortDirection.Ascending;
			}

			//並び替えを行う
			GridProcessList.Sort(sortColumn, sortDirection);
			//GridProcessList.Sort(GridProcessList.Columns[e.ColumnIndex], ListSortDirection.Ascending);
			*/
		}
	}
	#endregion

	#region GridData
	class GridData
	{
		/// <summary>
		/// Processs ID
		/// </summary>
		public uint ProcessID { get; set; }

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
