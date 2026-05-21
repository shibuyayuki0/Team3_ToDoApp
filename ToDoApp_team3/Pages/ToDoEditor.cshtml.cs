using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ToDoApp_team3.Model;

namespace ToDoApp_team3.Pages
{
    public class ToDoEditorModel : PageModel
    {
        // ===== DI =====
        // ITaskDataEditorの参照
        private readonly ITaskDataEditor _dataEditor;


        // ===== コンストラクタ =====
        public ToDoEditorModel(ITaskDataEditor dataEditor)
        {
            // DI
            _dataEditor = dataEditor;

            // 優先度リストの取得
            var priorities = _dataEditor.PriorityList;

            // セレクトリストを作る
            List<SelectListItem> options = [];
            foreach(var p in priorities)
            {
                var item = new SelectListItem {
                    Value = p.PriorityId.ToString(),
                    Text = p.PriorityName
                };
                options.Add(item);
            }
            Options = options;
        }


        // ===== 参照用のデータ =====
        // 優先度リスト
        public List<SelectListItem> Options { get; }


        // ===== 入力欄 =====
        // タスク名
        [BindProperty]
        [Display(Name = "タスク名")]
        [Required(ErrorMessage = "タスク名は必須項目です。")]
        [StringLength(30, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string? TaskName { get; set; }

        // 選択された優先度
        [BindProperty]
        [Display(Name = "優先度")]
        public int SelectedPriority { get; set; } = 1;

        // 期限
        [BindProperty]
        [Display(Name = "期限")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "期限は必須項目です。")]
        public DateTime? DeadlineAt { get; set; } = DateTime.Now;

        // 詳細内容
        [BindProperty]
        [Display(Name = "詳細内容")]
        [StringLength(255, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string? ContentText { get; set; }


        // ===== 外観 =====
        // 登録画面の説明
        public string FormTitle { get; private set; } = "タスクの新規作成";

        // 登録ボタンの名称
        public string PostButtonName { get; private set; } = "作成";


        // ===== メソッド =====
        // GET：画面表示
        public IActionResult OnGet(string? id)
        {
            // idが
            //   null：新規作成モード
            //   値あり：編集モード
            // として動作する

            // idがnullでないとき
            if (!string.IsNullOrEmpty(id))
            {
                // IDが不正（文字列や0以下）なら400エラー
                if (!int.TryParse(id, out var parsedId) || parsedId <= 0)
                {
                    return BadRequest("無効なIDです。");
                }

                // タスクを取得
                var targetTask = _dataEditor.GetTask(parsedId);

                // IDが存在しなければ404エラー
                if (targetTask is null)
                {
                    return NotFound("タスクは存在しません。");
                }

                // 各入力欄に初期値を入れる
                TaskName = targetTask.TaskName;
                SelectedPriority = targetTask.PriorityId;
                DeadlineAt = targetTask.DeadlineAt;
                ContentText = targetTask.ContentText;

                // タイトルとボタンの名称を変える
                FormTitle = "タスクの編集";
                PostButtonName = "更新";
            }

            // ページを表示
            return Page();
        }

        // POST：新規作成または更新
        public IActionResult OnPost(int? id)
        {
            // 必須入力欄がないとき
            if (!ModelState.IsValid)
            {
                return Page(); // 再表示→エラーメッセージが表示される
            }

            
            if (!id.HasValue)
            {
                /* idが無ければ新規登録を行う */

                // 新規のタスクを作る
                // TaskId：0は、DB登録時に使用しないので許容する
                var newTask = new Tasks(
                    0,
                    this.TaskName!,
                    this.ContentText ?? "",
                    this.DeadlineAt!.Value,
                    this.SelectedPriority,
                    null
                );

                // DBに登録する
                _dataEditor.Add(newTask);

            }
            else
            {
                /* idが有ればデータ更新を行う */

                // 対象タスク状態を取得する
                var targetTask = _dataEditor.GetTask(id.Value);

                // 更新用データを作成する
                // targetTask!：idの存在はページ表示時点で保証されているので、nullチェックをしない
                var updateTask = targetTask! with
                {
                    TaskName = this.TaskName!,
                    ContentText = this.ContentText ?? "",
                    DeadlineAt = this.DeadlineAt!.Value,
                    PriorityId = this.SelectedPriority,
                };

                // DBに書き込む
                _dataEditor.Update(updateTask);
            }

            // リダイレクト：Indexに戻る
            return RedirectToPage("/Index");
        }
    }
}
