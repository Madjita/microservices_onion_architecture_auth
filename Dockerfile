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
COPY ["Presentation_layer/Presentation_layer.csproj", "Presentation_layer/"]
COPY ["Data_layer/Data_layer.csproj", "Data_layer/"]
COPY ["Infrastructure_layer/Infrastructure_layer.csproj", "Infrastructure_layer/"]
RUN dotnet restore "Presentation_layer/Presentation_layer.csproj"
COPY . .
WORKDIR "/src/Presentation_layer"
RUN dotnet build "Presentation_layer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presentation_layer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation_layer.dll"]
