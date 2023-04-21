# gossip-glomers-challenges
My solutions to [Fly.io distributed systems challenges](https://fly.io/dist-sys) in C#

## challenge 1 - Echo
There's nothing to do, as it's just an introductory guide to Maelstrom.

[challenge link](https://fly.io/dist-sys/1/)

## challenge 2 - Unique Id generation
My solution involves concatenating the nodeId, UNIX timestamp in milliseconds, and an atomic increment.

[challenge link](https://fly.io/dist-sys/2/)

## challenge 3a - Brodcast part I - Single node broadcast
Nothing complicated since there is only one node.

[challenge link](https://fly.io/dist-sys/3a/)

## challenge 3b - Brodcast part II - Multi node broadcast
This time, there are five nodes. I have used the provided topology to broadcast messages to each neighbor.

[challenge link](https://fly.io/dist-sys/3b/)

## challenge 3c - brodcast part III - Multi node and fault tolerant broadcast
Once again, I have used the provided topology to broadcast messages to each neighbor. RPC calls are retried until the remote node responds

[challenge link](https://fly.io/dist-sys/3c/)

## challenge 3d - Brodcast part IV - Efficient Broadcast
 Now, this is more challenging! We have to respect some constraints :

- Messages-per-operation is below 30
- Median latency is below 400ms
- Maximum latency is below 600ms

My solution is to divide the 25 nodes into two groups, each with a master node. Each master node broadcasts messages to its children as well as the other master node.

My result : 
- Messages-per-operation : 23.21
- Median latency : 192ms
- Maximum latency : 316ms

[challenge link](https://fly.io/dist-sys/3d/)

## challenge 3e - Brodcast part IV - Efficient Broadcast (bis)
 constraints :

- Messages-per-operation is below 20
- Median latency is below 1s
- Maximum latency is below 2s

This is the same solution as before, but with messages now broadcasted in batches to ensure that the message rate stays below 20 messages per second with 25 nodes.

My result : 
- Messages-per-operation : 4.59
- Median latency : 442ms
- Maximum latency : 988ms

[challenge link](https://fly.io/dist-sys/3e/)

## challenge 4 - Grow-Only Counter
 
Solution need only be eventually consistent: given a few seconds without writes, it should converge on the correct counter value.
My solution : each node replicates local data to other nodes.

[challenge link](https://fly.io/dist-sys/4/)

## challenge 5a - Kafka style part I - Single-Node Kafka-Style Log
 
Nothing complicated since there is only one node.

[challenge link](https://fly.io/dist-sys/5a/)

## challenge 5b/5cc - Kafka style part II/III - Multi-Node Kafka-Style Log / Efficient Kafka-Style Log
 
My solution worked for the last two Kafka challenges without using the seq-kv provided by Maelstrom. 
I declared the first node as the master and the other nodes forwarded messages to the master and read from it.

- Messages-per-operation : 2.3
- Availability : 0.9991452
- Throughput max : 350 hz

[challenge link](https://fly.io/dist-sys/5b/)
[challenge link](https://fly.io/dist-sys/5c/)

## challenge 6a - Totally-Available part I - Single-Node, Totally-Available Transactions
 
Nothing complicated since there is only one node.

[challenge link](https://fly.io/dist-sys/6a/)

## challenge 6b - Totally-Available part II - Totally-Available, Read Uncommitted Transactions
 
Each transaction is replicated using my rpc implementation (infinite retries :-) )

[challenge link](https://fly.io/dist-sys/6b/)

## challenge 6c - Totally-Available part III - Totally-Available, Read Committed Transactions
 
Same solution as 6b, test is passing..

[challenge link](https://fly.io/dist-sys/6c/)
