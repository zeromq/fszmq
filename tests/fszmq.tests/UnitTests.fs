namespace fszmq.tests

open Xunit

module UnitTests =
    
  [<Fact>]
  let ``scratch`` () =
    printfn "This is a test." //TODO: is there a more idiomatically xUnit way to do logging?
    Assert.True(true)
