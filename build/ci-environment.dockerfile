FROM mcr.microsoft.com/dotnet/sdk:7.0

RUN export DEBIAN_FRONTEND=noninteractive

# Setup the Microsoft package feed
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
    
# install .NET 6 SDK
RUN apt-get update && apt-get install -y dotnet-sdk-6.0

RUN dotnet tool install Nuke.GlobalTool --global