namespace ToDoApp_team3.Model;

/// <summary>
/// DBからデータ読みだしたり、登録・更新を行う
/// </summary>
public interface ITaskDataEditor
{
    // ===== クエリ（参照系） =====

    /// <summary>
    /// 優先度リストの取得
    /// </summary>
    /// <returns>優先度リスト</returns>
    Task<List<Priorities>> GetPriorityListAsync();

    /// <summary>
    /// タスクの一覧を取得する
    /// </summary>
    /// <param name="mode">取得するモード（未完了・完了・すべて）</param>
    /// <returns>タスクの一覧リスト</returns>
    Task<List<Tasks>> GetTaskListAsync(ListFilterMode mode);

    /// <summary>
    /// 指定したタスクを取得する
    /// </summary>
    /// <param name="taskId">取得するタスクのID</param>
    /// <returns>タスク1件のデータ、タスクIDが存在しない場合はNULL</returns>
    Task<Tasks?> GetTaskAsync(int taskId);


    // ===== コマンド（書き込み系） =====

    /// <summary>
    /// タスクを1件追加する
    /// </summary>
    /// <param name="newTask">追加したいタスク</param>
    Task AddAsync(Tasks newTask);

    /// <summary>
    /// タスクの内容を更新する
    /// </summary>
    /// <param name="targetTask">更新タスク</param>
    /// <exception cref="KeyNotFoundException">
    /// 存在しないタスクIDを含む更新タスクが渡されるとスローされる
    /// </exception>
    Task UpdateAsync(Tasks targetTask);

    /// <summary>
    /// タスクを削除する
    /// </summary>
    /// <param name="taskId">削除するタスクのID</param>
    /// <exception cref="KeyNotFoundException">
    /// 存在しないタスクIDを指定するとスローされる
    /// </exception>
    Task DeleteAsync(int taskId);
}
