dotnet build
binPath="$(pwd)"
cd $MAELSTROM_PATH
./maelstrom test -w txn-rw-register --bin "$binPath/bin/debug/net7.0/TotallyAvailableC" --node-count 2 --concurrency 2n --time-limit 20 --rate 1000 --consistency-models read-committed --availability total --nemesis partition --log-stderr