module Casanova

let variable_counter = ref 0
let commit_variable_updates() =
  variable_counter := !variable_counter + 1

type Variable<'a> =
  struct
    val internal values : 'a[]

    new (f:unit->'a) = { values = [| f(); f() |] }
    new (v1:'a,v2:'a) = { values = [| v1; v2 |] }

    member this.Value
      with get() = this.values.[variable_counter.Value % 2]

    override this.ToString() = 
      "(" +
        this.values.[variable_counter.Value % 2].ToString() + ", " +
        this.values.[(variable_counter.Value + 1) % 2].ToString() + ")"

  end

let (!) (v:Variable<'a>) = v.Value
let (:=) (v:Variable<'a>) v' = v.values.[(variable_counter.Value + 1) % 2] <- v'
let variable<'a when 'a : struct> (v:'a) = Variable<'a>(fun () -> v)

let immediate_read (v:Variable<'a>) = v.values.[(variable_counter.Value + 1) % 2]
let immediate_write (v:Variable<'a>) v' = v.values.[(variable_counter.Value + 1) % 2] <- v'

type VariableList<'a> =
  struct
    val internal values : ResizeArray<'a>[]

    interface System.Collections.Generic.IEnumerable<'a> with
       member this.GetEnumerator () : System.Collections.Generic.IEnumerator<'a> = this.values.[variable_counter.Value % 2].GetEnumerator() :> System.Collections.Generic.IEnumerator<'a>
       member this.GetEnumerator () : System.Collections.IEnumerator = this.values.[variable_counter.Value % 2].GetEnumerator() :> System.Collections.IEnumerator

    new (f:unit->'a ResizeArray) = { values = [| f(); f() |] }

    member this.Value
      with get() = this.values.[variable_counter.Value % 2] :> seq<'a>
      and set v' = 
        let value' = this.values.[(variable_counter.Value + 1) % 2]
        do value'.Clear()
        for x in v' do
          do value'.Add x          

    override this.ToString() = 
      "(" +
        this.values.[variable_counter.Value % 2].ToString() + ", " +
        this.values.[(variable_counter.Value + 1) % 2].ToString() + ")"
  end

let (!!) (v:VariableList<'a>) = v.values.[variable_counter.Value % 2]
let (.=) (v:VariableList<'a>) v' = v.values.[(variable_counter.Value + 1) % 2] <- v'
let variable_list (v:'a) = Variable<'a>(fun () -> v)
