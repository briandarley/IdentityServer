#sql image
docker run -d --rm -p 1433:1433 --network=shared_network --name sql -h sql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=4qsW5psH2MXY" mcr.microsoft.com/mssql/server:2022-latest

#must be at root, even though we're specifying the dockerfile full path
docker build -f "F:\Projects\UNC.V2\IdentityServer4\src\host\Dockerfile" -t bdarley/identity-server .

docker run `
		-d `
		--network=shared_network `
		--name identity-server `
		-p 5003:80 -p 5002:443 `
		-e "ASPNETCORE_URLS=https://+:443;http://+:80" `
		-e "DEFAULT_CONNECTION=Server=sql,1433;Database=IdentityServer4_AspNetIdentity;User Id=sa; Password=4qsW5psH2MXY;TrustServerCertificate=true;MultipleActiveResultSets=true" `
		-e "ASPNETCORE_Kestrel__Certificates__Default__Password=password" `
		-e "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx" `
		-v F:\Projects\Sandbox\IdentityServer\IdentityServer4\src\host\IdKeys\:/IdKeys `
		-v f:\temp\https\:/https `
		bdarley/identity-server



docker-compose -f "F:\Projects\Sandbox\IdentityServer\IdentityServer4\src\docker-compose.yml" build
docker-compose -f "F:\Projects\Sandbox\IdentityServer\IdentityServer4\src\docker-compose.yml" -p restservices up -d 


Reminder, in order to debug using docker-compose, ensure docker-compose is set as the startup project.

