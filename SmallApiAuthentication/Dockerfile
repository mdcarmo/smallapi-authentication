FROM repo.br.scania.com:35007/dotnet/core/sdk:3.0 AS build
EXPOSE 5000
# COPY ./nuget.config /
WORKDIR /app
# copy csproj and restore as distinct layers
COPY ./*.csproj ./
WORKDIR /app/
# --configfile ../nuget.config
COPY nuget.config .
RUN dotnet restore --configfile ./nuget.config
# copy and publish app and libraries
WORKDIR /app/
COPY ./. ./
WORKDIR /app/
RUN dotnet publish -o out
FROM repo.br.scania.com:35007/dotnet/core/aspnet:3.0 AS runtime
ENV ORA_SDTZ "America/Sao_Paulo"
ENV TZ ${ORA_SDTZ}
RUN echo ${TZ} > /etc/timezone 
RUN ln -fs "/usr/share/zoneinfo/${TZ}" /etc/localtime
RUN dpkg-reconfigure -f noninteractive tzdata
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "SmallApi.dll"] #trocar para o nome do projeto
