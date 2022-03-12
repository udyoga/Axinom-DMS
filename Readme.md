## Build DMS-Sender Image
`docker build -t [docker-user]/dmssender . -f ".\DMS-Sender\Dockerfile"`

## Build DMS-Recipient Image
`docker build -t [docker-user]/dmsrecipient . -f ".\DMS-Recipient\Dockerfile"`

## Push Docker image to Docker hub
1. Create your own repository in docker hub 
2. Push image using this command
`docker push [docker-user]/dmssender:latest`

## App settings
1. Update username and password in DMS-Recipient Configs section
2. Change the RecipientAPI port number if you change the docker-compose file 
