This was a first semester university project. The UI and the task description were written in Hungarian.

<br/>

Ez egy féléves feladat volt az első szemeszterben. A leírás a [leiras.md](leiras.md)-ben található.

A leírásban található limitációk miatt a projektben több nem parktikus/ronda vagy egyéb módon nem megfelelő megoldás látható, ezeknek a követése erősen ellenjavallt (kivéve ha neked is ezek a limitációk).

Ennek a repo-nak nem az a célja hogy innentől mindenki ezt adja be. Azért hoztam létre, hogy hátha találsz benne valami jó ötletet vagy megoldást amit fel tudsz használni, de NEM átmásolni.

Ha szeretnéd továbbadni másnak és ezt a repo linkjével teszed azt megköszönöm.

<br/>

## Ismertető:

A helyes megjelenítéshez legalább Windows 10 szükséges mert a korábbi console fontkészletből hiányoznak a használt karakterek. (A "graph drawing elements.txt"-ben vannak a használt karakterek)

A map.txt-t mindig a a gyökérkönyvtárban keresi. A váltáshoz a maps mappából kell a map.txt-t felülírni.

A játék indításakor megkérdezi a játékosok számát. Ez után az alábbi lépések következnek:
1. Megkérdezi a játékos nevét
2. Megkérdezi, hogy honnan szeretne indulni a játékos
3. Addig ismétli amíg az összes játékos létre nem jött

A játék aktuális állapota az ablak bal alsó részén jelenik meg.

Az aktív játékos mindig világoskékkel jelenik meg a térképen és az információs sávon.

Addig tart a játék amíg DrX-et el nem kapjátok vagy el nem fogy a lehetséges körök száma. (pontok száma / 2)


## Pár részlet:

- A map.txt elérési útja a Game.cs 29. sorában módosítható

- Az ablak futásidőben átméretezhető és ehhez idomul az ablak tartalma is.

- A nyilak használatával bármikor mozgatható a térkép.

- A térkép csak akkor generálható ha a gráf síkba rajzolható és létre lehet úgy hozni, hogy:
    1. a leghosszabb hurokmentes szakaszt vízszintesen felrajzoljuk
    2. az esetlegesen kimaradt pontokat a megfelelő helyre hozzáfűzzük és ezután
    3. az összekötendő pontokat a legrövidebbtől kezdve összekötjük a legrövidebb útvonallal.

- További limitáció, hogy 1 ponthoz maximum 8 másik pont lehet hozzákötve (ha több van akkor a alábbi érvényesül).

- Előfordulhat, hogy nem sikerül a térképet felrajzolni a fenti algoritmussal, ekkor a DrX felirat jelenik meg a térkép helyén. Ez semmiben sem befolyásolja a játék menetét!

- A maps mappában mellékelt map-ok garantáltan játszhatóak a térkép megjelenítésével.

- Bármelyik map-on lehet játszani megjelenítés nélkül a "nomap" paraméterrel indítva a játékot: `./DrX.exe nomap`