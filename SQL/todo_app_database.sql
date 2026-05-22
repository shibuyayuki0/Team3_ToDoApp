-- DBの有無をチェックし、無ければ生成をする
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'todo_app_database')
BEGIN
	EXEC('CREATE DATABASE todo_app_database'); -- CREATE DbをIF中に書くための所作（文字列を渡して動的生成させる）
END
GO -- 生成チェックまでで一区切り


-- DBへ接続する
USE todo_app_database;
GO -- 接続までで一区切り


-- テーブルを生成する

-- 優先度を表すマスターテーブル
-- テーブルの存在チェックを行って無ければ生成する
-- OBJECT_ID：各オブジェクトの固有IDを取得する、なければNULL
-- UNIQUE：PK以外で重複を禁止する
IF OBJECT_ID('priorities', 'U') IS NULL
BEGIN
	CREATE TABLE priorities	(
		priority_id INT IDENTITY(1,1) PRIMARY KEY,
		priority_name NVARCHAR(1) UNIQUE NOT NULL);
END

-- タスクを表すテーブル
IF OBJECT_ID('tasks', 'U') IS NULL
BEGIN
	-- テーブル本体
	CREATE TABLE tasks(
		id INT IDENTITY(1,1) PRIMARY KEY,
		task_name NVARCHAR(30) NOT NULL,
		content_text NVARCHAR(255) NOT NULL,
		deadline_at DATETIME NOT NULL,
		priority_id INT NOT NULL,
		completed_at DATETIME,
		created_at DATETIME NOT NULL,
		deleted_at DATETIME,
		FOREIGN KEY(priority_id)REFERENCES priorities(priority_id));
		
	-- インデックスの作成
	-- 検索用、deadline_at ASCでソートする際に使用
	-- deleted_at, completed_atはソート時の条件に含まれるので結合インデックスとして処理
	CREATE INDEX IX_tasks_deleted_completed_deadline
	ON tasks (deleted_at, deadline_at ASC, completed_at)
	INCLUDE (task_name, content_text, priority_id);
END


-- マスターデータの定義

-- 優先度
-- SELECT～：p.priority_nameをまとめた仮テーブルを作成する
-- FROM～：priority_nameというキーで高～低という値をもつ
-- WHERE～：データがないよね？という条件
-- SELECT 1 FROM～：既存のpriorities中のpriority_nameと今回のpriority_nameが一致するものの一覧
-- →　まとめて既存のpriority_nameに含まれないなら、追加するという処理
INSERT INTO priorities(priority_name)
SELECT p.priority_name
FROM (
	SELECT N'高' AS [priority_name] UNION ALL
	SELECT N'中' AS [priority_name] UNION ALL
	SELECT N'低' AS [priority_name]
) AS p
WHERE NOT EXISTS(
	SELECT 1 FROM priorities p2 WHERE p2.priority_name = p.priority_name
);


-- テスト用データの作成

-- 仮ToDoデータ
INSERT INTO tasks(
	task_name,
	content_text,
	deadline_at,
	priority_id,
	created_at)
VALUES(
	N'お試しタスク',
	N'お試しお試しお試しお試しお試し',
	'2026-06-01 23:59:59',
	2,
	CURRENT_TIMESTAMP
);