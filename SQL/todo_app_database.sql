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
IF OBJECT_ID('priorities', 'U') IS NULL
BEGIN
	CREATE TABLE priorities	(
		priority_id INT IDENTITY(1,1) PRIMARY KEY,
		priority_name NVARCHAR(1) NOT NULL);
END

-- タスクを表すテーブル
IF OBJECT_ID('tasks', 'U') IS NULL
BEGIN
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
END


-- マスターデータの定義

-- 優先度
INSERT INTO priorities(priority_name)
VALUES (N'高'),(N'中'),(N'低');


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