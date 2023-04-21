dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w broadcast --bin "$binPath/bin/debug/net7.0/BroadcastC" --node-count 5 --time-limit 20 --rate 10 --nemesis partition