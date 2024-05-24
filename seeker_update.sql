CREATE TABLE IF NOT EXISTS system_state (
	version INT NOT NULL DEFAULT 0
);

INSERT INTO system_state
SELECT 0
where not exists (
    select 1 from system_state
);

CREATE OR REPLACE FUNCTION get_system_db_version() RETURNS int
AS $$
DECLARE DbVersion int;
BEGIN
	SELECT version INTO DbVersion
	FROM system_state;

	RETURN DbVersion;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION set_system_db_version(version INT) RETURNS void AS $$
BEGIN
    UPDATE system_state
    SET version = version;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF (get_system_db_version() < 1) THEN

        ALTER TABLE job_to_request 
        ADD PRIMARY KEY(job_id, request_id);

        ALTER TABLE filter 
        RENAME COLUMN filter_text TO text;

        ALTER TABLE filter 
        RENAME COLUMN filter_type TO type;

        ALTER TABLE filter
        ADD COLUMN subtype int NOT NULL DEFAULT 0;

        ALTER TABLE job
        ADD COLUMN title VARCHAR(255);

        UPDATE job 
        SET title = preview->>'title';

        ALTER TABLE job
        ALTER COLUMN title
        SET NOT NULL;

    END IF;
END;
$$ LANGUAGE plpgsql;

