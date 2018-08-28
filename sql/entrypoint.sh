#!/bin/bash

echo "Allowing $INIT_DB_DELAY seconds for SQL Server to bootstrap, then initializing database..."
# Use `until` to block the script on the running sqlservr process in the foreground,
# and execute the sleep and (`&&`) initialization in the background (`&`).
until
    sleep $INIT_DB_DELAY &&
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -l 60 -d master -i init-db.sql &
    /opt/mssql/bin/sqlservr;
    do
    echo ""; #noop
done