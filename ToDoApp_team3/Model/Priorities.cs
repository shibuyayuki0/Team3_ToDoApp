namespace ToDoApp_team3.Model;

/// <summary>
/// 優先度を表すデータ
/// </summary>
/// <param name="PriorityId">ID</param>
/// <param name="ProprotyName">名称</param>
public record Priorities
(
    int PriorityId,
    string PriorityName
);
