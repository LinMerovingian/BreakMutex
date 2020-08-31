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
                var GList = new SortBindingList<GridData>();
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
            lblStatus.Text = string.Empty;
            Enabled = false;
			lblStatus.Text = $"Reload {(await LoadProcessList() ? "Success." : "Failed.")}";
            Enabled = true;
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
			if (GridProcessList.SelectedRows.Count == 0)
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


    // ほぼ流用・・・
    /// <summary>
    /// ソート可能なバインディングリストクラス
    /// </summary>
    /// <typeparam name="T">リスト内容</typeparam>
    public class SortBindingList<T> : BindingList<T>
        where T : class
    {
        /// <summary>
        /// ソート済みか
        /// </summary>
        private bool isSorted;

        /// <summary>
        /// 並べ替え操作の方向
        /// </summary>
        private ListSortDirection sortDirection = ListSortDirection.Ascending;

        /// <summary>
        /// ソートを行う抽象化プロパティ
        /// </summary>
        private PropertyDescriptor sortProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        public SortBindingList()
        {
        }

        /// <summary>
        /// Constructor(IList)
        /// </summary>
        /// <param name="list">SortableBindingList に格納される System.Collection.Generic.IList</param>
        public SortBindingList(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// ソートサポートしているか
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        /// リストがソートされたかどうかを示す値を取得します。
        /// </summary>
        protected override bool IsSortedCore
        {
            get { return isSorted; }
        }

        /// <summary>
        /// ソートされたリストの並べ替え操作の方向を取得します
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirection; }
        }

        /// <summary>
        /// ソートに利用する抽象化プロパティ取得
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortProperty; }
        }

        /// <summary>
        /// ApplySortCore で適用されたソート情報削除
        /// </summary>
        protected override void RemoveSortCore()
        {
            sortDirection = ListSortDirection.Ascending;
            sortProperty = null;
            isSorted = false;
        }

        /// <summary>
        /// 指定されたプロパティおよび方向でソートを行います。
        /// </summary>
        /// <param name="prop">抽象化プロパティ</param>
        /// <param name="direction">並べ替え操作の方向</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            // ソート情報記録
            sortProperty = prop;
            sortDirection = direction;

            // ソートリスト取得
            var list = Items as List<T>;
            if (list == null)
                return;

            // ソート処理
            list.Sort(Compare);
            isSorted = true;

            // ListChangedEvent
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// 比較処理
        /// </summary>
        /// <param name="lhs">左側の値</param>
        /// <param name="rhs">右側の値</param>
        /// <returns>比較結果</returns>
        private int Compare(T lhs, T rhs)
        {
            // 比較を行う
            var result = OnComparison(lhs, rhs);

            // 昇順の場合 そのまま、降順の場合 反転させる
            return sortDirection == ListSortDirection.Ascending ? result : -result;
        }

        /// <summary>
        /// 昇順比較処理
        /// </summary>
        /// <param name="lhs">左側の値</param>
        /// <param name="rhs">右側の値</param>
        /// <returns>比較結果</returns>
        private int OnComparison(T lhs, T rhs)
        {
            var lhsValue = (lhs == null) ? null : sortProperty.GetValue(lhs);
            var rhsValue = (rhs == null) ? null : sortProperty.GetValue(rhs);

            if (lhsValue == null)
                return rhsValue == null ? 0 : -1;

            if (rhsValue == null)
                return 1;

            if (lhsValue is IComparable)
                return ((IComparable)lhsValue).CompareTo(rhsValue);

            if (lhsValue.Equals(rhsValue))
                return 0;

            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }
    }
    #endregion
}
