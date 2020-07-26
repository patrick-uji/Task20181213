# Start from the .NET Core SDK image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-image
WORKDIR /app

# Setup NodeJS
RUN apt-get update -y && \
    apt-get install curl gnupg -y && \
    curl -sL https://deb.nodesource.com/setup_12.x | bash && \
    apt-get install nodejs -y

# Copy the project files and build the project
COPY ./Task20181213.sln ./
COPY ./Task20181213_P2 ./Task20181213_P2/
COPY ./Task20181213_Common ./Task20181213_Common/
RUN dotnet publish Task20181213_P2 -c Release -o out

# Start from the ASP.NET Core runtime image and copy the output from the previous image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-image /app/out .

# Add a "service waiter" program
ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.7.3/wait ./services-wait
RUN chmod +x ./services-wait
CMD ["sh", "-c", "./services-wait && dotnet Task20181213_P2.dll"]
