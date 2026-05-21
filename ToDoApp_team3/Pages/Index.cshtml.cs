using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata.Ecma335;
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



        //[BindProperty]
        //public string FilterName { get; set; }

        [BindProperty]
        public int SelectedId { get; set; }

        public List<ToDoApp_team3.Model.Tasks> TaskList { get; set; } = [];
        public List<Priorities> PriorityList { get; set; }= [];

        public record Filters(ListFilterMode ListFilter,string FilterName);


        public List<Filters> FilterList { get; } = [
            new Filters(ListFilterMode.Continue, "未完了"),
            new Filters(ListFilterMode.Complete, "完了"),
            new Filters(ListFilterMode.All, "すべて")];

        [BindProperty]
        public int FilterNumber { get; set; } = 0;
        //public List<(int id, string name)> ItemList { get; set; } = [];

        //public DateTime DeadlineAt => DateTime.Now;

        public void OnGet(int filterNumber)
        {
            //for (int i = 1; i <= 10; i++)
            //{
            //    ItemList.Add((i, $"ID：{i}のタスク"));
            //}

            // フィルターの設定
            ListFilterMode filterMode = FilterList[filterNumber].ListFilter;
            // タスクリストの取得
            TaskList = _dataEditor.GetTaskList(filterMode);

            //
            PriorityList = [.._dataEditor.PriorityList];

        }

        public IActionResult OnGetDetail(int id)
        {
                 _dataEditor.GetTaskList(ListFilterMode.All)
                .Where(x => x.TaskId == id)
                .Select(x => new
                {
                    taskName = x.TaskName,
                    deadlineAt = x.DeadlineAt,
                    contentText = x.ContentText
                })
                .FirstOrDefault();

            return new JsonResult(TaskList);
        }




        //public void OnGetFiltering(string filterName)
        //{
        //    // 現在のフィルター状態を保持
        //    FilterName = filterName;

        //    // フィルターに応じて一覧取得
        //    //TaskList = filterName switch
        //    //{
        //    //    "Continue" => _dataEditor.GetTaskList(ListFilterMode.Continue),
        //    //    "Complete" => _dataEditor.GetTaskList(ListFilterMode.Complete),
        //    //    "All" => _dataEditor.GetTaskList(ListFilterMode.All)
        //    //};

        //    // 優先度一覧も再取得
        //    //PriorityList = _dataEditor.PriorityList;
        //}

   

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
