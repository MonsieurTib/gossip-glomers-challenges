dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w broadcast --bin "$binPath/bin/debug/net7.0/BroadcastB" --node-count 5 --time-limit 20 --rate 10