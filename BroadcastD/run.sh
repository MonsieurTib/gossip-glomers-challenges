dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w broadcast --bin "$binPath/bin/debug/net7.0/BroadcastD" --node-count 25 --time-limit 20 --rate 100 --latency 100