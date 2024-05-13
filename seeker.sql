CREATE TABLE job (
    id integer NOT NULL PRIMARY KEY,
    preview jsonb NOT NULL,
    posted_date timestamp without time zone DEFAULT '1994-11-26 00:00:00'::timestamp without time zone NOT NULL,
    content text NOT NULL,
    filter integer DEFAULT 0 NOT NULL
);

CREATE TABLE request (
    id SERIAL PRIMARY KEY,
    text character varying(255) NOT NULL,
    last_update_date timestamp without time zone
);

CREATE TABLE job_to_request (
    job_id integer NOT NULL REFERENCES job,
    request_id integer REFERENCES request
);


CREATE TABLE filter (
    id SERIAL PRIMARY KEY,
    filter_text character varying(250) NOT NULL,
    filter_type integer NOT NULL
);