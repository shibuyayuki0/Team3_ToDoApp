using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        }



        [BindProperty(SupportsGet = true)]
        public string FilterName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedId { get; set; }

        public List<ToDoApp_team3.Model.Tasks> TaskList { get; set; } = new ();
        public List<Priorities> PriorityList { get; set; }= new ();

        public List<(int id, string name)> ItemList { get; set; } = new();

        public DateTime DeadlineAt => DateTime.Now;

        public void OnGet()
        {
            FilterName ??= "Continue";

            TaskList = _dataEditor.GetTaskList(ListFilterMode.Continue);

            PriorityList = _dataEditor.PriorityList;
        }

        public void OnGetFiltering(string filterName)
        {
            // 現在のフィルター状態を保持
            FilterName = filterName;

            // フィルターに応じて一覧取得
            TaskList = filterName switch
            {
                "Continue" => _dataEditor.GetTaskList(ListFilterMode.Continue),
                "Complete" => _dataEditor.GetTaskList(ListFilterMode.Complete),
                "All" => _dataEditor.GetTaskList(ListFilterMode.All)
            };

            // 優先度一覧も再取得
            PriorityList = _dataEditor.PriorityList;
        }

        public IActionResult OnPostDelete(int selectedId)
        {
            // 対象タスク取得
            var task = _dataEditor.GetTask(selectedId);

            // タスクが存在しない場合
            if (task == null)
            {
                return RedirectToPage("Index");
            }

            // 論理削除
            task.DeletedAt = DateTime.Now;

            // 更新
            _dataEditor.Update(task);

            // フィルター状態を維持して戻る
            return RedirectToPage("Index", new { filterName = FilterName });
        }

        public IActionResult OnPostComplete(int SelectedId)
        {
            var task = _dataEditor.GetTask(SelectedId);

            if(task == null)
            {
                // 指定した別のページへ（index）画面を切り替える
                return RedirectToPage("Index");
            }

            task.CompletedAt = DateTime.Now;

            _dataEditor.Update(task);


            // RedirectToPage("Index",...)同じフォルダ内にあるIndexページへ画面を移動
            // new { filterName = FilterName }移動先に引き継ぎたいデータの指定
            // 左側の filterNameはURLに表示されるパラメータ名、右側の FilterName が現在のページで保持している変数の値です。
            return RedirectToPage("Index", new { filterName = FilterName });
        }
    }
}
