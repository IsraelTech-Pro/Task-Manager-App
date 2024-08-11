set -o errexit


wget https://download.visualstudio.microsoft.com/download/pr/60218cc4-13eb-41d5-aa0b-5fd5a3fb03b8/6c42bee7c3651b1317b709a27a741362/dotnet-sdk-8.0.303-linux-x64.tar.gz


mkdir -p $HOME/dotnet && tar zxf dotnet-sdk-8.0.303-linux-x64.tar.gz -C $HOME/dotnet


export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet


dotnet --version

cd TaskManagerApp

dotnet publish -c Release -o out

