# gossip-glomers-challenges
My solutions to Gossip-Glomers Fly.io Distributed Systems Challenges in C#

## challenge 1 - echo
There's nothing to do, as it's just an introductory guide to Maelstrom.

[challenge link](https://fly.io/dist-sys/1/)

## challenge 2 - unique Id generation
My solution involves concatenating the nodeId, UNIX timestamp in milliseconds, and an atomic increment.

[challenge link](https://fly.io/dist-sys/2/)

## challenge 3a - brodcast part I - Single node broadcast
Nothing complicated since there is only one node.

[challenge link](https://fly.io/dist-sys/3a/)

## challenge 3b - brodcast part II - Multi node broadcast
This time, there are five nodes. I have used the provided topology to broadcast messages to each neighbor.

[challenge link](https://fly.io/dist-sys/3b/)

## challenge 3c - brodcast part III - Multi node and fault tolerant broadcast
Once again, I have used the provided topology to broadcast messages to each neighbor. RPC calls are retried until the remote node responds

[challenge link](https://fly.io/dist-sys/3c/)
