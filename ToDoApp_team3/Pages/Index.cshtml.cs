using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoApp_team3.Model;

namespace ToDoApp_team3.Pages
{


    public class IndexModel : PageModel
    {
        // 外部から渡されたオブジェクトを、クラス内で使い回すためのプライベート変数
        private readonly ITaskDataEditor _dataEditor;

        public IndexModel(ITaskDataEditor dataEditor)
        {
            _dataEditor = dataEditor;

            // 優先度リスト
            PriorityList = _dataEditor.PriorityList;
        }

        // タスクの一覧
        public List<ToDoApp_team3.Model.Tasks> TaskList { get; set; } = [];

        // 優先度の一覧
        public List<Priorities> PriorityList { get; set; }= [];

        // フィルターリスト
        public List<SelectListItem> FilterList { get; } = [
            new SelectListItem{ Value = ListFilterMode.Continue.ToString(), Text = "未完了" },
            new SelectListItem{ Value = ListFilterMode.Complete.ToString(), Text = "完了" },
            new SelectListItem{ Value = ListFilterMode.All.ToString(), Text = "すべて" }
        ];

        // フィルターの選択肢（初期値は未完了）
        [BindProperty(SupportsGet = true)]
        public ListFilterMode SelectedFilter { get; set; } = ListFilterMode.Continue;

        // === GET ===
        public void OnGet()
        {
            TaskList = _dataEditor.GetTaskList(SelectedFilter);
        }

        // === POST：削除 ===
        public IActionResult OnPostDelete(int? id)
        {
            // id存在確認
            if (!id.HasValue || _dataEditor.GetTask(id.Value) is null)
            {
                return BadRequest("無効なIDが渡されました。");
            }

            // 削除処理
            _dataEditor.Delete(id.Value);

            // インデックスに戻る
            return RedirectToPage();
        }

        // === POST：完了 ===
        public IActionResult OnPostComplete(int id)
        {
            var task = _dataEditor.GetTask(id);
            
            if (task is null)
            {
                return BadRequest("不正な操作を検出しました。");
            }

            // 完了日のトグル処理（未完了なら完了へ、完了なら未完了へ）
            DateTime? newDate = task.CompletedAt is null ? DateTime.Now : null;

            // 完了日を更新
            var newTask = task with { CompletedAt = newDate };
            _dataEditor.Update(newTask);

            // RedirectToPage("/Index",...)同じフォルダ内にあるIndexページへ画面を移動
            // 左側の filterNameはURLに表示されるパラメータ名、右側の FilterName が現在のページで保持している変数の値です。
            return RedirectToPage("/Index", new { this.SelectedFilter });
        }
    }
}
