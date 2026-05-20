using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToDoApp_team3.Model;

namespace ToDoApp_team3.Pages
{
    public class ToDoEditorModel : PageModel
    {
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? TargetTaskId { get; set; }

        public Tasks? TargetTask { get; set; }

        public IActionResult OnGet()
        {
            // IDが無ければ新規作成モードとして起動
            if (!TargetTaskId.HasValue)
            {
                return Page();
            }

            // IDが不正（0以下）なら400エラー
            if(TargetTaskId.Value <= 0)
            {
                return BadRequest("無効なIDです。");
            }

            // タスクを取得
            //TargetTask = _xxxx

            // IDが存在しなければ404エラー
            if (TargetTask is null)
            {
                return NotFound("そのようなタスクは存在しません。");
            }

            // 編集モードとして起動
            return Page();
        }
    }
}
