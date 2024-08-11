
set -o errexit


wget https://download.visualstudio.microsoft.com/download/pr/xxxxxx/dotnet-sdk-8.0.x-linux-x64.tar.gz


mkdir -p $XDG_CACHE_HOME/dotnet && tar zxf dotnet-sdk-8.0.x-linux-x64.tar.gz -C $XDG_CACHE_HOME/dotnet


export DOTNET_ROOT=$XDG_CACHE_HOME/dotnet
export PATH=$PATH:$XDG_CACHE_HOME/dotnet

dotnet --version


cd TaskManagerApp

dotnet publish -c Release -o out


cp -r out/* /var/www/html
