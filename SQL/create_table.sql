-- 優先度を表すマスターテーブル
create table priorities
(
	priority_id int IDENTITY(1,1) PRIMARY KEY,
	priority_name nvarchar(1) NOT NULL
)
-- タスクを表すテーブル
CREATE TABLE tasks
(
	id int IDENTITY(1,1) PRIMARY KEY,
	task_name nvarchar(30) NOT NULL,
	content_text nvarchar(255) NOT NULL,
	deadline_at datetime NOT NULL,
	priority_id int NOT NULL,
	completed_at datetime,
	created_at datetime NOT NULL,
	deleted_at datetime,
	Foreign Key(priority_id)REFERENCES priorities(priority_id)
)

-- マスターデータの定義
INSERT INTO priorities(priority_name)
VALUES (N'高'),(N'中'),(N'低');

-- テスト用データ
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