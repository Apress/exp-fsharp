#light

// ----------------------------
// Listing 13-11.

open Microsoft.FSharp.Control.Mailboxes

/// The internal type of messages for the agent
type internal msg = Increment of int | Fetch of IChannel<int> | Stop

type CountingAgent() =
    let counter = MailboxProcessor.Start(fun inbox ->
             // The states of the message-processing state machine...
             let rec loop(n) =
                async { let! msg = inbox.Receive()
                        match msg with
                        | Increment m ->
                            // increment and continue.
                            return! loop(n+m)
                        | Stop ->
                            // exit
                            return ()
                        | Fetch  replyChannel  ->
                            // post response to reply channel and continue
                            do replyChannel.Post(n)
                            return! loop(n) }

             // The initial state of the message-processing state machine...
             loop(0))

    member a.Increment(n) = counter.Post(Increment(n))
    member a.Stop() = counter.Post(Stop)
    member a.Fetch() = counter.PostSync(fun replyChannel -> Fetch(replyChannel))

// ----------------------------

let counter = new CountingAgent()

counter.Increment(1)

counter.Fetch()

counter.Increment(2)

counter.Fetch()

counter.Stop()
