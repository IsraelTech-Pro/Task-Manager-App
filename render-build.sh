
set -o errexit


wget https://download.visualstudio.microsoft.com/download/pr/d3e46476-4494-41b7-a628-c517794c5a6a/6066215f6c0a18b070e8e6e8b715de0b/dotnet-sdk-6.0.402-linux-x64.tar.gz 


mkdir -p $XDG_CACHE_HOME/dotnet && tar zxf dotnet-sdk-6.0.402-linux-x64.tar.gz -C $XDG_CACHE_HOME/dotnet


export DOTNET_ROOT=$XDG_CACHE_HOME/dotnet
export PATH=$PATH:$XDG_CACHE_HOME/dotnet


dotnet --version


cd TaskManagerApp


dotnet publish -c Release -o out


cp -r out/* /var/www/html
