## Dr X

Dr X vagy rabló-pandúr, egy jól ismert társasjáték ahol a pandúrok a menekülő rablót próbálják elkapni bizonyos időn belül. A játék egy térképen játszódik, ahol utak kötnek össze csomópontokat. A játék célja, hogy legalább egy pandúr azonos csomópontra lépjen a menekülővel. A játék során a pandúrok nem tudják, hol áll éppen a menekülő Dr X, viszont ő minden 4. körben felfedi, hogy hol áll éppen.

Készítsen konzolalkalmazást a fenti játékhoz, a következő szabályok betartásával:

A csomópontok száma: N (10 <N <101). Egy csomópontból bármelyik másik csomópontba ellehet jutni közvetve. A játékosok, azaz a pandúrok száma: M (0 <M < (N/4)). Dr X nem játékos, hanem a program része. Az ő feladata, hogy ne lépjen olyan csomópontra ahol játékos tartózkodik (A hallgatónak nem kell mesterséges intelligenciát írnia). Egy játékos és Dr X is csak a vele szomszédos csomópontokra léphetnek. A bemeneti map.txt állomány első sora a csomópontok számát adja meg (N), a többi N db sorban pedig az adott csomópont, és annak szomszédjai állnak.

### A játék menete:

A játékosok megrajzolják a játék térképét, majd a csomópontok számát és azok kapcsolatát a map.txt állományba írják. A program beolvassa a bemeneti map.txt állományt. Majd a játékosok a konzol segítségével megadják, hányan vannak és elhelyezkednek a pályán. Ez után minden körben a program megadja minden egyes játékosnak, mely csomópontra léphet. A játékosnak megengedett, hogy egy helyben maradjon. Ha egy játékos Dr X mezejére lépett a játék véget ér! Amennyiben egyik játékos se lépett Dr X mezőjére, Dr X lép. Dr X nem léphet játékos csomópontjára, ha minden szomszédos csomópontján játékos áll, Dr X egy helyben marad. Dr X helyzete minden 4. körben nyilvánossá válik arra a körre. A játék N/2 körből áll, ha annyi idő alatt nem sikerült elkapni, akkor Dr X győzött.

map.txt példa:
```
12
ABCDE
BACE
CAB
DA
EABFGI
FEGK
GEFHIJ
HGKL
IGE
JGL
KHLF
LHJK
```

### A feladat során az alábbi limitációk voltak:

Általános, nyelvi elemekkel való megkötés, melyet a házi feladatok során nem használhat a megoldásában:

- Methods: Array.Sort, Array.Reverse, Console.ReadKey, Environment.Exit

- LINQ: System.Linq

- Attributes

- Collections: ArrayList, BitArray, DictionaryEntry, Hashtable, Queue, SortedList, Stack
- Generic collections:    Dictionary<K,V>, HashSet<T>, List<T>, SortedList<T>, Stack<T>, Queue<T>

- Keywords:

    - Modiﬁers: protected, internal, abstract, async, event, external, in, out, sealed, unsafe, virtual, volatile

    - Method parameters: params, in, out

    - Generic type constraint: where

    - Access: base

    - Contextual: partial, when, add, remove, init

    - Statement: checked, unchecked, try-catch-finally, throw, fixed, foreach, continue, goto, yield, lock, break - in loop

    - Operator and Expression:

        - Member access:  ˆ - index from end, .. - range

        - Type-testing: is, as, typeof

        - Conversion: implicit, explicit

        - Pointer: * - pointer, & - address-of, * - pointer indirection, -> - member access

        - Lambda: => - expression, statement

        - Others: ?: - tenary, ! - null forgiving, ?. - null conditional member access, ?[] - null conditional element access, ?? - null coalescing, ??= - null coalescing assignment, :: - namespace alias qualiﬁer, await, default - operator, literal, delegate, is - pattern matching, nameof, sizeof, stackalloc, switch, with - expressiong, operator

    - Types: dynamic, interface, object, Object, var, struct, nullable, pointer, record, Tuple, Func<T>, Action<T>
