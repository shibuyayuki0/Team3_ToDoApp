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

        [BindProperty]
        public int SelectedId { get; set; }

        public List<ToDoApp_team3.Model.Tasks> TaskList { get; set; } = [];
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
        //public IActionResult OnPostComplete(int SelectedId)
        //{
        //    try
        //    {
        //
        //    }
        //    catch
        //    {
        //
        //    }var task = _dataEditor.GetTask(SelectedId);

        //    if(task == null)
        //    {
        //        // 指定した別のページへ（index）画面を切り替える
        //        return RedirectToPage("Index");
        //    }

        //    task.CompletedAt = DateTime.Now;

        //    _dataEditor.Update(task);


        //    // RedirectToPage("Index",...)同じフォルダ内にあるIndexページへ画面を移動
        //    // new { filterName = FilterName }移動先に引き継ぎたいデータの指定
        //    // 左側の filterNameはURLに表示されるパラメータ名、右側の FilterName が現在のページで保持している変数の値です。
        //    return RedirectToPage("Index", new { FilterNumber });
        //}
    }
}
