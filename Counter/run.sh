dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w g-counter --bin "$binPath/bin/debug/net7.0/Counter" node-count 3 --rate 100 --time-limit 20 --nemesis partition