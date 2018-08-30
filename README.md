# Container based demo voting app

This is a demo solution to showcase how to containerize .NET Core 2.1 based projects.

The solution can be deployed with docker-compose to Azure App Service, and also supports running in a Kubernetes
cluster.

## Structure

### .NET Core Projects

* **ContainerVote.Web** The web frontend hosting the voting and results pages. Submitting the votes and retrieving the
  results happens client-side with JavaScript calls to the API.
* **ContainerVote.Api** The web API, which communicates with the Redis service to queue votes and retrieve the results.
  Once a vote has been queued, it publishes to the Redis 'vote' pub/sub channel.
* **ContainerVote.Store** Worker console app, which retrieves votes from the queue and writes them to a SQL server. The
  process subscribes to the Redis 'vote' channel. Once a event happens, it pops up to 10 votes a time from the queue and
  inserts them into the SQL server. Finally, if votes have been inserted, the process publishes to the Redis 'calculate'
  channel.
* **ContainerVote.Calculate** Worker console app, which process subscribes to the Redis 'calculate' channel. It then
  triggers a calculation helper if no calculation was made in the last 1 second, or schedules the calculation to happen
  1 second later. The calculation helper queries the SQL server for the results and inserts them into the Redis cache.
* **ContainerVote.Shared.Primitives** Contains data transfer objects (DTOs) and the base class for the worker projects.

### Container Services

* **traefik** The reverse proxy routing traffic to either the web or api services. [Traefik](https://traefik.io) is used
  because it can detect service endpoints automatically due to it's neat integration with Docker and Kubernetes.
  Moreover, we don't want to expose the Kestrel servers of the web and api projects directly to the internet.
* **containervote.web** The related web frontend .NET project. 
* **containervote.api** The related web API .NET project.
* **redis** The Redis service in between the web and backend services.
* **sql** (local only) SQL Server 2017 for Linux in Developer mode.
* **containervote.store** The related store .NET project.
* **containervote.calculate** The related calculate .NET project.

## Setup

*NOTE:* The solution has been made on macOS, and hasn't been tested on Windows. There may be path issues when running on
a Windows machine.

1. Ensure you have [Docker](https://www.docker.com) installed and running.

## Running & Debugging

### Visual Studio

- Start debugging.

To stop the running docker-compose project: 

- `docker ps` and note the prefix used by Visual Studio for the docker-compose project.
- `docker-compose -p <prefix> down`

### CLI

- `docker-compose -f docker-compose.yml -f docker-compose.overrides.yml up`
- `docker-compose down` to stop.  

For Kubernetes, see minikube section below.

## Deploying

### Azure App Service

- `az appservice plan create --name <plan> --resource-group <group> --sku B1 --is-linux
- `az webapp create --resource-group <group> --plan <plan>  --name container-vote --multicontainer-config-type compose
  --multicontainer-config-file azure-app-service-docker-compose.yml`
- Set web app configuration entry `SQL_CONNECTION_STRING` - the connection string to SQL server.
- FTP into the web app, and copy `traefik/traefik.toml` to `traefik/traefik.toml`.
- Restart the web app.

### Kubernetes

#### Local (minikube)

- [Install](https://kubernetes.io/docs/setup/minikube/) and run `minikube`.
- `kubectl config use-context minikube`
- Create a plaintext file named `connection-string` (no ext) and add the SQL connection string. 
- `kubectl create secret generic mssql --from-file=./connection-string --namespace=container-vote`
- `kubectl apply -f kubernetes/ -f kubernetes/local/`
- `minikube ip` Reveals the IP to connect to.
- Open a browser, and navigate to the IP. Port 80 hosts the app, port 8080 hosts the Traefik dashboard.

#### Azure Kubernetes Service (AKS)

- Ensure you have the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)
  installed, and are logged in.
- `az aks create --resource-group <group> --name <cluster-name> --node-count <number> --generate-ssh-keys`
- `az aks get-credentials --resource-group <group> --name <cluster-name>`
- `kubectl config use-context <cluster-name>` (optional with previous step)
- Create a plaintext file named `connection-string` (no ext) and add the SQL connection string. 
- `kubectl create secret generic mssql --from-file=./connection-string --namespace=container-vote`
- `kubectl apply -f kubernetes/ -f kubernetes/azure/`
- `kubectl --namespace container-vote get service` Note the EXTERNAL-IP of `traefik-ingress-service`.
- Visit the external IP address to see the app. 