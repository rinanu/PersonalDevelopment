CREATE TABLE users (
    user_id SERIAL NOT NULL PRIMARY KEY,
    username VARCHAR NOT NULL UNIQUE,
    email VARCHAR,
    password_hash VARCHAR NOT NULL
);

CREATE TABLE notes (
    note_id SERIAL NOT NULL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    title VARCHAR,
    note_content TEXT NOT NULL,
FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE habits (
    habit_id SERIAL NOT NULL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    habit_name VARCHAR NOT NULL,
    description TEXT,
    frequency VARCHAR CHECK (frequency IN ('daily', 'weekly', 'monthly')),
    is_active BOOLEAN NOT NULL,
FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE habit_logs (
    habit_id INTEGER NOT NULL,
    log_date DATE NOT NULL,
    status BOOLEAN NOT NULL,  -- true = completed, false = skipped/missed
    note TEXT,                -- optional reflection for the day
    PRIMARY KEY (habit_id, log_date), -- one log per habit per date
FOREIGN KEY (habit_id) REFERENCES habits(habit_id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Enable RLS
ALTER TABLE notes ENABLE ROW LEVEL SECURITY;

-- SELECT: users can only see their own notes
CREATE POLICY notes_select ON notes
FOR SELECT
USING (user_id = current_setting('app.current_user')::INT);

-- INSERT: users can only insert notes with their own user_id
CREATE POLICY notes_insert ON notes
FOR INSERT
WITH CHECK (user_id = current_setting('app.current_user')::INT);

-- UPDATE: users can only update their own notes
CREATE POLICY notes_update ON notes
FOR UPDATE
USING (user_id = current_setting('app.current_user')::INT)
WITH CHECK (user_id = current_setting('app.current_user')::INT);

-- DELETE: users can only delete their own notes
CREATE POLICY notes_delete ON notes
FOR DELETE
USING (user_id = current_setting('app.current_user')::INT);
ALTER TABLE habits ENABLE ROW LEVEL SECURITY;

CREATE POLICY habits_select ON habits
FOR SELECT
USING (user_id = current_setting('app.current_user')::INT);

CREATE POLICY habits_insert ON habits
FOR INSERT
WITH CHECK (user_id = current_setting('app.current_user')::INT);

CREATE POLICY habits_update ON habits
FOR UPDATE
USING (user_id = current_setting('app.current_user')::INT)
WITH CHECK (user_id = current_setting('app.current_user')::INT);

CREATE POLICY habits_delete ON habits
FOR DELETE
USING (user_id = current_setting('app.current_user')::INT);
ALTER TABLE habit_logs ENABLE ROW LEVEL SECURITY;

-- SELECT logs only for habits belonging to the current user
CREATE POLICY logs_select ON habit_logs
FOR SELECT
USING (
  habit_id IN (
    SELECT habit_id FROM habits
    WHERE user_id = current_setting('app.current_user')::INT
  )
);

-- INSERT logs only for user’s habits
CREATE POLICY logs_insert ON habit_logs
FOR INSERT
WITH CHECK (
  habit_id IN (
    SELECT habit_id FROM habits
    WHERE user_id = current_setting('app.current_user')::INT
  )
);

-- UPDATE logs only for user’s habits
CREATE POLICY logs_update ON habit_logs
FOR UPDATE
USING (
  habit_id IN (
    SELECT habit_id FROM habits
    WHERE user_id = current_setting('app.current_user')::INT
  )
)
WITH CHECK (
  habit_id IN (
    SELECT habit_id FROM habits
    WHERE user_id = current_setting('app.current_user')::INT
  )
);

-- DELETE logs only for user’s habits
CREATE POLICY logs_delete ON habit_logs
FOR DELETE
USING (
  habit_id IN (
    SELECT habit_id FROM habits
    WHERE user_id = current_setting('app.current_user')::INT
  )
);
CREATE ROLE guest NOINHERIT;
CREATE ROLE app_user NOINHERIT;
GRANT INSERT ON users TO guest;
GRANT SELECT, INSERT, UPDATE, DELETE ON notes, habits, habit_logs TO app_user;
-- users can also UPDATE their own row in "users" (e.g. change email)
GRANT SELECT, UPDATE ON users TO app_user;