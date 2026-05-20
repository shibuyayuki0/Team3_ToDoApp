namespace ToDoApp_team3.Model;

/// <summary>
/// タスクを表すデータ
/// </summary>
/// <param name="TaskId">ID</param>
/// <param name="TaskName">タスク名</param>
/// <param name="ContentText">詳細内容</param>
/// <param name="DeadlineAt">締切日</param>
/// <param name="PriorityId">優先度を表すID</param>
/// <param name="CompletedAt">完了日</param>
public record Tasks
(
    int TaskId,
    string TaskName,
    string ContentText,
    DateTime DeadlineAt,
    int PriorityId,
    DateTime? CompletedAt // NULL許容
);
