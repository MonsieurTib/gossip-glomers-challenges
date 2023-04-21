dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w unique-ids --bin "$binPath/bin/debug/net7.0/Maelstrom.UniqueId" --time-limit 30 --rate 1000 --node-count 3 --availability total --nemesis partition