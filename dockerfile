FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80
EXPOSE 443
WORKDIR /ToDo
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out



FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /ToDO
COPY --from=build-env /ToDo/out .
ENTRYPOINT ["dotnet", "ToDo.dll","--environment=Development"]
