dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w kafka --bin "$binPath/bin/debug/net7.0/KafkaStyleB" --node-count 2 --concurrency 2n --time-limit 20 --rate 1000 --log-stderr