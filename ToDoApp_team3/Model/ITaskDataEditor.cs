namespace ToDoApp_team3.Model;

/// <summary>
/// DBからデータ読みだしたり、登録・更新を行う
/// </summary>
public interface ITaskDataEditor
{
    /// <summary>
    /// 優先度を表すリスト
    /// </summary>
    List<Priorities> PriorityList { get; }

    /// <summary>
    /// タスクを1件追加する
    /// </summary>
    /// <param name="task">追加したいタスク</param>
    void Add(Tasks task);

    /// <summary>
    /// タスクの一覧を取得する
    /// </summary>
    /// <param name="mode">取得するモード（未完了・完了・すべて）</param>
    /// <returns>タスクの一覧リスト</returns>
    List<Tasks> GetTaskList(ListFilterMode mode);

    /// <summary>
    /// 指定したタスクを取得する
    /// </summary>
    /// <param name="taskId">取得するタスクのID</param>
    /// <returns>タスク1件のデータ、タスクIDが存在しない場合はNULL</returns>
    Tasks? GetTask(int taskId);

    /// <summary>
    /// タスクの内容を更新する
    /// </summary>
    /// <param name="targetTask">更新タスク</param>
    /// <exception cref="InvalidOperationException">
    /// 存在しないタスクIDを含む更新タスクが渡されるとスローされる
    /// </exception>
    void Update(Tasks targetTask);

    /// <summary>
    /// タスクを削除する
    /// </summary>
    /// <param name="taskId">削除するタスクのID</param>
    /// <exception cref="InvalidOperationException">
    /// 存在しないタスクIDを指定するとスローされる
    /// </exception>
    void Delete(int taskId);
}
