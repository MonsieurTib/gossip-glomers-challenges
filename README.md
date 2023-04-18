# gossip-glomers-challenges
My solutions to Gossip-Glomers Fly.io Distributed Systems Challenges in C#

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
