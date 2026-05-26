using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ToDoApp_team3.Model;

namespace ToDoApp_team3.Pages;

public class IndexModel(ITaskDataEditor dataEditor) : PageModel
{
    // ===== DI =====

    // 外部から渡されたオブジェクトを、クラス内で使い回すためのプライベート変数
    private readonly ITaskDataEditor _dataEditor = dataEditor;


    // ===== プロパティ =====

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


    // ===== メソッド =====

    // GET
    public IActionResult OnGet()
    {
        // --- DB接続 ---
        try
        {
            // 優先度リストの取得
            PriorityList = _dataEditor.GetPriorityList();
            // タスクリストの取得
            TaskList = _dataEditor.GetTaskList(SelectedFilter);
        }
        catch (SqlException)
        {
            return StatusCode(500, "データベース接続に失敗しました。管理者に問い合わせてください。");
        }

        // 画面を表示する
        return Page();
    }

    // POST：削除
    public IActionResult OnPostDelete(int? id)
    {
        // id存在確認
        if (!id.HasValue)
        {
            return BadRequest("無効なIDが渡されました。");
        }

        // --- DB接続 ---
        try
        {
            // 削除処理
            _dataEditor.Delete(id.Value);
        }
        catch (SqlException)
        {
            return StatusCode(500, "データベース接続に失敗しました。管理者に問い合わせてください。");
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest($"{ex.Message}"); // タスクIDxxは存在しない、または既に削除されています。
        }

        // インデックスに戻る
        return RedirectToPage();
    }

    // POST：完了
    public IActionResult OnPostComplete(int id)
    {
        // --- DB接続 ---
        Tasks? targetTask;
        try
        {
            // タスクを取得する
            targetTask = _dataEditor.GetTask(id);
        }
        catch (SqlException)
        {
            return StatusCode(500, "データベース接続に失敗しました。管理者に問い合わせてください。");
        }
        
        // idが存在しない場合
        if (targetTask is null)
        {
            return BadRequest("不正な操作を検出しました。");
        }

        // 完了日のトグル処理（未完了なら完了へ、完了なら未完了へ）
        DateTime? newDate = targetTask.CompletedAt is null ? DateTime.Now : null;

        // 完了日を更新
        var newTask = targetTask with { CompletedAt = newDate };

        // --- DB接続 ---
        try
        {
            // タスクを更新する
            _dataEditor.Update(newTask);
        }
        catch (SqlException)
        {
            return StatusCode(500, "データベース接続に失敗しました。管理者に問い合わせてください。");
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest($"{ex.Message}"); // タスクIDxxは存在しない、または既に削除されています。
        }

        // RedirectToPage("/Index",...)同じフォルダ内にあるIndexページへ画面を移動
        // 左側の filterNameはURLに表示されるパラメータ名、右側の FilterName が現在のページで保持している変数の値です。
        return RedirectToPage("/Index", new { this.SelectedFilter });
    }
}
