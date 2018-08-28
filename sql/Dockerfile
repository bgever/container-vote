FROM microsoft/mssql-server-linux:2017-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=4DevTestOnly
ENV MSSQL_PID=Developer
ENV INIT_DB_DELAY=15

WORKDIR /src

COPY init-db.sql .
COPY entrypoint.sh .

# Grant permissions for the entrypoint shell scripts to be executable.
RUN chmod +x ./entrypoint.sh

CMD bash entrypoint.sh