using Dapper;
using Microsoft.Data.SqlClient;

namespace ToDoApp_team3.Model;

public class TaskDataEditor : ITaskDataEditor
{
    // アプリケーション構成プロパティの参照
    private readonly IConfiguration _config;

    // DB接続文字列
    private readonly string? _connectionString;

    public TaskDataEditor(IConfiguration config)
    {
        // DBの接続設定
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
    }


    // ===== クエリ（参照系） =====

    // 優先度リスト　→　非同期で取得するのでメソッド化
    public async Task<List<Priorities>> GetPriorityListAsync()
    {
        var sql = @"
            SELECT
                priority_id AS [PriorityId],
                priority_name AS [PriorityName]
            FROM priorities;
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // Prioritiesのリストを返す
        return [.. await connection.QueryAsync<Priorities>(sql)];
    }

    // タスクリストの取得
    public async Task<List<Tasks>> GetTaskListAsync(ListFilterMode mode)
    {
        // タスクを全行取得する（条件：論理削除されていないもの）
        var sql = @"
            SELECT
                id AS [TaskId],
                task_name AS [TaskName],
                content_text AS [ContentText],
                deadline_at AS [DeadlineAt],
                priority_id AS [PriorityId],
                completed_at AS [CompletedAt]
            FROM tasks
            WHERE deleted_at IS NULL";

        // modeによって追加の条件を付加する（未完了or完了orすべて）
        var sqlConditions = mode switch
        {
            ListFilterMode.Continue => " AND completed_at IS NULL ",
            ListFilterMode.Complete => " AND completed_at IS NOT NULL",
            _ => ""
        };

        // 文字列結合＋idの降順ソート指定
        sql += sqlConditions + " ORDER BY deadline_at ASC;";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // List型に変換して返す
        var taskList = await connection.QueryAsync<Tasks>(sql);
        return [.. taskList];
        // ①スプレッド演算子(..)で、taskListの戻り値をバラバラにする
        // ②コレクション式（[]）で、List型として再構成される
        // ⇒ 今回は戻り値の型がList型なので、[]はList型を返す（型推論）
    }

    // タスク1件の取得、なければNULL
    public async Task<Tasks?> GetTaskAsync(int taskId)
    {
        // 1個のタスクを取得するsql文
        var sql = @"
            SELECT
                id AS [TaskId],
                task_name AS [TaskName],
                content_text AS [ContentText],
                deadline_at AS [DeadlineAt],
                priority_id AS [PriorityId],
                completed_at AS [CompletedAt]
            FROM tasks
            WHERE id = @id AND deleted_at IS NULL;
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // ヒットしたタスクを返す、ヒットしないときはNULLを返す
        return await connection.QueryFirstOrDefaultAsync<Tasks>(sql, new { id = taskId });
    }


    // ===== コマンド（書き込み系） =====

    // タスクの追加
    public async Task AddAsync(Tasks newTask)
    {
        // タスクを追加する
        // created_atにはCURRENT_TIMESTAMP（現在時刻）を入れる
        var sql = @"
            INSERT INTO tasks (
                task_name,
                content_text,
                deadline_at,
                priority_id,
                created_at
            ) VALUES (
                @taskName,
                @contentText,
                @deadlineAt,
                @priorityId,
                CURRENT_TIMESTAMP
            );
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // sql実行、戻り値（影響を受けた件数）は使わない
        _ = await connection.ExecuteAsync(
            sql,
            new
            {
                taskName = newTask.TaskName,
                contentText = newTask.ContentText,
                deadlineAt = newTask.DeadlineAt,
                priorityId = newTask.PriorityId
            });
    }

    // タスク更新
    public async Task UpdateAsync(Tasks targetTask)
    {
        // タスクIDを取得する
        int taskId = targetTask.TaskId;

        // タスクを更新するsql文
        // WHEREで①idが指定したもの、②deleted_atがnullのもの（＝まだ削除されていないタスク）に限定している
        var sql = @"
            UPDATE tasks
            SET
                task_name = @taskName,
                content_text = @contentText,
                deadline_at = @deadlineAt,
                priority_id = @priorityId,
                completed_at = @completedAt
            WHERE id = @id AND deleted_at IS NULL;
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // sqlを実行し、実際に影響を受けた件数を取得する
        var affectedTasks = await connection.ExecuteAsync(
            sql,
            new
            {
                taskName = targetTask.TaskName,
                contentText = targetTask.ContentText,
                deadlineAt = targetTask.DeadlineAt,
                priorityId = targetTask.PriorityId,
                completedAt = targetTask.CompletedAt,
                id = taskId
            });

        // 件数が0のとき（影響を受けたタスクがない）は、taskIDがおかしいと表示
        if (affectedTasks == 0)
        {
            throw new KeyNotFoundException($"ID：{taskId}というタスクは存在しません。");
        }
    }

    // タスク削除
    public async Task DeleteAsync(int taskId)
    {
        // tasksテーブルのdeleted_atにタイムスタンプを入れる
        // WHEREで①idが指定したもの、②deleted_atがnullのもの（＝まだ削除されていないタスク）に限定している
        var sql = @"
            UPDATE tasks
            SET deleted_at = CURRENT_TIMESTAMP
            WHERE id = @id AND deleted_at IS NULL;
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // sqlを実行し、実際に影響を受けた件数を取得する
        var affectedTasks = await connection.ExecuteAsync(sql, new { id = taskId });

        // 件数が0のとき（影響を受けたタスクがない）は、taskIDがおかしいと表示
        if (affectedTasks == 0)
        {
            throw new KeyNotFoundException($"ID：{taskId}というタスクは存在しません。");
        }
    }
}
