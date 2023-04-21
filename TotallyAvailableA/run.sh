dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w txn-rw-register --bin "$binPath/bin/debug/net7.0/TotallyAvailableA" --node-count 1 --time-limit 20 --rate 1000 --concurrency 2n --consistency-models read-uncommitted --availability total --log-stderr