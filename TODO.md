# Random Shop (Fumbishop) — TODO & Plan

> Status: in progress. Server-side roll sudah jalan, display machine client masih blocker.

## Status Saat Ini

### Sudah Jalan ✅
- Server startup dengan data client (`item_effect.x7` + enum `Attribute` 12 nilai baru)
- Random shop roll: potong uang → roll item → grant ke inventory
- Item **real** render sempurna di inventory (gambar + nama + durasi hari)
- `RandomShopDto` array-of-structs (`RandomShopItemDto[]`)
- Flow kirim `RandomShopUpdateRequestAck` (1074) → UpdateCheckAck → UpdateInfoAck
- DB: price row Days (1/3/7/15/30) untuk item pool + `1001001` PEN group
- `netsphere.sql` sync (Days rows + hapus 63 package display-only)

### Belum Jalan ❌ (blocker)
- Machine client: **slot item kosong** (pool nggak ke-render)
- Toast **"fail to create item"** pas roll (animasi hasil kosong)
- **Nama item di machine kosong** (client cuma render nama dari package list-nya)

## Akar Masalah

1. **Format wire persis** `RandomShopUpdateInfoAck` (1076) / `RandomShopRollingStartAck` (1077)
   yang diterima client fumbishop **belum ketemu** (tebak-tebakan gagal: parallel arrays, struct array).
2. Client menampilkan **package** (`randomshop_package.x7`) di machine, tapi **grant item real**
   (isi package) — dua hal beda; kita belum punya mapping package→item.
3. Nama machine diambil client dari **`randomshopinfo_string_table.x7`** — item real biasa nggak
   ada di situ.

## Fakta Kunci (hasil riset)

| File client | Isi |
|---|---|
| `new_random_shop.x7` | Harga mesin costume=800 / weapon=3000 PEN; period HOURS 10/7/5/3/1 + NONE; rate prob 3/7/15/170/800/4; gauge_max=10000 |
| `randomshop_package.x7` | 75 package (1001001 "Melee Weapon 1", 2104001 "Male Shirt 1", 1002001 "Shooting Weapon 1", dst) — cuma id + name_key + desc_key |
| `randomshopinfo_string_table.x7` | Nama 70 package (`N{id}`) |
| `randomshop_ani_info.x7` | Slot animasi machine (posisi 2,4,5,6,7) |

- Item yang **real + package** di client: `1001001, 1000001, 3000001, 3000002, 4010001, 4020001, 4020002` (+`4000001/2` tanpa nama)
- Package non-real (`1002001`, `2104001`, dst) → client **freeze** di connecting
- `Unk1 = 31` (EUNewRandomShop) benar; `Unk1 = 0` bikin client ignore pool

## Plan Selanjutnya

### A. Finalisasi pool real (buat langsung playable)
- [ ] Tulis `RandomShop.xml` final: 23 item real, 3 pool, mix permanent + Days (1/3/7/15/30), rate distribusi (1d=80, 3d=34, 7d=15, 15d=7, 30d=3, perm=4)
- [ ] Sync ke `publish\GameServer_LatestOld_Debug\data\xml`
- [ ] Restart & test
- [ ] Terima tradeoff: machine kosong + toast "fail to create item" (kosmetik, item tetap masuk)

### B. Official-like server-side (tanpa RE client)
- [ ] Tambah field `RewardNumber` di `RandomShop.xml` (package buat display + item real buat grant)
- [ ] Roll logic: grant **reward item** (bukan package)
- [ ] Period/rate ikut `new_random_shop.x7` (HOURS prob: 1h=800, 3h=170, 5h=15, 7h=7, 10h=3, NONE=4)
- [ ] Harga mesin: costume 800 PEN / weapon 3000 PEN
- [ ] Config mapping package→item real (bisa dari XML atau DB)

### C. Fix display client (definitif — butuh RE client)
- [ ] Identifikasi packer `E:\S4 League\Client_Release.exe` (tanpa string = packed)
- [ ] Unpack / cari client yang udah unpacked
- [ ] RE handler `RandomShopUpdateInfoAck` (1076) — struktur DTO persis (field order/type)
- [ ] RE handler `RandomShopRollingStartAck` (1077) — struktur persis
- [ ] Extract mapping package→item contents
- [ ] Implement server sesuai format
- [ ] Test: machine slot nongol + toast hilang

### D. Repack client resources (opsional — nama machine)
- [ ] Extract `randomshopinfo_string_table.x7` dari resource client
- [ ] Tambah kunci `N{id}` untuk item pool yang dipake
- [ ] Repack ke `_resources` client
- [ ] Test nama muncul di machine

## Referensi File

- `src/Violet/Constants.cs` — enum `Attribute` (fix item_effect.x7)
- `src/Violet.Network/Data/Game/RandomShopDto.cs` — array-of-structs
- `src/GameServer/Network/Services/ShopService.cs` — handler random shop + 1074
- `data/xml/RandomShop.xml` — pool config
- `netsphere.sql` — schema + harga Days
- `client resources/xml/randomshop_package.x7`
- `client resources/xml/new_random_shop.x7`
- `client resources/language/xml/randomshopinfo_string_table.x7`
- Client: `E:\S4 League\Client_Release.exe`, `E:\S4 League\_resources`
