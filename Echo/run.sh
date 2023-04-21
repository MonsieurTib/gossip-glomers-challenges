dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w echo --bin "$binPath/bin/debug/net7.0/Echo" --node-count 1 --time-limit 10 --log-stderr