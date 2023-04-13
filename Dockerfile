#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


RUN curl --silent --location https://deb.nodesource.com/setup_10.x | bash -
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx

RUN apt-get install -y nodejs

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuthDomain/AuthDomain.csproj", "AuthDomain/"]
COPY ["AuthDAL/AuthDAL.csproj", "AuthDAL/"]
COPY ["AuthBLL/AuthBLL.csproj", "AuthBLL/"]
RUN dotnet restore "AuthDomain/AuthDomain.csproj"
COPY . .
WORKDIR "/src/AuthDomain"
RUN dotnet build "AuthDomain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthDomain.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthDomain.dll"]
